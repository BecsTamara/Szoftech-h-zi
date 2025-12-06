using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Services.Generalas
{
    public class Generalo : IGeneralo
    {
        private readonly Random _random = new();

        // Lehetséges kategóriák, amiből válogat a random generáló
        private readonly string[] _kategoriak = new[] 
        { 
            "Általános", 
            "Kültér", 
            "Beltér", 
            "Labor", 
            "Szerverszoba", 
            "Üvegház",
            "Mobil egység"
        };

        public List<Adat> General(int darab, DateTime kezdoIdo, DateTime vegIdo, double minErtek, double maxErtek, string mertekegyseg)
        {
            var generaltLista = new List<Adat>();
            // Időintervallum teljes hossza másodpercben
            double teljesIntervallumSec = (vegIdo - kezdoIdo).TotalSeconds;
            if (teljesIntervallumSec < 0) teljesIntervallumSec = 0;

            for (int i = 0; i < darab; i++)
            {
                // Véletlen időpont generálása az intervallumon belül
                double randomSec = _random.NextDouble() * teljesIntervallumSec;
                DateTime randomIdo = kezdoIdo.AddSeconds(randomSec);

                // Véletlen érték generálása a tartományban
                double ertek = minErtek + (_random.NextDouble() * (maxErtek - minErtek));
                string randomKategoria = _kategoriak[_random.Next(_kategoriak.Length)];
                var adat = new Adat(randomIdo, ertek, mertekegyseg, AdatEredet.Generated)
                {
                    // Opcionális: véletlenszerű extra adatok
                    SzenzorNev = $"Gen_Szenzor_{_random.Next(1, 10)}",
                    Kategoria = randomKategoria
                };

                generaltLista.Add(adat);
            }

            return generaltLista;
        }
    }
}