using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Services.Statisztika
{
    public class StatisztikaElemzo : IStatisztikaElemzo
    {
        public double Min(IEnumerable<Adat> adatok) =>
            adatok.Min(a => a.Ertek);

        public double Max(IEnumerable<Adat> adatok) =>
            adatok.Max(a => a.Ertek);

        public double Atlag(IEnumerable<Adat> adatok) =>
            adatok.Average(a => a.Ertek);

        public int Darabszam(IEnumerable<Adat> adatok) =>
            adatok.Count();

        public Dictionary<DateTime, (double Min, double Max, double Atlag)> NapiStatisztika(IEnumerable<Adat> adatok)
        {
            return adatok
                .GroupBy(a => a.Ido.Date)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        g.Min(x => x.Ertek),
                        g.Max(x => x.Ertek),
                        g.Average(x => x.Ertek)
                    )
                );
        }
    }
}