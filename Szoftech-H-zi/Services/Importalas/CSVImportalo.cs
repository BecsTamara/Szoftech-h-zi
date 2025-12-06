using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class CsvImportalo : IImportalo
    {
        public string FajlNev => "CSV Importalo";

        public CsvImportalo() { }

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            var lista = new List<Adat>();
            int sikeres = 0;
            int hibas = 0;

            if (!File.Exists(fajl))
                throw new FileNotFoundException("A megadott CSV fájl nem található.", fajl);

            // A kiírás szerint a hibás sorok nem okozhatnak programleállást
            foreach (var sor in File.ReadAllLines(fajl).Skip(0))
            {
                var mezok = sor.Split(';');
                if (mezok.Length < 3)
                {
                    hibas++;
                    continue;
                }

                // Használjuk az InvariantCulture-t, hogy biztosítsuk a tizedes ponttal való beolvasást
                if (DateTime.TryParse(mezok[0], out DateTime ido) &&
                    double.TryParse(mezok[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double ertek))
                {
                    lista.Add(new Adat(ido, ertek, mezok[2], AdatEredet.Imported)
                    {
                        SzenzorNev = mezok.Length > 3 ? mezok[3] : null,
                        Kategoria = mezok.Length > 4 ? mezok[4] : null
                    });
                    sikeres++;
                }
                else
                {
                    hibas++;
                }
            }

            return (sikeres, hibas, lista);
        }
    }
}