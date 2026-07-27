namespace FlightBooking.AgentServices
{
    public interface IGeminiService
    {
        // Varsayilan seyahat asistani promptuyla soru sorar.
        Task<string> AskAsync(string userMessage);

        // Ozel bir sistem talimati ile ham cagri yapar (agent adimlari icin).
        Task<string> GenerateAsync(string systemInstruction, string userMessage);
    }
}
