using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.ClientWallets.Domain.Models
{
    [DataContract]
    public class ClientWallet
    {
        [DataMember(Order = 1)]
        public string WalletId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Last activated wallet keeps as default wallet
        /// </summary>
        [DataMember(Order = 3)]
        public bool IsDefault { get; set; }
    }
}
