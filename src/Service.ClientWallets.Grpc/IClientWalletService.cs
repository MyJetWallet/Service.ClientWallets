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
    }

    public class CreateWalletRequest
    {
        public JetClientIdentity ClientId { get; set; }

        public string Name { get; set; }
    }

    public class CreateWalletResponse
    {
        public bool Success { get; set; }
        public string WalletId { get; set; }
        public string Name { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}