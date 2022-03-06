using dominikz.Endpoints;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Services
{
    public class DataContext
    {
        private readonly HttpClient _client;

        public DataContext(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> From<T>(Request<T> endpoint, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync(endpoint.Url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception(error);
            }

            var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var result = await JsonSerializer.DeserializeAsync<T>(contentStream, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, cancellationToken);

            return result;
        }

    }
}
