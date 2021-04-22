using System.Runtime.Serialization;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class CreateWalletResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string WalletId { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }

        [DataMember(Order = 4)] public string ErrorMessage { get; set; }
    }
}