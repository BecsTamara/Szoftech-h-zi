using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.Model
{
    public class AdatHalmaz
    {
        public List<Adat> Adatok { get; private set; } = new List<Adat>();

        public bool Ures => !Adatok.Any();

        public void HozzaadTobb(IEnumerable<Adat> ujAdatok)
        {
            // Null check: csak akkor adjuk hozzá, ha létezik a lista és van benne elem
            if (ujAdatok != null && ujAdatok.Any())
            {
                Adatok.AddRange(ujAdatok);
            }
        }
        public void TorolMindent()
        {
            Adatok.Clear();
        }
    }
}