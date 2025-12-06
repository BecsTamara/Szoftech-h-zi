using MeterologiaiAdatbazis.Core;
using MeterologiaiAdatbazis.Model;
using MeterologiaiAdatbazis.Services.Adatforras;
using MeterologiaiAdatbazis.Services.Importalas;
using MeterologiaiAdatbazis.Services.Mertekegyseg;
using MeterologiaiAdatbazis.Services.Statisztika;
using MeterologiaiAdatbazis.Services.Szures;
using MeterologiaiAdatbazis.UI;
using MeterologiaiAdatbazis.UserManager;
using MeterologiaiAdatbazis.Services.Generalas;
using System;

namespace MeterologiaiAdatbazis
{
    public class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Szerepkör választás
            Role szerep = FelhasznaloBejelentkezes();

            Felhasznalo aktFelhasznalo = (szerep == Role.Admin)
                ? new Admin("Adminisztrátor")
                : new Felhasznalo("Felhasználó");

            // Függőségek inicializálása
            var adatforras = new MemoriaAdatforras();
            var importalo = new ImportaloSzolgaltatas();
            var generalo = new Generalo();
            var szuro = new Szuro();
            var stat = new StatisztikaElemzo();
            var mertekKezelo = new MertekegysegKezelo("°C");
            var jogosultsag = new JogosultsagKezelo();
            var parancsKezelo = new ParancsKezelo();

            var menu = new MenuRenderer();
            var megjelenito = new Megjelenito(menu);

            // MÓDOSÍTVA: Alkalmazás Vezérlő példányosítása (10 paraméter)
            var vezerlo = new AlkalmazasVezerlo(
                adatforras,
                importalo,
                generalo,
                szuro,
                stat,
                mertekKezelo,
                jogosultsag,
                megjelenito,
                parancsKezelo,
                aktFelhasznalo
            );

            // Fő program ciklus indítása
            vezerlo.Futtat();

            Console.WriteLine("A program kilép.");
        }

        private static Role FelhasznaloBejelentkezes()
        {
            Console.WriteLine(new string('=', 37));
            Console.WriteLine(new string(' ', 8) + "SZEREPKÖR VÁLASZTÁS");
            Console.WriteLine(new string('=', 37));
            Console.WriteLine("1. Felhasználó");
            Console.WriteLine("2. Admin");
            Console.Write("Választás: ");

            var valasz = Console.ReadLine();
            return valasz == "2" ? Role.Admin : Role.Felhasznalo;
        }
    }
}