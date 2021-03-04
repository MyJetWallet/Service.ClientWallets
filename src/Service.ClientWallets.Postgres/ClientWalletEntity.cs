using Service.ClientWallets.Domain.Models;

namespace Service.ClientWallets.Postgres
{
    public class ClientWalletEntity : ClientWallet
    {
        public ClientWalletEntity()
        {
        }

        public ClientWalletEntity(string brokerId, string brandId, string clientId, ClientWallet wallet)
        {
            BrokerId = brokerId;
            BrandId = brandId;
            ClientId = clientId;

            WalletId = wallet.WalletId;
            IsDefault = wallet.IsDefault;
            Name = wallet.Name;
        }


        public string BrokerId { get; set; }
        public string BrandId { get; set; }
        public string ClientId { get; set; }
    }
}