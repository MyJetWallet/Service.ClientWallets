using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class SearchWalletsResponse
    {
        [DataMember(Order = 1)] public List<SearchWallet> Clients { get; set; }
    }
}