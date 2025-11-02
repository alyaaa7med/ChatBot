using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace ChatBot.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIService(IConfiguration config)
        {
            _apiKey = config["OpenAI:Key"];
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetAIResponseAsync(string userMessage)
        {
        
            var requestBody = new
            {
                model = "gpt-4o-mini", // or gpt-4o
                messages = new[]
                {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = userMessage }
        },
                max_tokens = 1000
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);

            // ✅ Check for API error
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var errorMessage = error.GetProperty("message").GetString();
                return $"⚠️ API Error: {errorMessage}";
            }

            // ✅ Check for "choices"
            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                return "⚠️ No response from AI. Please try again.";
            }

            return choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                .Trim();


        } 
    }
}


