using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.UI
{
    public class Megjelenito
    {
        private readonly MenuRenderer _menuRenderer;

        public Megjelenito(MenuRenderer menuRenderer)
        {
            _menuRenderer = menuRenderer;
        }

        public void Menu()
        {
            _menuRenderer.RenderMenu();
        }

        public void Szoveg(string uzenet)
        {
            Console.WriteLine(uzenet);
        }

        public void Siker(string uzenet)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(uzenet);
            Console.ResetColor();
        }

        public void Hiba(string uzenet)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(uzenet);
            Console.ResetColor();
        }

        public void MegjelenitListat(IEnumerable<Adat> lista)
        {
            if (!lista.Any())
            {
                Console.WriteLine("Nincs megjeleníthető adat.");
            }
                        foreach (var adat in lista)
            {
                Console.WriteLine(adat.ToString());
            }
        }

        public void MegjelenitNapiStat(Dictionary<DateTime, (double Min, double Max, double Atlag)> stat)
        {
            Console.WriteLine("\n--- Napi statisztikák ---");

            foreach (var nap in stat.OrderBy(n => n.Key))
            {
                Console.WriteLine(
                    $"{nap.Key:yyyy-MM-dd} | Min: {nap.Value.Min:F2}, Max: {nap.Value.Max:F2}, Átlag: {nap.Value.Atlag:F2}"
                );
            }
        }
    }
}