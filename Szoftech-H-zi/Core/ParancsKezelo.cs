using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Core
{
    public class ParancsKezelo
    {
        public string BekerParancs()
        {
            Console.Write("Választás: ");
            return Console.ReadLine() ?? "";
        }

        public string BekerSzoveg(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine() ?? "";
        }

        public DateTime BekerDatum(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                if (DateTime.TryParse(Console.ReadLine(), out var d))
                    return d;

                Console.WriteLine("Érvénytelen dátum.");
            }
        }

        public DateTime? BekerDatumNull(string prompt)
        {
            Console.Write(prompt + " ");
            var bevitel = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(bevitel))
                return null;

            if (DateTime.TryParse(bevitel, out var d))
                return d;

            Console.WriteLine("Érvénytelen dátum, üresen hagyható.");
            return null;
        }

        public int BekerInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                if (int.TryParse(Console.ReadLine(), out var szam))
                    return szam;

                Console.WriteLine("Érvénytelen szám.");
            }
        }

        public double BekerDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                if (double.TryParse(Console.ReadLine(), out var szam))
                    return szam;

                Console.WriteLine("Érvénytelen szám.");
            }
        }

        public double? BekerDoubleNull(string prompt)
        {
            Console.Write(prompt + " ");
            var be = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(be))
                return null;

            if (double.TryParse(be, out var szam))
                return szam;

            Console.WriteLine("Érvénytelen szám, üresen hagyható.");
            return null;
        }
    }
}