using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.UserManager
{
    public class Admin : Felhasznalo
    {
        // Az öröklött konstruktort hívjuk, megadva a Role.Admin szerepkört
        public Admin(string nev)
            : base(nev, Role.Admin)
        {
        }
    }
}