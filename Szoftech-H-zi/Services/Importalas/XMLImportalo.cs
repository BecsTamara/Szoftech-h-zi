using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class XMLImportalo : IImportalo
    {
        public string FajlNev => "XML Importalo";

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            var lista = new List<Adat>();
            int sikeres = 0;
            int hibas = 0;

            if (!File.Exists(fajl))
                throw new FileNotFoundException("Az XML fájl nem található.", fajl);

            try
            {
                XDocument xdoc = XDocument.Load(fajl);

                foreach (var elem in xdoc.Descendants("Adat"))
                {
                    try
                    {
                        string? idoStr = elem.Element("timestamp")?.Value;
                        string? ertekStr = elem.Element("value")?.Value;
                        string? egyseg = elem.Element("unit")?.Value;
                        string? szenzor = elem.Element("sensor")?.Value;

                        // Kategória beolvasása (ha létezik)
                        string? kategoria = elem.Element("category")?.Value;

                        if (DateTime.TryParse(idoStr, out DateTime ido) &&
                            double.TryParse(ertekStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double ertek) &&
                            !string.IsNullOrEmpty(egyseg))
                        {
                            var adat = new Adat(ido, ertek, egyseg, AdatEredet.Imported)
                            {
                                SzenzorNev = szenzor,
                                Kategoria = kategoria // Itt adjuk hozzá
                            };
                            lista.Add(adat);
                            sikeres++;
                        }
                        else
                        {
                            hibas++;
                        }
                    }
                    catch
                    {
                        hibas++;
                    }
                }
            }
            catch
            {
                // XML formátum hiba
                hibas++;
            }

            return (sikeres, hibas, lista);
        }
    }
}