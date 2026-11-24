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
        public decimal Price { get; private set; }
        public byte[] Image { get; set; } = [];
        public Guid OwnerId { get; set; }
        public int Stock { get; private set; }
        public ProductsCategory Category { get; set; }
        public StatusProduct Status { get; private set; }

        public void SetPrice(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "O preço do produto não pode ser negativo.");
            }

            Price = price;
        }

        public void SetStock(int stock)
        {
            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock), "O estoque do produto não pode ser negativo.");
            }

            Stock = stock;
            Status = Stock > 0 ? StatusProduct.Available : StatusProduct.Exhausted;
        }
    }
}