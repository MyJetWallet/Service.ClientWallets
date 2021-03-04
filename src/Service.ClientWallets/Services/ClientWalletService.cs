using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.Domain.Models;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.ClientWallets.MyNoSql;
using Service.ClientWallets.Postgres;

namespace Service.ClientWallets.Services
{
    public class ClientWalletService: IClientWalletService
    {
        private readonly ILogger<ClientWalletService> _logger;
        private readonly IMyNoSqlServerDataWriter<ClientWalletNoSqlEntity> _writer;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public ClientWalletService(ILogger<ClientWalletService> logger, IMyNoSqlServerDataWriter<ClientWalletNoSqlEntity> writer,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _writer = writer;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<ClientWalletList> GetWalletsByClient(JetClientIdentity clientId)
        {
            _logger.LogInformation("Request wallets for Broker/Brand/Client: {brokerId}/{brandId}/{clientId}",  
                clientId.BrokerId, clientId.BrandId, clientId.ClientId);


            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var list = await ctx.ClientWallet.Where(e => e.BrokerId == clientId.BrokerId && e.ClientId == clientId.ClientId)
                .ToListAsync();

            if (!list.Any())
            {
                var wallet = new ClientWallet()
                {
                    IsDefault = true,
                    Name = "spot",
                    WalletId = GenerateDefaultWalletId(clientId.ClientId)
                };

                var entity = new ClientWalletEntity(clientId.BrokerId, clientId.BrandId, clientId.ClientId, wallet);

                await ctx.UpsetAsync(new [] { entity });

                var noSqlEntity = new ClientWalletNoSqlEntity()
                {
                    BrokerId = entity.BrokerId,
                    BrandId = entity.BrandId,
                    ClientId = entity.ClientId,
                    WalletId = entity.WalletId,
                    PartitionKey = ClientWalletNoSqlEntity.GeneratePartitionKey(clientId.BrokerId, clientId.BrandId),
                    RowKey = ClientWalletNoSqlEntity.GenerateRowKey(clientId.ClientId),
                    Wallets = new List<ClientWallet>() {wallet}
                };

                await _writer.InsertOrReplaceAsync(noSqlEntity);

                list.Add(entity);
            }

            return new ClientWalletList()
            {
                Wallets = list.Cast<ClientWallet>().ToList()
            };
        }

        private string GenerateDefaultWalletId(string clientId)
        {
            return $"{Program.Settings.WalletPrefix}{clientId}";
        }
    }
}