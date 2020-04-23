using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.PayrexxIntegration.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [SqlCheck]
        public string SqlDbConnString { get; set; }
    }
}
