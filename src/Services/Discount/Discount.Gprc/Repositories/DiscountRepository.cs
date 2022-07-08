using Dapper;
using Discount.Gprc.Entities;
using Npgsql;

namespace Discount.Gprc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public DiscountRepository(IConfiguration configuration, ILogger<Coupon> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            try
            {
                using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                var affeted = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName,Description,Amount) VALUES (@ProductName,@Description,@Amount)",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                if (affeted == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);

                return false;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            try
            {
                string connectionInfo = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
                _logger.LogInformation(connectionInfo);
                using var connection = new NpgsqlConnection(connectionInfo);

                var affeted = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName", new { ProductName = productName });

              

                if (affeted == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);

                return false;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName=@ProductName",new {ProductName=productName });

            if (coupon == null)
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affeted = await connection.ExecuteAsync("UPDATE Coupon (ProductName,Description,Amount) SET ProductName=@ProductName,Description=@Description,Amount=@Amount WHERE Id=@Id",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount,Id=coupon.Id });

            if (affeted == 0)
                return false;

            return true;
        }
    }
}
