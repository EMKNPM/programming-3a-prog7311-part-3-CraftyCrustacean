namespace GLMS.ApiClient
{

    public interface IGlmsApiClient
    {
        // Clients
        Task<IEnumerable<ClientDto>> GetClientsAsync();
        Task<ClientDto?> GetClientAsync(int id);
        Task<ClientDto?> CreateClientAsync(ClientWriteDto dto);
        Task<bool> UpdateClientAsync(int id, ClientWriteDto dto);
        Task<bool> DeleteClientAsync(int id);
    }
}