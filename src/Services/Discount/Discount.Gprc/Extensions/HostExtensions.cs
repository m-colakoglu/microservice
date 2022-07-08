using Npgsql;

namespace Discount.Gprc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,int? retry=0)
        {
            int retryForAvailability = retry.Value;

            using (var scope=host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration=services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql databse. ");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = "DROP TABLE IF EXIST Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                              ProductName VARCHAR(24) NOT NULL,
                                                              Description TEXT,
                                                              Amount INT)";
                    command.ExecuteNonQuery();


                    connection.Close();
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, ex.Message);

                    if (retryForAvailability<30)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}
