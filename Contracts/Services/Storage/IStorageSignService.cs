using MagicOnion;
using Meeko.Contracts.Storage.Dtos;

namespace Meeko.Contracts.Storage;

/// <summary>
/// 存储签名 RPC：产品服务换预签名 URL，密钥不出存储服务。
/// </summary>
public interface IStorageSignService : IService<IStorageSignService>
{
    UnaryResult<StorageSignPutResult> SignPutAsync(StorageSignPutCommand cmd);
    UnaryResult<string?> SignGetAsync(StorageSignGetQuery query);

    /// <summary>
    /// 上传完成确认：服务端读 staging 字节验 sha256，通过后落到内容寻址 blob key。
    /// 返回的 <see cref="StorageConfirmResult.StorageKey"/> 才是业务应持久化的最终 key。
    /// </summary>
    UnaryResult<StorageConfirmResult> ConfirmAsync(StorageConfirmCommand cmd);

    /// <summary>storage key → 当前后端配置下的公开访问 URL；key 无效返回 null。业务表只存 key，出口经此解析。</summary>
    UnaryResult<string?> ResolveUrlAsync(string storageKey);
}
