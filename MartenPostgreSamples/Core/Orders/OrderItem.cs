using MartenPostgreSamples.Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core.Orders
{
    public class OrderItem : Entity
    {
        private OrderItem()
        {
        }

        internal static OrderItem For(Product product, int quantity = 1)
        {
            var item = new OrderItem
            {
                ProductId = product.Id,
                Name = product.Name,
                BasePrice = product.Price,
                Quantity = quantity >= 1 ? quantity : 1
            };
            return item;
        }

        public string Name { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal BasePrice { get; private set; }
        public int Quantity { get; private set; }

        public decimal TotalPrice
            => BasePrice * Quantity;

        public void IncreaseQuantity()
            => Quantity++;

        public void DecreaseQuantity()
        {
            var canDecrease = Quantity > 1;
            if (canDecrease)
                Quantity--;
        }
    }
}
