using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using Service.ClientWallets.Grpc.Models;

namespace Service.ClientWallets.Grpc
{
    [ServiceContract]
    public interface IClientWalletService
    {
        [OperationContract]
        Task<ClientWalletList> GetWalletsByClient(JetClientIdentity clientId);

        [OperationContract]
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);

        [OperationContract]
        Task<SearchWalletsResponse> SearchClientsAsync(SearchWalletsRequest request);
    }
}