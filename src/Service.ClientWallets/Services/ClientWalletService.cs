using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.Service;
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

            clientId.BrokerId.AddToActivityAsTag("brokerId");
            clientId.ClientId.AddToActivityAsTag("clientId");

            _logger.LogInformation("Request to get wallets. clientId: {clientText}", JsonSerializer.Serialize(clientId));

            using var activity = MyTelemetry.StartActivity($"Use DB context {DatabaseContext.Schema}")?.AddTag("db-schema", DatabaseContext.Schema);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var list = await ctx.ClientWallet.Where(e => e.BrokerId == clientId.BrokerId && e.ClientId == clientId.ClientId)
                .ToListAsync();

            if (!list.Any())
            {
                using var _ = MyTelemetry.StartActivity($"Create a new wallet");
                var wallet = new ClientWallet()
                {
                    IsDefault = true,
                    Name = "spot",
                    WalletId = GenerateDefaultWalletId(clientId.ClientId)
                };

                wallet.WalletId.AddToActivityAsTag("walletId");

                var entity = new ClientWalletEntity(clientId.BrokerId, clientId.BrandId, clientId.ClientId, wallet);

                await ctx.UpsetAsync(new [] { entity });

                list.Add(entity);

                _logger.LogInformation("Created default wallet. Wallet: {walletJson}", JsonSerializer.Serialize(entity));
            }

            await UpdateCache(clientId, list);

            return new ClientWalletList()
            {
                Wallets = list.Select(e => new ClientWallet(){IsDefault = e.IsDefault, Name = e.Name, WalletId = e.WalletId}).ToList()
            };
        }

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request)
        {
            using var _ = MyTelemetry.StartActivity($"Create a new wallet");
            request.Name.AddToActivityAsTag("wallet-name");
            request.ClientId.ClientId.AddToActivityAsTag("clientId");
            request.ClientId.BrokerId.AddToActivityAsTag("brokerId");
            request.ClientId.BrandId.AddToActivityAsTag("brandId");

            _logger.LogInformation("Request to create wallet. Request: {requestText}", JsonSerializer.Serialize(request));

            if (string.IsNullOrEmpty(request.ClientId?.ClientId) ||
                string.IsNullOrEmpty(request.ClientId?.BrandId) ||
                string.IsNullOrEmpty(request.ClientId?.BrokerId) ||
                string.IsNullOrEmpty(request.Name))
            {
                _logger.LogError("Cannot create wallet. BadRequest.");
                return new CreateWalletResponse()
                {
                    Success = false,
                    ErrorMessage = "Bad request"
                };
            }

            var index = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var wallet = new ClientWallet()
            {
                IsDefault = false,
                Name = request.Name,
                WalletId = $"{Program.Settings.WalletPrefix}{request.ClientId}-{index}",
            };

            wallet.WalletId.AddToActivityAsTag("walletId");

            var entity = new ClientWalletEntity(request.ClientId.BrokerId, request.ClientId.BrandId, request.ClientId.ClientId, wallet);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.ClientWallet.AddAsync(entity);
            await ctx.SaveChangesAsync();

            var list = await ctx.ClientWallet
                .Where(e => e.ClientId == request.ClientId.ClientId && e.BrokerId == request.ClientId.BrokerId)
                .ToListAsync();

            await UpdateCache(request.ClientId, list);

            _logger.LogInformation("Wallet created. Wallet: {walletJson}", JsonSerializer.Serialize(entity));

            return new CreateWalletResponse()
            {
                Success = true,
                Name = request.Name,
                WalletId = wallet.WalletId
            };
        }

        private string GenerateDefaultWalletId(string clientId)
        {
            return $"{Program.Settings.WalletPrefix}{clientId}";
        }

        private async Task UpdateCache(JetClientIdentity clientId, List<ClientWalletEntity> list)
        {
            var noSqlEntity = new ClientWalletNoSqlEntity()
            {
                BrokerId = clientId.BrokerId,
                BrandId = clientId.BrandId,
                ClientId = clientId.ClientId,
                PartitionKey = ClientWalletNoSqlEntity.GeneratePartitionKey(clientId.BrokerId, clientId.BrandId),
                RowKey = ClientWalletNoSqlEntity.GenerateRowKey(clientId.ClientId),
                Wallets = list.Select(e => new ClientWallet()
                {
                    Name = e.Name,
                    IsDefault = e.IsDefault,
                    WalletId = e.WalletId
                }).ToList()
            };

            await _writer.InsertOrReplaceAsync(noSqlEntity);
        }
    }
}