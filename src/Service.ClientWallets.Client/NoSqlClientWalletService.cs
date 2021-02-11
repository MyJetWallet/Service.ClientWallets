using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyNoSqlServer.DataReader;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.ClientWallets.MyNoSql;

namespace Service.ClientWallets.Client
{
    public class NoSqlClientWalletService : IClientWalletService
    {
        private readonly IClientWalletService _grpcService;
        private readonly MyNoSqlReadRepository<ClientWalletNoSqlEntity> _reader;

        public NoSqlClientWalletService(IClientWalletService grpcService, MyNoSqlReadRepository<ClientWalletNoSqlEntity> reader)
        {
            _grpcService = grpcService;
            _reader = reader;
        }

        public async Task<ClientWalletList> GetWalletsByClient(JetClientIdentity clientId)
        {
            var entity = _reader.Get(ClientWalletNoSqlEntity.GeneratePartitionKey(clientId.BrokerId, clientId.BrandId),
                ClientWalletNoSqlEntity.GenerateRowKey(clientId.ClientId));

            if (entity != null)
                return new ClientWalletList(){Wallets = entity.Wallets};

            var wallets = await _grpcService.GetWalletsByClient(clientId);

            return wallets;
        }
    }
}