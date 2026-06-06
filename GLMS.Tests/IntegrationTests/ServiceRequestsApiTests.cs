using GLMS.Api.Dtos;
using GLMS.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests.IntegrationTests
{
    public class ServiceRequestsApiTests : IClassFixture<ApiTestFactory>
    {
        private readonly HttpClient _client;

        public ServiceRequestsApiTests(ApiTestFactory factory)
        {
            _client = factory.CreateClient();
        }

        //create a client and active freight contract in memeory
        private async Task<int> SeedFreightContract()
        {
            //create client
            var client = await (await _client.PostAsJsonAsync("/api/clients", new ClientWriteDto
            {
                Name = "Test Client",
                ContactDetails = "[email protected]",
                Region = "South Africa"
            })).Content.ReadFromJsonAsync<ClientDto>();

            //create freight contract
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(client!.Id.ToString()), "ClientId");
            form.Add(new StringContent("Freight"), "ContractType");
            form.Add(new StringContent(DateTime.Today.ToString("o")), "StartDate");
            form.Add(new StringContent(DateTime.Today.AddMonths(6).ToString("o")), "EndDate");
            form.Add(new StringContent("Standard"), "ServiceLevel");

            var contractResponse = await _client.PostAsync("/api/contracts", form);
            var contract = await contractResponse.Content.ReadFromJsonAsync<ContractDto>();

            //make the contract active
            await _client.PatchAsJsonAsync(
                $"/api/contracts/{contract!.Id}/status",
                new UpdateContractStatusDto { Status = ContractStatus.Active });

            return contract.Id;
        }

        [Fact]
        public async Task Create_ValidRequest_OnActive_Works()
        {
            //test to see if creating a service request on an active contract succeeds
            int contractId = await SeedFreightContract();
            var dto = new CreateServiceRequestDto
            {
                ContractId = contractId,
                Discription = "Standard delivery",
                CostUSD = 500m,
                WeightTonnes = 20m
            };

            var response = await _client.PostAsJsonAsync("/api/service-requests", dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
            Assert.NotNull(created);
            Assert.Equal(ServiceRequestStatus.Pending, created!.Status);
            Assert.True(created.CostZAR > 0, "ZAR cost should be calculated");
        }

        [Fact]
        public async Task Approve_ValidRequest_Test()
        {
            //test to see if a valid contract will succesfully approve
            int contractId = await SeedFreightContract();
            var srResponse = await _client.PostAsJsonAsync("/api/service-requests", new CreateServiceRequestDto
            {
                ContractId = contractId,
                Discription = "Standard delivery",
                CostUSD = 500m,
                WeightTonnes = 20m
            });
            var sr = await srResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();

            var actionResponse = await _client.PatchAsJsonAsync(
                $"/api/service-requests/{sr!.Id}/status",
                new ServiceRequestActionDto { Action = "Approve" });

            actionResponse.EnsureSuccessStatusCode();
            var result = await actionResponse.Content.ReadFromJsonAsync<ServiceRequestActionResultDto>();
            Assert.True(result!.Success);
            Assert.Equal(ServiceRequestStatus.Approved, result.FinalStatus);
            Assert.NotNull(result.InvoiceNumber);
            Assert.StartsWith("INV-", result.InvoiceNumber);
        }

        [Fact]
        public async Task Approve_Fat_Fails()
        {
            //test that trying to approve an overwieght contract fails and doesnt change the status
            int contractId = await SeedFreightContract();
            var srResponse = await _client.PostAsJsonAsync("/api/service-requests", new CreateServiceRequestDto
            {
                ContractId = contractId,
                Discription = "Fat shipment",
                CostUSD = 500m,
                WeightTonnes = 50m
            });
            var sr = await srResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/service-requests/{sr!.Id}/status",
                new ServiceRequestActionDto { Action = "Approve" });

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceRequestActionResultDto>();
            Assert.False(result!.Success);
            Assert.Contains("exceeds maximum", result.Message);
            Assert.Equal(ServiceRequestStatus.Pending, result.FinalStatus);
        }

        [Fact]
        public async Task Reject_Request_Test()
        {
            //test that rejecting a service requests properly labels it as rejected
            int contractId = await SeedFreightContract();
            var srResponse = await _client.PostAsJsonAsync("/api/service-requests", new CreateServiceRequestDto
            {
                ContractId = contractId,
                Discription = "Bad Contract",
                CostUSD = 200m,
                WeightTonnes = 5m
            });
            var sr = await srResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/service-requests/{sr!.Id}/status",
                new ServiceRequestActionDto { Action = "Reject", Reason = "Stinky" });

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceRequestActionResultDto>();
            Assert.Equal(ServiceRequestStatus.Rejected, result!.FinalStatus);
        }

        [Fact]
        public async Task Create_On_NonrealContract()
        {
            //test that making a service request on a non exisitent contract returns 400
            var response = await _client.PostAsJsonAsync("/api/service-requests", new CreateServiceRequestDto
            {
                ContractId = 99999,
                Discription = "Danny",
                CostUSD = 100m
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_Fake_Status()
        {
            //Check that setting a service request status to something thats not real returns 400
            int contractId = await SeedFreightContract();
            var srResponse = await _client.PostAsJsonAsync("/api/service-requests", new CreateServiceRequestDto
            {
                ContractId = contractId,
                Discription = "test",
                CostUSD = 100m,
                WeightTonnes = 5m
            });
            var sr = await srResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/service-requests/{sr!.Id}/status",
                new ServiceRequestActionDto { Action = "Supercalifragilisticexpialidocious" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}