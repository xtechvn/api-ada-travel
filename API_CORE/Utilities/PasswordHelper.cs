using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class PasswordHelper
    {
        public static string GenerateNumeric(int length)
        {
            var random = new Random();
            var chars = "0123456789";
            return new string(Enumerable
                .Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}

