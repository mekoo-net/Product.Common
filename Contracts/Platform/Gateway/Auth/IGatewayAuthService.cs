using MagicOnion;
using MessagePack;

namespace Meeko.Contracts.Gateway.Auth;

/// <summary>
/// Gateway → Keystone：JWT 撤销查询 + API Key 解析。Meeko-Keystone.md §7.2 + §10.2。
/// 两个端点都走 FusionCache：Gateway 命中缓存时不调用 Keystone。
/// </summary>
public interface IGatewayAuthService : IService<IGatewayAuthService>
{
    /// <summary>
    /// 通过 SHA-256 后的 KeyHash 解析 API Key。返回 null 表示 Key 不存在 / 已撤销 / 已过期。
    /// </summary>
    UnaryResult<ApiKeyResolution?> ResolveApiKeyAsync(string keyHash);

    /// <summary>
    /// 查询某个 Access Token 的 jti 是否已加黑名单（改密、登出、强制下线）。
    /// </summary>
    UnaryResult<bool> IsTokenRevokedAsync(string jti);

    /// <summary>
    /// 验签 Keystone access JWT 并返回调用者身份。Hosted gateway（如 Demux.Gateway）通过 RPC 验票，不持有签名密钥。
    /// </summary>
    UnaryResult<AccessTokenResolution?> ValidateAccessTokenAsync(string accessToken);

    /// <summary>
    /// ES256 验签公钥（Account + Staff）。Meeko.Gateway 经 InternalRpc 拉取，不走 HTTP JWKS。
    /// </summary>
    UnaryResult<JwtSigningKeySet> GetJwtSigningKeysAsync();
}

[MessagePackObject]
public sealed class JwtSigningKeySet
{
    [Key(0)] public required JwtSigningKey[] Keys { get; init; }
}

/// <summary>EC P-256 公钥材料（JWKS 字段子集，MessagePack 直传）。</summary>
[MessagePackObject]
public sealed class JwtSigningKey
{
    [Key(0)] public required string KeyId { get; init; }
    [Key(1)] public required string Kty { get; init; }
    [Key(2)] public required string Crv { get; init; }
    [Key(3)] public required string X { get; init; }
    [Key(4)] public required string Y { get; init; }
    [Key(5)] public required string Alg { get; init; }
    [Key(6)] public required string Use { get; init; }
}

[MessagePackObject]
public sealed class ApiKeyResolution
{
    [Key(0)] public required long AccountUid { get; init; }
    [Key(1)] public required long KeyUid { get; init; }
    [Key(2)] public required long CreatedByIamUserUid { get; init; }
    [Key(3)] public required string[] Scopes { get; init; }

    /// <summary>personal / organization。</summary>
    [Key(4)] public required string AccountType { get; init; }
}

[MessagePackObject]
public sealed class AccessTokenResolution
{
    [Key(0)] public required long AccountUid { get; init; }
    [Key(1)] public long? IamUserUid { get; init; }
    [Key(2)] public string? Actor { get; init; }
    [Key(3)] public string? Role { get; init; }
    [Key(4)] public long? StaffUid { get; init; }
    [Key(5)] public string? AccountType { get; init; }
}
