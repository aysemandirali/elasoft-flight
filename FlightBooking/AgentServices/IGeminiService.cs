namespace FlightBooking.AgentServices
{
    public interface IGeminiService
    {
        // Kullanicinin sorusunu Gemini'ye gonderir, cevabi metin olarak dondurur.
        Task<string> AskAsync(string userMessage);
    }
}
