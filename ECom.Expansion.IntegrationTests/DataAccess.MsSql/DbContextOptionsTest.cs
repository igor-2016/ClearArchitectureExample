using DataAccess.Interfaces;
using DataAccess.MsSql;
using ECom.Expansion.IntegrationTests.Services;
using ECom.Expansion.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ECom.Expansion.IntegrationTests.DataAccess.MsSql
{
    public class DbContextOptionsTest
    {
        //private static DbContextOptions<AppDbContext> _dbContextOptions;
        //private static IDataAccess _expansionSqlServerDataAccess;

        public static DbContextOptions<AppDbContext> GetDbContextOptions(bool useTestDb)
        {
            //if (_dbContextOptions == null)
            //{
                var config = ConfigHelper.InitConfiguration();
                var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(config.GetConnectionString(useTestDb ? "MsSql" : "MsSqlDev"))
                    .Options;
            //}
            return dbContextOptions;
        }

        public static IDataAccess GetSqlServerOptionsContext(bool useTestDb)
        {
            //if (_expansionSqlServerDataAccess == null)
            //{
                var expansionSqlServerDataAccess = new AppDbContext(GetDbContextOptions(useTestDb), MapperTest.GetMapper());
            //}
            return expansionSqlServerDataAccess;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetDbContextOptionsTest(bool useTestDb)
        {
            var context = GetDbContextOptions(useTestDb);
            Assert.IsType<DbContextOptions<AppDbContext>>(context);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetSqlServerOptionsContextTest(bool useTestDb)
        {
            var context = GetSqlServerOptionsContext(useTestDb);
            Assert.IsType<AppDbContext>(context);
        }
    }
}
