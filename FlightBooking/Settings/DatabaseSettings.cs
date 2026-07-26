namespace FlightBooking.Settings
{
    // appsettings.json icindeki "DatabaseSettingsKey" bolumu bu sinifa baglanir.
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string FlightCollectionName { get; set; } = string.Empty;
        public string BookingCollectionName { get; set; } = string.Empty;
        public string CheckInCollectionName { get; set; } = string.Empty;
        public string FlightDemandHistoryCollection { get; set; } = string.Empty;
        public string NoShowHistoryCollection { get; set; } = string.Empty;
    }
}
