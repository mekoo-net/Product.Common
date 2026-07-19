namespace Product.Common.Auth.Staff;

public sealed record PermissionCatalogItem(string Code, string? Description, bool ReadOnly);

public sealed record PermissionCatalogRegistration(
    string ProductKey,
    IReadOnlyCollection<PermissionCatalogItem> Items);
