using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.Domain.Models;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.ClientWallets.MyNoSql;

namespace Service.ClientWallets.Services
{
    public class ClientWalletService: IClientWalletService
    {
        private readonly ILogger<ClientWalletService> _logger;
        private readonly IMyNoSqlServerDataWriter<ClientWalletNoSqlEntity> _writer;

        public ClientWalletService(ILogger<ClientWalletService> logger, IMyNoSqlServerDataWriter<ClientWalletNoSqlEntity> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task<ClientWalletList> GetWalletsByClient(JetClientIdentity clientId)
        {
            _logger.LogInformation("Request wallets for Broker/Brand/Client: {brokerId}/{brandId}/{clientId}",  
                clientId.BrokerId, clientId.BrandId, clientId.ClientId);

            //todo: store wallets in postgress database
            var entity = await _writer.GetAsync(
                ClientWalletNoSqlEntity.GeneratePartitionKey(clientId.BrokerId, clientId.BrandId),
                ClientWalletNoSqlEntity.GenerateRowKey(clientId.ClientId));

            if (entity == null || !entity.Wallets.Any())
            {
                entity = ClientWalletNoSqlEntity.Create(clientId, new List<ClientWallet>()
                {
                    new ClientWallet()
                    {
                        WalletId = $"{clientId.ClientId}--default",
                        Name = "Wallet",
                        IsDefault = true
                    }
                });

                await _writer.InsertOrReplaceAsync(entity);
            }

            return new ClientWalletList()
            {
                Wallets = entity.Wallets
            };
        }
    }
}