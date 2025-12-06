using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class ImportaloSzolgaltatas : IImportalo
    {
        public string FajlNev => "Importáló Szolgáltatás";

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            string extension = Path.GetExtension(fajl).ToLower();
            IImportalo importalo;

            if (extension == ".csv")
            {
                importalo = new CsvImportalo();
            }
            else if (extension == ".json")
            {
                importalo = new JSONImportalo();
            }
            else if (extension == ".xml")
            {
                importalo = new XMLImportalo();
            }
            else
            {
                throw new NotSupportedException($"Nem támogatott fájltípus: '{extension}'. Csak .csv, .json, .xml támogatott.");
            }

            return importalo.Importal(fajl);
        }
    }
}