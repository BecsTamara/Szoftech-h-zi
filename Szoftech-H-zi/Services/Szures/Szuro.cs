using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Services.Szures
{
    public class Szuro : ISzuro
    {
        public IEnumerable<Adat> SzuresErtekAlapjan(IEnumerable<Adat> adatok, double? minimum, double? maximum)
        {
            return adatok.Where(a =>
                (minimum == null || a.Ertek >= minimum) &&
                (maximum == null || a.Ertek <= maximum));
        }

        public IEnumerable<Adat> Szures(
            IEnumerable<Adat> adatok,
            DateTime? tol,
            DateTime? ig,
            double? min,
            double? max)
        {
            var list = adatok;

            if (tol != null)
                list = list.Where(a => a.Ido >= tol.Value);

            if (ig != null)
                list = list.Where(a => a.Ido <= ig.Value);

            list = SzuresErtekAlapjan(list, min, max);

            return list.ToList();
        }
    }
}