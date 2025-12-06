using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface IImportalo
    {
        string FajlNev { get; }

        (int sikeres, int hibas, List<Adat> lista) Importal(string fajl);
    }
}