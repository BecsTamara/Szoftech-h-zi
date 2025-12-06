using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Model
{
    public class MertEgyseg
    {
        public string Alapertelmezett { get; set; }

        public MertEgyseg(string alapertelmezett)
        {
            Alapertelmezett = alapertelmezett;
        }
    }
}