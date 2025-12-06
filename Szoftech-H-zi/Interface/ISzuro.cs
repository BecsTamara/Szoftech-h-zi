using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface ISzuro
    {
        IEnumerable<Adat> Szures(IEnumerable<Adat> adatok,
            DateTime? tol, DateTime? ig, double? min, double? max);
    }
}