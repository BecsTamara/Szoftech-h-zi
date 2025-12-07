using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterologiaiAdatbazis.UI
{
    public class MenuRenderer
    {
        public void RenderMenu()
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("  METEROLÓGIAI ADATBÁZIS RENDSZER");
            Console.WriteLine("=====================================");
            Console.WriteLine();
            Console.WriteLine("1. Adatok importálása (.csv, .json, .xml)");
            Console.WriteLine("2. Adatok generálása");
            Console.WriteLine("3. Összes adat megtekintése");
            Console.WriteLine("4. Szűrés");
            Console.WriteLine("5. Statisztika megjelenítése");
            Console.WriteLine("6. Mértékegység módosítása (Admin)");
            Console.WriteLine("7. Összes adat törlése (Admin)");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("0. Kilépés");
            Console.WriteLine("-------------------------------------");
        }
    }
}