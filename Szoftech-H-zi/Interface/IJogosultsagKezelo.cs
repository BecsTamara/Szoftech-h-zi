using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Interface
{
    public interface IJogosultsagKezelo
    {
        bool Ellenoriz(Role aktualisSzerep, Role szuksegesSzerep);
        void Kovetel(Role aktualisSzerep, Role szuksegesSzerep);
    }
}