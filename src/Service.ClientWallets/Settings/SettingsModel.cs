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
    }
}