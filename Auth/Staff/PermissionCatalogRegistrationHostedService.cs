using Meeko.Contracts.Keystone;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Product.Common.PlatformServices;

namespace Product.Common.Auth.Staff;

/// <summary>
/// 启动时把 Product 权限目录注册到 Keystone（幂等）。Keystone 未就绪时退避重试，失败不阻塞 Product 启动。
/// </summary>
public sealed class PermissionCatalogRegistrationHostedService : BackgroundService
{
    private const int MaxAttempts = 10;

    private readonly KeystonePlatformService _keystone;
    private readonly PermissionCatalogRegistration _registration;
    private readonly ILogger<PermissionCatalogRegistrationHostedService> _logger;

    public PermissionCatalogRegistrationHostedService(
        KeystonePlatformService keystone,
        PermissionCatalogRegistration registration,
        ILogger<PermissionCatalogRegistrationHostedService> logger)
    {
        _keystone = keystone;
        _registration = registration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cmd = new RegisterPermissionCatalogCommand
        {
            ProductKey = _registration.ProductKey,
            Items = _registration.Items
                .Select(i => new PermissionRegistrationItem
                {
                    Code = i.Code,
                    Description = i.Description,
                    ReadOnly = i.ReadOnly,
                })
                .ToArray(),
        };

        for (var attempt = 1; attempt <= MaxAttempts && !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                var result = await _keystone.StaffAdmin.RegisterPermissionCatalogAsync(cmd);
                if (result.Success)
                {
                    _logger.LogInformation(
                        "{Product} permission catalog registered to Keystone: {Total} codes (+{Added} new, +{Grants} role grants)",
                        _registration.ProductKey, cmd.Items.Length, result.PermissionsAdded, result.GrantsAdded);
                    return;
                }

                _logger.LogWarning("{Product} permission catalog registration rejected: {Message}",
                    _registration.ProductKey, result.FailureMessage);
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(5 * attempt, 30));
                _logger.LogWarning(ex,
                    "{Product} permission catalog registration attempt {Attempt}/{Max} failed; retrying in {Delay}s",
                    _registration.ProductKey, attempt, MaxAttempts, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Product} permission catalog registration failed after {Max} attempts",
                    _registration.ProductKey, MaxAttempts);
                return;
            }
        }
    }
}
