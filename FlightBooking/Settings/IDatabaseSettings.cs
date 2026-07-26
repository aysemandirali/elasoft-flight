namespace FlightBooking.Settings
{
    // MongoDB baglanti bilgilerini ve koleksiyon adlarini tutan sozlesme.
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string FlightCollectionName { get; set; }
        string BookingCollectionName { get; set; }
        string CheckInCollectionName { get; set; }
        string FlightDemandHistoryCollection { get; set; }
        string NoShowHistoryCollection { get; set; }
    }
}
