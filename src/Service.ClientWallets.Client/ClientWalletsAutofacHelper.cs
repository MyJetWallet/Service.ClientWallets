using Autofac;
using JetBrains.Annotations;
using MyNoSqlServer.DataReader;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.MyNoSql;
// ReSharper disable UnusedMember.Global

namespace Service.ClientWallets.Client
{
    [UsedImplicitly]
    public static class ClientWalletsAutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IClientWalletService
        /// </summary>
        public static void RegisterClientWalletsClients(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber, string clientWalletsGrpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<ClientWalletNoSqlEntity>(myNoSqlSubscriber, ClientWalletNoSqlEntity.TableName);

            var factory = new ClientWalletsClientFactory(clientWalletsGrpcServiceUrl, subs);

            builder
                .RegisterInstance(factory.ClientWalletService())
                .As<IClientWalletService>()
                .AutoActivate()
                .SingleInstance();
        }

        /// <summary>
        /// Register interfaces:
        ///   * IClientWalletService
        /// </summary>
        public static void RegisterClientWalletsClientsWithoutCache(this ContainerBuilder builder, string clientWalletsGrpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<ClientWalletNoSqlEntity>(null, ClientWalletNoSqlEntity.TableName);

            var factory = new ClientWalletsClientFactory(clientWalletsGrpcServiceUrl, subs);

            builder
                .RegisterInstance(factory.ClientWalletService())
                .As<IClientWalletService>()
                .AutoActivate()
                .SingleInstance();
        }

    }
}