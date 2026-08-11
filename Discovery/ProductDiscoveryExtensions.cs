using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Common.Auth.InternalRpc;
using Platform.Common.Discovery;

namespace Product.Common.Discovery;

public static class ProductDiscoveryExtensions
{
    /// <summary>
    /// Product 侧 gRPC 客户端栈（Consul 解析 + Gateway 身份透传）。须在 AddServiceDiscovery 之后调用。
    /// </summary>
    public static WebApplicationBuilder AddProductServiceClients(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection(DiscoveryOptions.SectionName);
        builder.Services.Configure<ProductDiscoveryOptions>(section);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<OutgoingContextPropagationFilter>();

        // InternalRpc 凭据（调用方侧）。未配置 key 时 filter 什么都不加，所以无条件注册即可。
        builder.Services.Configure<InternalRpcAuthOptions>(
            builder.Configuration.GetSection(InternalRpcAuthOptions.SectionName));
        builder.Services.AddSingleton<InternalRpcCredentialSigner>();
        builder.Services.AddSingleton<InternalRpcCredentialFilter>();

        builder.Services.AddSingleton<Grpc.Net.Client.Balancer.ResolverFactory, ConsulGrpcResolverFactory>();
        builder.Services.AddSingleton<IGrpcChannelFactory, GrpcChannelFactory>();
        builder.Services.AddSingleton<IHttpEndpointResolver, ConsulHttpEndpointResolver>();

        return builder;
    }
}
