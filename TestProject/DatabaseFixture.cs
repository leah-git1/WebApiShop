using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class DatabaseFixture : IDisposable
    {
        public ShopContext Context { get; private set; }

        public DatabaseFixture()
        {

            // Set up the test database connection and initialize the context
            var options = new DbContextOptionsBuilder<ShopContext>()

                .UseSqlServer("Server=srv2\\pupils;Database=TestDataBase;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;
            Context = new ShopContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            // Clean up the test database after all tests are completed
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}