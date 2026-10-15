using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Domain.Base;

namespace RUAZ.Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } 
        public List<Product> ProductsOfSell { get; set; } = [];
        public byte[]? Avatar { get; set; } =  null;
        
    }
}