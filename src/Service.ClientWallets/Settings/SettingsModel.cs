using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.ClientWallets.Settings
{
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

        [YamlProperty("ClientWallets.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("ClientWallets.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}