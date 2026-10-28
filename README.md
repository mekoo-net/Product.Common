# Product.Common

产品层（Yomi / Tavern / ToApi）共享约定，引用 Platform.Common。git submodule，上游
`github.com/mekoo-net/Product.Common`，检出在 `{Demux,Tavern,ToApi,Platform}/Common/`。

**必须用公共组件、禁止自造**的策略见 [Platform.Common/README.md](../Platform.Common/README.md#策略必须用公共组件禁止自造)，
对产品仓同样是硬规则。本文只补充产品层特有的部分。

## 分层规则

1. **产品项目不直接 `using Platform.Common.Hosting`。** 连接配置、Kestrel 都走
   `ProductInfrastructureExtensions`（`GetRequiredProductDbConnectionString()` 等）。
   Product.Common 没包装、产品又需要的能力，在 Product.Common 里加包装，
   而不是在产品里直接引用 Platform.Common.Hosting 或者自己写一份。
2. **两边都有的类型，用 Product.Common 那份。** 身份、鉴权、权限（`Product.Common.Identity` /
   `.Auth` / `.Authorization`）在两个库里各有一份，产品仓一律用 Product.Common 的。
3. **调平台服务走 PlatformServices 门面**（Billing / Keystone / Storage），不自己 new gRPC 客户端，
   不自己拼 MagicOnion 调用。

## 组件清单

| 需求 | 用这个 |
|---|---|
| PG / Redis 连接串 | `GetRequiredProductDbConnectionString()` / `GetProductDbConnectionString()` / `GetProductRedisConnectionString()` |
| 缓存 | `AddProductCache()` |
| EF 迁移 | `ProductDatabaseHosting.MigrateAsync<TContext>()` |
| 宿主启动 | `ProductHostBootstrap.RunAsync()` |
| 三端口 Kestrel（Http / InternalRpc / BackendRpc） | `GetKestrelEndpointPort()` / `RequireLocalPort()`；端口只写在 yaml，代码里不写数字 |
| REST 面 | `AddProductRestSurface()` / `AddProductStaffSurface()` / `MapProductHttpEndpoints()` |
| RPC 端口托管与鉴权 | `ProductRpcHosting`（`MapInternalRpcGroup` / `MapBackendRpcGroup` 及对应鉴权管线） |
| BackendRpc HMAC 鉴权 | `AddBackendRpcAuth()` / `AddBackendRpcCredentialAuth()` |
| 服务发现与注册 | `AddProductServiceClients()` / `AddProductConsulRegistration(serviceKey)` |
| 调 Billing / Keystone / Storage | `AddBillingPlatformService()` / `AddKeystonePlatformService()` / `AddStoragePlatformService()` |
| 预占计费 | `AddReservationMeteringOptions()` |
| Gateway Consul 与模型目录 KV | `AddGatewayConsul()` / `GatewayConsulPaths` / `ProviderCatalogWire` |
| access JWT 验签 | `AccessTokenJwtValidator` / `AccessTokenJwtValidation` |
| access JWT 缓存 | `AccessTokenCache.KeyHash()` / `PositiveTtl()` |
| 身份与权限 | `AddCurrentAuth()` / `AddCurrentActor()` / `AddGatewayForwardedAuth()` / `RequirePermission(code)` |
| Staff 权限目录 | `AddProductStaffPermissionSources()` / `AddProductPlatformPermissions()` |
| 响应信封 | `ProductApiEnvelope` |

新增组件时补进这张表。
