namespace GLMS.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> GetUsdToZarRateAsync();
    }
}
