using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface IStatisztikaElemzo
    {
        double Min(IEnumerable<Adat> adatok);
        double Max(IEnumerable<Adat> adatok);
        double Atlag(IEnumerable<Adat> adatok);
        int Darabszam(IEnumerable<Adat> adatok);

        /// <summary>
        /// Napi bontású statisztikák:
        /// Key = nap dátuma
        /// Value = (min, max, átlag)
        /// </summary>
        Dictionary<DateTime, (double Min, double Max, double Atlag)> NapiStatisztika(IEnumerable<Adat> adatok);
    }
}