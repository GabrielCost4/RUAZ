using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Domain.Base;
using RUAZ.Domain.Enums;


namespace RUAZ.Domain.Entities
{
    public class Product : EntityBase
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public byte[] Image { get; set; } = [];
        public Guid OwnerId { get; set; }
        public int Stock { get; set; }
        public ProductsCategory Category { get; set; }
        public StatusProduct Status { get; set; }

        public void SyncStatusFromStock()
        {
            Status = Stock > 0 ? StatusProduct.Available : StatusProduct.Exhausted;
        }
    }
}