using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Product.Common.Metering.Reservations;

public static class ReservationMeteringServiceExtensions
{
    public static IServiceCollection AddReservationMeteringOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.AddOptions<ReservationMeteringOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<ReservationMeteringOptions>>(
            _ => new ReservationMeteringOptionsValidator(sectionName));
        return services;
    }
}
