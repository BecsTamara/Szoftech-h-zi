using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class CsvImportalo : IImportalo
    {
        public string FajlNev => "CSV Importalo";

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            var lista = new List<Adat>();
            int sikeres = 0;
            int hibas = 0;

            if (!File.Exists(fajl))
                throw new FileNotFoundException("A megadott CSV fájl nem található.", fajl);

            // Skip(1)-et használunk, feltételezve, hogy van fejléc.
            // Ha a fájljokban NINCS fejléc, írd vissza Skip(0)-ra!
            foreach (var sor in File.ReadAllLines(fajl).Skip(1))
            {
                // Üres sorok átugrása
                if (string.IsNullOrWhiteSpace(sor)) continue;

                var mezok = sor.Split(';');

                // Minimum 3 mező kell (Idő, Érték, Egység)
                if (mezok.Length < 3)
                {
                    hibas++;
                    continue;
                }

                // InvariantCulture a tizedespontos számokhoz (pl. 12.5)
                if (DateTime.TryParse(mezok[0], out DateTime ido) &&
                    double.TryParse(mezok[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double ertek))
                {
                    var adat = new Adat(ido, ertek, mezok[2], AdatEredet.Imported)
                    {
                        // Opcionális mezők kezelése (Szenzor és Kategória)
                        SzenzorNev = mezok.Length > 3 && !string.IsNullOrWhiteSpace(mezok[3]) ? mezok[3] : null,
                        Kategoria = mezok.Length > 4 && !string.IsNullOrWhiteSpace(mezok[4]) ? mezok[4] : null
                    };

                    lista.Add(adat);
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