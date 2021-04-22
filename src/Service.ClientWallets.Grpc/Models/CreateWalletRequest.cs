using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class CreateWalletRequest
    {
        [DataMember(Order = 1)] public JetClientIdentity ClientId { get; set; }

        [DataMember(Order = 2)] public string Name { get; set; }
    }
}