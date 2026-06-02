using Xunit;

namespace GLMS.Tests.Services
{
    public class CurrencyMathTests
    {
        private static decimal ConvertUsdToZar(decimal usd, decimal rate) => usd * rate;

        [Theory]
        [InlineData(100, 16.50, 1650.00)]    //standard
        [InlineData(0, 16.50, 0)]            //zero
        [InlineData(1, 16.75, 16.75)]        //small
        [InlineData(50000, 20.00, 1000000)]  //large
        [InlineData(99.99, 18.00, 1799.82)]  //fractional
        public void Convert_KnownInputs_ReturnsExpectedZar(double usd, double rate, double expectedZar)
        {
            decimal usdAmount = (decimal)usd;
            decimal exchangeRate = (decimal)rate;
            decimal expected = (decimal)expectedZar;

            decimal result = ConvertUsdToZar(usdAmount, exchangeRate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_PreservesPrecisionWithDecimal()
        {
            decimal usd = 0.1m + 0.2m;
            decimal rate = 10m;

            decimal result = ConvertUsdToZar(usd, rate);

            Assert.Equal(3.0m, result);
        }
    }
}