namespace GLMS.ApiClient
{
    public interface IGlmsApiClient
    {
        //clients
        Task<IEnumerable<ClientDto>> GetClientsAsync();
        Task<ClientDto?> GetClientAsync(int id);
        Task<ClientDto?> CreateClientAsync(ClientWriteDto dto);
        Task<bool> UpdateClientAsync(int id, ClientWriteDto dto);
        Task<bool> DeleteClientAsync(int id);

        //contracts
        Task<IEnumerable<ContractDto>> GetContractsAsync(ContractFilterDto filter);
        Task<ContractDto?> GetContractAsync(int id);
        Task<ContractDto?> CreateContractAsync(CreateContractDto dto, IFormFile? signedAgreement);
        Task<bool> UpdateContractStatusAsync(int id, ContractStatus status);
        Task<bool> DeleteContractAsync(int id);

        //service requests
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync();
        Task<ServiceRequestDto?> GetServiceRequestAsync(int id);
        Task<ServiceRequestDto?> CreateServiceRequestAsync(CreateServiceRequestDto dto);
        Task<bool> DeleteServiceRequestAsync(int id);
        Task<ServiceRequestActionResultDto?> ExecuteActionAsync(int id, ServiceRequestActionDto action);
        Task<IEnumerable<CommandHistoryEntryDto>> GetCommandHistoryAsync();
    }
}