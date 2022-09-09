using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymEShopAdminApplication.Models
{
    public class ProductInOrder
    {
        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }

        public Product SelectedProduct { get; set; }

        public Order UserOrder { get; set; }

        public int Quantity { get; set; }
    }
}
