using Chat.HttpClients.Interface;
using System.Text.Json;

namespace Chat.HttpClients
{
    public class ExternalApi : IExternalApi
    {
        private readonly HttpClient _httpClient;

        public ExternalApi(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetContextAsync(string query)
        {
            var response = await _httpClient.GetAsync($"http://127.0.0.1:8000/search?query={query}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching data: {responseContent}");
            }

            using var jsonDoc = JsonDocument.Parse(responseContent);
            return jsonDoc.RootElement.GetProperty("context").GetString();
        }
    }
}
