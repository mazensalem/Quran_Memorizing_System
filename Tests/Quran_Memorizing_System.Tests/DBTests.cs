using Microsoft.Extensions.Configuration;
using Xunit;
using Quran_Memorizing_System.Models;
using System.Collections.Generic;

namespace Quran_Memorizing_System.Tests
{
    public class DBTests
    {
        [Fact]
        public void DB_UsesDefaultConnection_FromConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
                { "ConnectionStrings:DefaultConnection", "Data Source=TestServer;Initial Catalog=TestDb;Integrated Security=True;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var db = new DB(configuration);

            Assert.Contains("TestServer", db.GetConnectionStringForTests());
        }

        [Fact]
        public void DB_FallsBackTo_Hardcoded_WhenNoConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();
            var db = new DB(configuration);
            var cs = db.GetConnectionStringForTests();
            Assert.Contains("Elabd", cs);
        }
    }
}
