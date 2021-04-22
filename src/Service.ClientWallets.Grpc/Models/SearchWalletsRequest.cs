using System.Runtime.Serialization;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class SearchWalletsRequest
    {
        [DataMember(Order = 1)] public string SearchText { get; set; }
        [DataMember(Order = 2)] public int Take { get; set; }
    }
}