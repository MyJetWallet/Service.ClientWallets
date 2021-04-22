using System;
using System.Collections.Generic;
using MyJetWallet.Domain;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.Domain.Models;

namespace Service.ClientWallets.MyNoSql
{
    public class ClientWalletNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-clietwallet-cache";

        public static string GeneratePartitionKey(string brokerId, string brandId) => $"{brokerId}::{brandId}";

        public static string GenerateRowKey(string clientId) => clientId;

        public string BrokerId { get; set; }
        public string BrandId { get; set; }
        public string ClientId { get; set; }

        public List<ClientWallet> Wallets { get; set; }

        public static ClientWalletNoSqlEntity Create(IJetClientIdentity clientId, List<ClientWallet> wallets)
        {
            return new ClientWalletNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(clientId.BrokerId, clientId.BrandId),
                RowKey = GenerateRowKey(clientId.ClientId),
                BrokerId = clientId.BrokerId,
                BrandId = clientId.BrandId,
                ClientId = clientId.ClientId,

                Wallets = wallets ?? new List<ClientWallet>()
            };
        }
    }
}
