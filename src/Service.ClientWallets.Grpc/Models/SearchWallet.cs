using System.Runtime.Serialization;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class SearchWallet
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string BrandId { get; set; }
        [DataMember(Order = 3)] public string ClientId { get; set; }
        [DataMember(Order = 4)] public int Count { get; set; }
    }
}