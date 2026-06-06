using System.Net.Http.Json;

namespace GLMS.ApiClient
{
    public class GlmsApiClient : IGlmsApiClient
    {
        private readonly HttpClient _http;

        public GlmsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<ClientDto>> GetClientsAsync()
        {
            var clients = await _http.GetFromJsonAsync<IEnumerable<ClientDto>>("api/clients");
            return clients ?? Enumerable.Empty<ClientDto>();
        }

        public async Task<ClientDto?> GetClientAsync(int id)
        {
            var response = await _http.GetAsync($"api/clients/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task<ClientDto?> CreateClientAsync(ClientWriteDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/clients", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ClientDto>();
        }

        public async Task<bool> UpdateClientAsync(int id, ClientWriteDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/clients/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}