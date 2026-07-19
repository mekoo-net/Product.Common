using Platform.Common.Web;
using Microsoft.AspNetCore.Http;

namespace Product.Common.Web;

public sealed record ApiEnvelope<T>(bool Success, T? Data, string? Message = null);

public static class ApiEnvelope
{
    public static IResult Ok<T>(T data, string? message = null)
        => Platform.Common.Web.ApiEnvelope.Ok(data, message);

    public static IResult Failure<T>(string message, T? data = default)
        => Platform.Common.Web.ApiEnvelope.Failure(message, data);

    public static IResult Failure(string message)
        => Platform.Common.Web.ApiEnvelope.Failure(message);

    public static IResult NotImplemented(string operation)
        => Platform.Common.Web.ApiEnvelope.NotImplemented(operation);

    public static IResult NotImplemented<T>(string operation, T? data = default)
        => Platform.Common.Web.ApiEnvelope.NotImplemented(operation, data);
}

public sealed class ItemsEnvelope<T>
{
    public T[] Items { get; init; } = [];
    public int Total { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public static class EpochMillis
{
    public static DateTime? ToUtcDateTime(long? millis) => Platform.Common.Web.EpochMillis.ToUtcDateTime(millis);
    public static DateTime ToUtcDateTime(long millis) => Platform.Common.Web.EpochMillis.ToUtcDateTime(millis);
}
