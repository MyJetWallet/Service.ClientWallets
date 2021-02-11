using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using MyNoSqlServer.DataReader;
using ProtoBuf.Grpc.Client;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.MyNoSql;

namespace Service.ClientWallets.Client
{
    [UsedImplicitly]
    public class ClientWalletsClientFactory
    {
        private readonly MyNoSqlReadRepository<ClientWalletNoSqlEntity> _reader;
        private readonly CallInvoker _channel;

        public ClientWalletsClientFactory(string clientWalletsGrpcServiceUrl, MyNoSqlReadRepository<ClientWalletNoSqlEntity> reader)
        {
            _reader = reader;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(clientWalletsGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IClientWalletService ClientWalletService() => new NoSqlClientWalletService(_channel.CreateGrpcService<IClientWalletService>(), _reader);
    }

    public static class ClientWalletsAutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IAssetsDictionaryClient
        ///   * ISpotInstrumentDictionaryClient
        /// </summary>
        public static void RegisterAssetsDictionaryClients(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var assetSubs = new MyNoSqlReadRepository<AssetNoSqlEntity>(myNoSqlSubscriber, AssetNoSqlEntity.TableName);
            var brandSubs = new MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity>(myNoSqlSubscriber, BrandAssetsAndInstrumentsNoSqlEntity.TableName);
            var instrumentSubs = new MyNoSqlReadRepository<SpotInstrumentNoSqlEntity>(myNoSqlSubscriber, SpotInstrumentNoSqlEntity.TableName);

            builder
                .RegisterInstance(new AssetsDictionaryClient(assetSubs, brandSubs))
                .As<IAssetsDictionaryClient>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterInstance(new SpotInstrumentDictionaryClient(instrumentSubs, brandSubs))
                .As<ISpotInstrumentDictionaryClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
