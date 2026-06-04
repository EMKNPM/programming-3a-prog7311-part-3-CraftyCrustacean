using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace GLMS.Services
{
    //Consumes Exchange Rate API (https://www.exchangerate-api.com) for currency conversions.
    //Uses their free, signup-less tier: open.er-api.com
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private const string ApiUrl = "https://open.er-api.com/v6/latest/USD";
        private const string CacheKey = "ExchangeRate_USD_ZAR";
        private const decimal FallbackRate = 16.50m;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CurrencyExchangeService> _logger;

        public CurrencyExchangeService(HttpClient http, IMemoryCache cache, ILogger<CurrencyExchangeService> logger)
        {
            _http = http;
            _cache = cache;
            _logger = logger;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            if (_cache.TryGetValue<decimal>(CacheKey, out var cachedRate))
            {
                _logger.LogDebug("Using cached USD to ZAR rate: {Rate}", cachedRate);
                return cachedRate;
            }

            try
            {
                var response = await _http.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<ExchangeRateApiResponse>(json);

                if (parsed?.Rates != null && parsed.Rates.TryGetValue("ZAR", out var rate) && rate > 0)
                {
                    _logger.LogInformation("Fetched USD to Zar rate from API: {Rate}", rate);
                    _cache.Set(CacheKey, rate, CacheDuration);
                    return rate;
                }

                _logger.LogWarning("API response did not contain a valid ZAR rate, using fall back rate of 16.50");
                return FallbackRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch exchange rate, using fallback rate of 16.50");
                return FallbackRate;
            }
        }

    }
}
