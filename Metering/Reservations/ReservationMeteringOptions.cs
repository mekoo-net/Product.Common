using Microsoft.Extensions.Options;

namespace Product.Common.Metering.Reservations;

public sealed class ReservationMeteringOptions
{
    public TimeSpan DefaultReservationTtl { get; set; } = TimeSpan.FromMinutes(20);
    public TimeSpan MaxReservationTtl { get; set; } = TimeSpan.FromMinutes(30);
    public int DefaultEstimatedCompletionTokens { get; set; } = 512;
}

public sealed class ReservationMeteringOptionsValidator(string sectionName)
    : IValidateOptions<ReservationMeteringOptions>
{
    public ValidateOptionsResult Validate(string? name, ReservationMeteringOptions options)
    {
        if (options.DefaultReservationTtl <= TimeSpan.Zero)
            return ValidateOptionsResult.Fail($"{sectionName}.DefaultReservationTtl must be > 0.");
        if (options.MaxReservationTtl <= TimeSpan.Zero)
            return ValidateOptionsResult.Fail($"{sectionName}.MaxReservationTtl must be > 0.");
        if (options.MaxReservationTtl < options.DefaultReservationTtl)
            return ValidateOptionsResult.Fail($"{sectionName}.MaxReservationTtl must be >= DefaultReservationTtl.");
        if (options.DefaultEstimatedCompletionTokens < 0)
            return ValidateOptionsResult.Fail($"{sectionName}.DefaultEstimatedCompletionTokens must be >= 0.");
        return ValidateOptionsResult.Success;
    }
}
