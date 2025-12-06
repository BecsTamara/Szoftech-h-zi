using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.UserManager
{
    public class JogosultsagKezelo : IJogosultsagKezelo
    {
        public bool Ellenoriz(Role szerep, Role szukseges)
        {
            // Admin jogosultság mindig elegendő, vagy ha a szerep megegyezik a szükséges szereppel
            return szerep == Role.Admin || szerep == szukseges;
        }

        public void Kovetel(Role szerep, Role szukseges)
        {
            if (!Ellenoriz(szerep, szukseges))
                throw new UnauthorizedAccessException(
                    $"Ehhez a művelethez {szukseges} jogosultság szükséges, de az aktuális szerepkör: {szerep}.");
        }
    }
}