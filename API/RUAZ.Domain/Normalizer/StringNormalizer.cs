using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RUAZ.Domain.Normalizer
{
    public static class StringNormalizer
    {
        public static string Normalize(string name)
        {
            return name.Trim().ToUpperInvariant();
        }
    }
}