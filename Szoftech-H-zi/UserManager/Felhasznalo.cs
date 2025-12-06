using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.UserManager
{
    public class Felhasznalo
    {
        public string Nev { get; }
        public Role Szerep { get; }

        public Felhasznalo(string nev, Role szerep = Role.Felhasznalo) // Alapértelmezett szerepkör
        {
            Nev = nev;
            Szerep = szerep;
        }

        public override string ToString()
        {
            return $"{Nev} ({Szerep})";
        }
    }
}