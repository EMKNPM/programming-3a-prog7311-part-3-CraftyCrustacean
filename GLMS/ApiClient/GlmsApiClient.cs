using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

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

        public async Task<IEnumerable<ContractDto>> GetContractsAsync(ContractFilterDto filter)
        {
            var query = new List<string>();
            if (filter.StartDateFrom.HasValue)
                query.Add($"startDateFrom={filter.StartDateFrom.Value:yyyy-MM-dd}");
            if (filter.StartDateTo.HasValue)
                query.Add($"startDateTo={filter.StartDateTo.Value:yyyy-MM-dd}");
            if (filter.Status.HasValue)
                query.Add($"status={filter.Status.Value}");

            var url = "api/contracts";
            if (query.Count > 0) url += "?" + string.Join("&", query);

            var results = await _http.GetFromJsonAsync<IEnumerable<ContractDto>>(url);
            return results ?? Enumerable.Empty<ContractDto>();
        }

        public async Task<ContractDto?> GetContractAsync(int id)
        {
            var response = await _http.GetAsync($"api/contracts/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task<ContractDto?> CreateContractAsync(CreateContractDto dto, IFormFile? signedAgreement)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(dto.ClientId.ToString()), "ClientId");
            form.Add(new StringContent(dto.ContractType), "ContractType");
            form.Add(new StringContent(dto.StartDate.ToString("o")), "StartDate");
            form.Add(new StringContent(dto.EndDate.ToString("o")), "EndDate");
            form.Add(new StringContent(dto.ServiceLevel), "ServiceLevel");

            if (signedAgreement != null && signedAgreement.Length > 0)
            {
                var streamContent = new StreamContent(signedAgreement.OpenReadStream());
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(signedAgreement.ContentType);
                form.Add(streamContent, "signedAgreement", signedAgreement.FileName);
            }

            var response = await _http.PostAsync("api/contracts", form);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus status)
        {
            var response = await _http.PatchAsJsonAsync($"api/contracts/{id}/status", new { Status = status });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync()
        {
            var results = await _http.GetFromJsonAsync<IEnumerable<ServiceRequestDto>>("api/service-requests");
            return results ?? Enumerable.Empty<ServiceRequestDto>();
        }

        public async Task<ServiceRequestDto?> GetServiceRequestAsync(int id)
        {
            var response = await _http.GetAsync($"api/service-requests/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        }

        public async Task<ServiceRequestDto?> CreateServiceRequestAsync(CreateServiceRequestDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/service-requests", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/service-requests/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<ServiceRequestActionResultDto?> ExecuteActionAsync(int id, ServiceRequestActionDto action)
        {
            var response = await _http.PatchAsJsonAsync($"api/service-requests/{id}/status", action);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ServiceRequestActionResultDto>();
        }

        public async Task<IEnumerable<CommandHistoryEntryDto>> GetCommandHistoryAsync()
        {
            var results = await _http.GetFromJsonAsync<IEnumerable<CommandHistoryEntryDto>>("api/service-requests/history");
            return results ?? Enumerable.Empty<CommandHistoryEntryDto>();
        }
    }
}