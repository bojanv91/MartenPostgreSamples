using Marten;
using MartenPostgreSamples.Core.Orders;
using MartenPostgreSamples.Core.Products;
using MartenPostgreSamples.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Tests
{
    public class MartenFixture : IDisposable
    {
        public IDocumentStore Store { get; private set; }

        public MartenFixture()
        {
            Store = DocumentStore.For(_ =>
            {
                _.Connection("host=localhost;database=marten_samples;username=postgres;password=postgres;");

                // Custom schema mappings
                _.Schema.For<User>()
                    .Duplicate(x => x.Username);    // example with duplicating a field to a column

                _.Schema.For<Product>();

                _.Schema.For<Order>()
                    .ForeignKey<User>(x => x.UserId);   // adding foreign key to user
            });

            // Drop all marten tables
            Store.Advanced.Clean.CompletelyRemoveAll();

            // Create needed tables based on the schema mapping
            Store.Schema.ApplyAllConfiguredChangesToDatabase();
        }

        public void Dispose()
        {
            Store.Dispose();
        }
    }
}
