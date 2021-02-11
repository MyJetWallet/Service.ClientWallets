using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.ClientWallets.Domain.Models;

namespace Service.ClientWallets.Grpc.Models
{
    [DataContract]
    public class ClientWalletList
    {
        [DataMember(Order = 1)]
        public List<ClientWallet> Wallets { get; set; }
    }
}