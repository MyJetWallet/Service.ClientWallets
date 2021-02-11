using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.ClientWallets.Grpc;

namespace Service.ClientWallets.Client
{
    [UsedImplicitly]
    public class ClientWalletsClientFactory
    {
        private readonly CallInvoker _channel;

        public ClientWalletsClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IClientWalletService ClientWalletService() => _channel.CreateGrpcService<IClientWalletService>();
    }
}
