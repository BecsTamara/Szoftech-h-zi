using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Services.Mertekegyseg
{
    public class Mertekegyseg
    {
        public string Alapertelmezett { get; set; } = "°C";

        public Mertekegyseg() { }

        public Mertekegyseg(string alapertelmezett)
        {
            Alapertelmezett = alapertelmezett;
        }
    }

    public class MertekegysegKezelo
    {
        public Mertekegyseg Mertekegyseg { get; set; } = new Mertekegyseg();

        public MertekegysegKezelo(string alapertelmezett)
        {
            Mertekegyseg.Alapertelmezett = alapertelmezett;
        }

        /// <summary>
        /// Módosítja az alapértelmezett mértékegységet, és visszaad egy konverziós szabálygyűjteményt
        /// az összes ismert egységről az új célmértékegységre.
        /// </summary>
        public Dictionary<string, Func<double, double>> Modosit(string uj)
        {
            string regi = Mertekegyseg.Alapertelmezett;

            if (string.Equals(regi, uj, StringComparison.OrdinalIgnoreCase))
            {
                Mertekegyseg.Alapertelmezett = uj;
                return new Dictionary<string, Func<double, double>>(); 
            }

            var konverziok = KonverziokLetrehozasa(uj);

            if (!konverziok.Any())
            {
                throw new InvalidOperationException($"Nincs definiálva konverziós szabály a célmértékegységre ({uj}).");
            }

            Mertekegyseg.Alapertelmezett = uj;
            return konverziok;
        }

        private Dictionary<string, Func<double, double>> KonverziokLetrehozasa(string celEgyseg)
        {
            var konverziok = new Dictionary<string, Func<double, double>>(StringComparer.OrdinalIgnoreCase);

            // --- HŐMÉRSÉKLET KONVERZIÓK ---

            // Ha a cél: Celsius (°C)
            if (celEgyseg == "°C")
            {
                konverziok.Add("°F", (f) => (f - 32) * 5 / 9); // F -> C
                konverziok.Add("K", (k) => k - 273.15); // K -> C
            }
            // Ha a cél: Fahrenheit (°F)
            else if (celEgyseg == "°F")
            {
                konverziok.Add("°C", (c) => (c * 9 / 5) + 32); // C -> F
                konverziok.Add("K", (k) => (k - 273.15) * 9 / 5 + 32); // K -> F
            }
            // Ha a cél: Kelvin (K)
            else if (celEgyseg == "K")
            {
                konverziok.Add("°C", (c) => c + 273.15); // C -> K
                konverziok.Add("°F", (f) => (f - 32) * 5 / 9 + 273.15); // F -> K
            }

            return konverziok;
        }
    }
}