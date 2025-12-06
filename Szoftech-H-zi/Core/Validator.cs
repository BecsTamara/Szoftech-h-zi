using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Core
{
    public class Validator
    {
        public static void NemUres(string szoveg, string uzenet)
        {
            if (string.IsNullOrWhiteSpace(szoveg))
                throw new ArgumentException(uzenet);
        }

        public static void NullEllenorzes(object? obj, string uzenet)
        {
            if (obj == null)
                throw new ArgumentNullException(uzenet);
        }
    }
}