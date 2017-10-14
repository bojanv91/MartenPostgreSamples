using MartenPostgreSamples.Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core.Orders
{
    public class Order : RootEntity
    {
        readonly List<OrderItem> _items = new List<OrderItem>();

        public Order(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; private set; }
        public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
        public CouponInfo Coupon { get; private set; }

        public decimal TotalPriceWithoutDiscount => _items.Sum(x => x.TotalPrice);
        public decimal TotalDiscount => TotalPriceWithoutDiscount * (Coupon?.DiscountPercentage ?? 0m);
        public decimal TotalPrice => TotalPriceWithoutDiscount - TotalDiscount;
        public IEnumerable<OrderItem> Items => _items;

        public OrderItem AddItem(Product product, int quantity = 1)
        {
            var item = OrderItem.For(product, quantity);
            _items.Add(item);
            return item;
        }

        public void RemoveItem(Guid orderItemId)
        {
            var item = _items.FirstOrDefault(x => x.Id == orderItemId);
            if (item == null) return;
            _items.Remove(item);
        }
        public void RemoveItem(OrderItem item)
        {
            _items.Remove(item);
        }

        public void ApplyCoupon(CouponInfo coupon)
        {
            Coupon = coupon;
        }
    }
}