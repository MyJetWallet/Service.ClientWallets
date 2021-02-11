using Autofac;
using JetBrains.Annotations;
using MyNoSqlServer.DataReader;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.MyNoSql;

namespace Service.ClientWallets.Client
{
    [UsedImplicitly]
    public static class ClientWalletsAutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IAssetsDictionaryClient
        ///   * ISpotInstrumentDictionaryClient
        /// </summary>
        public static void RegisterAssetsDictionaryClients(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber, string clientWalletsGrpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<ClientWalletNoSqlEntity>(myNoSqlSubscriber, ClientWalletNoSqlEntity.TableName);

            var factory = new ClientWalletsClientFactory(clientWalletsGrpcServiceUrl, subs);

            builder
                .RegisterInstance(factory.ClientWalletService())
                .As<IClientWalletService>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}