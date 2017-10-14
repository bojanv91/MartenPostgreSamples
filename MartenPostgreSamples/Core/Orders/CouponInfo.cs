using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core.Orders
{
    public class CouponInfo
    {
        public CouponInfo(decimal discount, string code)
        {
            DiscountPercentage = discount;
            Code = code;
        }

        public decimal DiscountPercentage { get; private set; }
        public string Code { get; private set; }
    }
}
