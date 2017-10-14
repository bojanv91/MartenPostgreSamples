using Dapper;
using Marten;
using Marten.Linq;
using Marten.Pagination;
using MartenPostgreSamples.Core.Orders;
using MartenPostgreSamples.Core.Products;
using MartenPostgreSamples.Core.Users;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MartenPostgreSamples.Tests
{
    public class code_samples_Tests : IClassFixture<MartenFixture>
    {
        IDocumentStore store;

        public code_samples_Tests(MartenFixture fixture)
        {
            store = fixture.Store;
        }

        [Fact]
        public void create_store_and_open_session()
        {
            // Connecting a document store to PostgreSQL database
            var documentStore = DocumentStore.For("host=localhost;database=marten_samples;username=postgres;password=postgres;");

            // Opening a session
            using (var session = documentStore.OpenSession())
            {
                // do operations in current transaction
            }
        }

        [Fact]
        public void projections()
        {
            using (var session = store.OpenSession())
            {
                var result = session.Query<Order>()
                    .Select(x => new
                    {
                        UserId = x.UserId,
                        TotalPrice = x.TotalPrice
                    });
            }
        }

        [Fact]
        public void persist_document()
        {
            using (var session = store.OpenSession())
            {
                var user = new User("thor");
                user.Address = new Address
                {
                    Country = "Asgard",
                    City = "Asgard City",
                    StreetName = "Anywhere 101"
                };

                // store object in session (which acts as unit of work)
                session.Store(user);

                // persist changes to database
                session.SaveChanges();
            }
        }

        [Fact]
        public void load_document_by_id()
        {
            using (var session = store.OpenSession())
            {
                // Persist dummy user
                var user = new User("Joe");
                session.Store(user);
                session.SaveChanges();

                // Load the dummy user again from database 
                var theUser = session.Load<User>(user.Id);

                // Assert
                theUser.Username.ShouldBe(user.Username);
            }
        }

        [Fact]
        public void querying_documents_with_linq()
        {
            using (var session = store.OpenSession())
            {
                // Basic querying
                var admins = session.Query<User>()
                    .Where(x => x.IsAdmin)
                    .ToList();

                // Filtering on nested objects
                var asgardians = session.Query<User>()
                    .Where(x => x.Address.City == "Asgard")
                    .ToList();

                //
                //

                // Paging and sorting
                var pagedResult = session.Query<User>()
                    .Where(x => x.IsAdmin)
                    .OrderBy(x => x.Username)
                    .AsPagedList(pageNumber: 2, pageSize: 10);
            }
        }

        [Fact]
        public void filtering_in_arrays()
        {
            using (var session = store.OpenSession())
            {
                var items = session.Query<Product>()
                    .Where(x => x.Tags.Contains("Electronics"))
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        [Fact]
        public void querying_with_SQL_1()
        {
            // Arrange
            using (var session = store.OpenSession())
            {
                var user1 = new User("abc") { Address = new Address { Country = "MKD" } };
                var user2 = new User("qwerty") { Address = new Address { Country = "MKD" } };
                var user3 = new User("thor") { Address = new Address { Country = "Asgard" } };
                session.StoreObjects(new object[] { user1, user2, user3 });
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                // Get all countries with total users registered
                var sql = @"
                SELECT 
                    data->'Address'->>'Country' AS Country,
                    COUNT(id)                   AS TotalUsers
                FROM mt_doc_user
                GROUP BY Country";

                var resultItems = session.Connection
                    .Query<QueryCountriesWithTotalUsersDto>(sql);
            }
        }

        [Fact]
        public void querying_with_SQL_2()
        {
            // Arrange
            using (var session = store.OpenSession())
            {
                var product1 = new Product("Keyboard", 20);
                var product2 = new Product("Mouse", 10);

                var user1 = new User("abc");
                var order1 = new Order(user1.Id);
                order1.AddItem(product1, 1);
                order1.AddItem(product2, 1);

                var user2 = new User("qwerty");
                var order2 = new Order(user2.Id);
                order2.AddItem(product2, 2);
                var order3 = new Order(user2.Id);
                order3.AddItem(product1, 1);
                order3.AddItem(product2, 1);

                session.StoreObjects(new object[] { product1, product2, user1, order1, user2, order2, order3 });
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                // Get all users and for each, get the total orders price
                var sql = @"
                SELECT 
                    u.id                                   AS UserId,
                    u.data->>'Username'                    AS Username,
                    u.data->'Address'->>'Country'          AS Country,
                    SUM((o.data->>'TotalPrice')::NUMERIC)  AS TotalOrdersPrice
                FROM mt_doc_user u
                INNER JOIN mt_doc_order o ON o.user_id = u.id
                GROUP BY UserId, Username, Country
                ORDER BY Username";

                var data = session.Connection.Query<QueryUsersWithTotalOrdersDto>(sql);

                // Assert
                data.Count().ShouldBe(2);
                data.ElementAt(0).Username.ShouldBe("abc");
                data.ElementAt(0).TotalOrdersPrice.ShouldBe(30);
                data.ElementAt(1).Username.ShouldBe("qwerty");
                data.ElementAt(1).TotalOrdersPrice.ShouldBe(50);
            }
        }
    }

    public class QueryCountriesWithTotalUsersDto
    {
        public string Country { get; set; }
        public long TotalUsers { get; set; }
    }

    public class QueryUsersWithTotalOrdersDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public decimal TotalOrdersPrice { get; set; }
    }
}