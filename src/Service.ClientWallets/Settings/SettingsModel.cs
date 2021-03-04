using SimpleTrading.SettingsReader;

namespace Service.ClientWallets.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("ClientWallets.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("ClientWallets.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("ClientWallets.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("ClientWallets.WalletPrefix")]
        public string WalletPrefix { get; set; }
    }
}