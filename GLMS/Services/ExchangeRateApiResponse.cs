using System.Text.Json.Serialization;

namespace GLMS.Services
{
    public class ExchangeRateApiResponse
    {
        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
        [JsonPropertyName("time_last_update-unix")]
        public long TimeLastUpdateUnix { get; set; }
    }
}
