using GLMS.Api.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests.IntegrationTests
{
    public class ClientsApiTests : IClassFixture<ApiTestFactory>
    {
        private readonly HttpClient _client;

        public ClientsApiTests(ApiTestFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsListOfClients()
        {
            //test that fetching client list returns something
            var response = await _client.GetAsync("/api/clients");

            response.EnsureSuccessStatusCode();
            var clients = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
            Assert.NotNull(clients);
        }

        [Fact]
        public async Task Create_ValidClient_Works()
        {
            //test that creating a client works and auto generates an id
            var dto = new ClientWriteDto
            {
                Name = "Cyber Acme",
                ContactDetails = "[email protected]",
                Region = "South Africa"
            };

            var response = await _client.PostAsJsonAsync("/api/clients", dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<ClientDto>();
            Assert.NotNull(created);
            Assert.True(created!.Id > 0);
            Assert.Equal("Cyber Acme", created.Name);
        }

        [Fact]
        public async Task Create_NoName()
        {
            //test that if the name is missing it throws 400
            var dto = new ClientWriteDto
            {
                Name = "",
                ContactDetails = "[email protected]",
                Region = "South Africa"
            };

            var response = await _client.PostAsJsonAsync("/api/clients", dto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task NonexistentId_Returns404()
        {
            //test that fetching an id that doesnt exist returns a 404 not found error
            var response = await _client.GetAsync("/api/clients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Everything_Works()
        {
            //test to see if create delete and edit all work

            //test create
            var createDto = new ClientWriteDto
            {
                Name = "Test Client",
                ContactDetails = "[email protected]",
                Region = "South Africa"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/clients", createDto);
            var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();
            Assert.NotNull(created);

            //test update
            var updateDto = new ClientWriteDto
            {
                Name = "Updated Client",
                ContactDetails = "[email protected]",
                Region = "South Africa"
            };
            var updateResponse = await _client.PutAsJsonAsync($"/api/clients/{created!.Id}", updateDto);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/clients/{created.Id}");
            var fetched = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
            Assert.Equal("Updated Client", fetched!.Name);

            //test delete
            var deleteResponse = await _client.DeleteAsync($"/api/clients/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var verifyResponse = await _client.GetAsync($"/api/clients/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        }
    }
}