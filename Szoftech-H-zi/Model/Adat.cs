using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Model
{
    public class Adat
    {
        public DateTime Ido { get; set; }
        public double Ertek { get; set; }
        public string MertEgyseg { get; set; }
        public AdatEredet Eredet { get; set; }
        public string? SzenzorNev { get; set; }
        public string? Kategoria { get; set; }

        public Adat(DateTime ido, double ertek, string mertEgyseg, AdatEredet eredet)
        {
            Ido = ido;
            Ertek = ertek;
            MertEgyseg = mertEgyseg;
            Eredet = eredet;
        }

        public override string ToString()
        {
            // 1. Magyarosítás
            string eredetStr = Eredet == AdatEredet.Imported ? "Importált" : "Generált";

            // 2. Opcionális adatok kezelése
            string szenzorStr = !string.IsNullOrWhiteSpace(SzenzorNev) ? $" | {SzenzorNev, -17}" : "";
            string kategoriaStr = !string.IsNullOrWhiteSpace(Kategoria) ? $" | {Kategoria}" : "";

            // 3. Formázás (Padding) az egységes vonalakért:
            // {Ertek,8:F2} -> 8 karakter széles helyet foglal, jobbra igazítva (szépen egymás alá kerülnek a tizedesvesszők)
            // {MertEgyseg,-3} -> 3 karakter széles, balra igazítva
            // {eredetStr,-8} -> 8 karakter széles, balra igazítva (így a következő | jel mindig ugyanoda esik)
            
            return $"{Ido:yyyy-MM-dd HH:mm:ss} | {Ertek,8:F2} {MertEgyseg,-3} | {eredetStr,-10}{szenzorStr}{kategoriaStr}";
        }
    }
}