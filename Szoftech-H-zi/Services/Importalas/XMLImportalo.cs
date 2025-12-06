using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class XMLImportalo : IImportalo
    {
        public string FajlNev => "XML Importalo";

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            var lista = new List<Adat>();
            int sikeres = 0,
            hibas = 0;

            if (!File.Exists(fajl))
                throw new FileNotFoundException("Az XML fájl nem található.", fajl);

            try
            {
                XDocument xdoc = XDocument.Load(fajl);
                // Feltételezve, hogy <Adatok> a gyökér és <Adat> vagy <Record> a sorok
                // A specifikáció nem ad XML példát, így általános struktúrát feltételezünk.
                
                foreach (var elem in xdoc.Descendants("Adat"))
                {
try
                    {
                        string idoStr = elem.Element("timestamp")?.Value;
                        string ertekStr = elem.Element("value")?.Value;
                        string egyseg = elem.Element("unit")?.Value;
                        string szenzor = elem.Element("sensor")?.Value;

                        if (DateTime.TryParse(idoStr, out DateTime ido) &&
                            double.TryParse(ertekStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double ertek) &&
                            !string.IsNullOrEmpty(egyseg))
                        {
                            var adat = new Adat(ido, ertek, egyseg, AdatEredet.Imported)
                            {
                                SzenzorNev = szenzor
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