using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using MeterologiaiAdatbazis.Services.Mertekegyseg;
using MeterologiaiAdatbazis.UI;
using MeterologiaiAdatbazis.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeterologiaiAdatbazis.Core
{
    public class AlkalmazasVezerlo
    {
        public readonly IAdatforras _adatforras;
        public readonly IImportalo _importalo;
        public readonly IGeneralo _generalo;
        public readonly ISzuro _szuro;
        public readonly IStatisztikaElemzo _statisztika;
        public readonly MertekegysegKezelo _mertekegysegKezelo;
        public readonly IJogosultsagKezelo _jogosultsag;
        public readonly Megjelenito _megjelenito;
        public readonly ParancsKezelo _parancsok;
        private Felhasznalo _aktFelhasznalo;

        // MÓDOSÍTVA: IGeneralo eltávolítva (10 paraméter)
        public AlkalmazasVezerlo(
            IAdatforras adatforras,
            IImportalo importalo,
            IGeneralo generalo,
            ISzuro szuro,
            IStatisztikaElemzo statisztika,
            MertekegysegKezelo mertekegysegKezelo,
            IJogosultsagKezelo jogosultsag,
            Megjelenito megjelenito,
            ParancsKezelo parancsKezelo,
            Felhasznalo alapFelhasznalo)
        {
            _adatforras = adatforras ?? throw new ArgumentNullException(nameof(adatforras));
            _importalo = importalo ?? throw new ArgumentNullException(nameof(importalo));
            _generalo = generalo ?? throw new ArgumentNullException(nameof(generalo));
            _szuro = szuro ?? throw new ArgumentNullException(nameof(szuro));
            _statisztika = statisztika ?? throw new ArgumentNullException(nameof(statisztika));
            _mertekegysegKezelo = mertekegysegKezelo ?? throw new ArgumentNullException(nameof(mertekegysegKezelo));
            _jogosultsag = jogosultsag ?? throw new ArgumentNullException(nameof(jogosultsag));
            _megjelenito = megjelenito ?? throw new ArgumentNullException(nameof(megjelenito));
            _parancsok = parancsKezelo ?? throw new ArgumentNullException(nameof(parancsKezelo));
            _aktFelhasznalo = alapFelhasznalo ?? throw new ArgumentNullException(nameof(alapFelhasznalo));
        }

        public void Futtat()
        {
            bool fut = true;

            while (fut)
            {
                _megjelenito.Menu();
                _megjelenito.Szoveg($"Bejelentkezve: {_aktFelhasznalo.Nev} ({_aktFelhasznalo.Szerep}) | Aktuális mértékegység: {_mertekegysegKezelo.Mertekegyseg.Alapertelmezett}");
                _megjelenito.Szoveg("-------------------------------------");

                var parancs = _parancsok.BekerParancs();

                try
                {
                    switch (parancs)
                    {
                        case "1":
                            Importalas();
                            break;
                        case "2":
                            Generalas();
                            break;
                        case "3":
                            Megtekintes();
                            break;
                        case "4":
                            Szures();
                            break;
                        case "5":
                            Statisztika();
                            break;
                        case "6":
                            ModositMertekegyseg();
                            break;
                        case "7":
                            TorolMindent();
                            break;
                        case "0":
                            fut = false;
                            break;
                        default:
                            _megjelenito.Hiba("Ismeretlen parancs.");
                            break;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _megjelenito.Hiba($"Nincs jogosultság: {ex.Message}");
                }
                catch (FileNotFoundException ex)
                {
                    _megjelenito.Hiba($"Fájl hiba: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    _megjelenito.Hiba($"Beviteli hiba: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _megjelenito.Hiba($"Hiba történt: {ex.Message}");
                }

                if (fut)
                {
                    Console.WriteLine("\nNyomj Enter-t a folytatáshoz...");
                    Console.ReadLine();
                }
            }
        }

        private void Importalas()
        {
            string fajl = _parancsok.BekerSzoveg("Fájl elérési útja (.csv, .json, .xml): ");

            var (sikeres, hibas, lista) = _importalo.Importal(fajl);

            _adatforras.AdatHalmaz.HozzaadTobb(lista);
            _megjelenito.Siker($"Importálás befejezve. Sikeresen importálva: {sikeres}, hibás sorok/elemek: {hibas}");
        }

        private void Generalas()
        {
            
            // 1. Inputok bekérése (Vezérlő feladata: UI és logika összekötése)
            DateTime tol = _parancsok.BekerDatum("Intervallum eleje (YYYY-MM-DD HH:MM:SS):");
            DateTime ig = _parancsok.BekerDatum("Intervallum vége (YYYY-MM-DD HH:MM:SS):");
            
            if (tol > ig)
            {
            _megjelenito.Hiba("A kezdő dátum nem lehet későbbi a végsőnél!");
            return;
            }

            // Specifikáció: darabszám és értéktartomány bekérése
            double minErtek = _parancsok.BekerDouble("Minimum érték:");
            double maxErtek = _parancsok.BekerDouble("Maximum érték:");
            
            int darab = (int)_parancsok.BekerDouble("Generálandó darabszám:"); // Feltételezve, hogy van int bekérő, vagy castolunk
           
            // 2. Logika átadása a szerviznek (SOLID - SRP)
            var ujAdatok = _generalo.General(
                darab, 
                tol, 
                ig, 
                minErtek, 
                maxErtek, 
                _mertekegysegKezelo.Mertekegyseg.Alapertelmezett
            );

            // 3. Eredmény tárolása
            _adatforras.AdatHalmaz.HozzaadTobb(ujAdatok); // Feltételezve, hogy van HozzaadTobb, vagy foreach Add
            
            _megjelenito.Siker($"{darab} adat sikeresen generálva és hozzáadva.");
        }

        private void Megtekintes()
        {
            if (_adatforras.AdatHalmaz.Ures)
            {
                _megjelenito.Hiba("Nincs megjeleníthető adat.");
                return;
            }

            const int oldalMeret = 10;

            var adatok = _adatforras.AdatHalmaz.Adatok.OrderBy(a => a.Ido).ToList();
            
            int osszDarab = adatok.Count;
            int oldalSzam = (int)Math.Ceiling((double)osszDarab / oldalMeret);
            int aktualisOldal = 1;

            while (true)
            {
                Console.Clear();
                int kihagy = (aktualisOldal - 1) * oldalMeret;

                var aktualisAdatok = adatok
                    .Skip(kihagy)
                    .Take(oldalMeret);

                _megjelenito.Szoveg(new string('=', 45));
                _megjelenito.Szoveg($"--- Adatok ({aktualisOldal}/{oldalSzam} oldal, összesen: {osszDarab} db) ---");
                _megjelenito.Szoveg(new string('=', 45));
                _megjelenito.MegjelenitListat(aktualisAdatok);
                _megjelenito.Szoveg(new string('-', 45));
                _megjelenito.Szoveg("[N]ext: Következő oldal | [P]rev: Előző oldal | [M]enu: Vissza a főmenübe");

                var parancs = _parancsok.BekerParancs().ToLower();

                if (parancs == "m")
                    break;

                if (parancs == "n" && aktualisOldal < oldalSzam)
                {
                    aktualisOldal++;
                }
                else if (parancs == "p" && aktualisOldal > 1)
                {
                    aktualisOldal--;
                }
                else if (parancs != "n" && parancs != "p")
                {
                    _megjelenito.Hiba("Ismeretlen parancs. Használd az N, P vagy M betűt.");
                }
            }
        }

        private void Szures()
        {
            var adatok = _adatforras.AdatHalmaz.Adatok.ToList();

            if (!adatok.Any())
            {
                _megjelenito.Hiba("Nincs adat a szűréshez.");
                return;
            }

            var tol = _parancsok.BekerDatumNull("Időintervallum - Tól (ENTER = nincs):");
            var ig = _parancsok.BekerDatumNull("Időintervallum - Ig (ENTER = nincs):");
            var min = _parancsok.BekerDoubleNull("Min (ENTER = nincs):");
            var max = _parancsok.BekerDoubleNull("Max (ENTER = nincs):");

            var eredmeny = _szuro.Szures(
                adatok,
                tol, ig, min, max
            );

            _megjelenito.Szoveg($"Szűrés eredménye ({eredmeny.Count()} találat):");
            _megjelenito.MegjelenitListat(eredmeny);
        }

        private void Statisztika()
        {
            var adatok = _adatforras.AdatHalmaz.Adatok.ToList();

            if (!adatok.Any())
            {
                _megjelenito.Hiba("Nincs adat a statisztikához.");
                return;
            }

            _megjelenito.Szoveg("--- Összesített statisztika ---");
            _megjelenito.Szoveg($"Min: {_statisztika.Min(adatok):F2} {_mertekegysegKezelo.Mertekegyseg.Alapertelmezett}");
            _megjelenito.Szoveg($"Max: {_statisztika.Max(adatok):F2} {_mertekegysegKezelo.Mertekegyseg.Alapertelmezett}");
            _megjelenito.Szoveg($"Átlag: {_statisztika.Atlag(adatok):F2} {_mertekegysegKezelo.Mertekegyseg.Alapertelmezett}");
            _megjelenito.Szoveg($"Darabszám: {_statisztika.Darabszam(adatok)}");
            _megjelenito.Szoveg("------------------------------");

            var napi = _statisztika.NapiStatisztika(adatok);
            _megjelenito.MegjelenitNapiStat(napi);
        }

        private void ModositMertekegyseg()
        {
            _jogosultsag.Kovetel(_aktFelhasznalo.Szerep, Role.Admin);

            string uj = _parancsok.BekerSzoveg("Új alapértelmezett mértékegység: ");

            try
            {
                // 1. Elkérjük a konverziós logikát. Ha az átváltás nem definiált, InvalidOperationException dobódik.
                var konverziok = _mertekegysegKezelo.Modosit(uj);

                if (konverziok.Any())
                {
                    int konvertaltAdatDb = 0;

                    // 2. Konverzió végrehajtása az ÖSSZES betöltött adaton
                    foreach (var adat in _adatforras.AdatHalmaz.Adatok)
                    {
                        // Megkeressük a konverziós függvényt az adat aktuális mértékegysége alapján
                        if (konverziok.TryGetValue(adat.MertEgyseg, out var konverzioreFuggveny))
                        {
                            // Végrehajtjuk az érték konverzióját
                            adat.Ertek = konverzioreFuggveny(adat.Ertek);

                            // FRISSÍTJÜK az adat mértékegységét az új alapértelmezettre (pl. °C -> °F)
                            adat.MertEgyseg = uj;
                            konvertaltAdatDb++;
                        }
                        // Azokat az adatok, amelyekhez nincs szabály (pl. %), érintetlenül maradnak az eredeti mértékegységükkel.
                    }
                    _megjelenito.Siker($"Mértékegység módosítva {uj}-re. {konvertaltAdatDb} db adat konvertálva.");
                }
                else
                {
                    _megjelenito.Siker($"Mértékegység módosítva {uj}-re. Nincs szükség konverzióra.");
                }
            }
            catch (InvalidOperationException ex)
            {
                // Hiba, ha a konverzió nem definiált (pl. °C -> hPa)
                _megjelenito.Hiba($"Hiba a mértékegység módosításakor: {ex.Message}");
            }
        }

        private void TorolMindent()
        {
            // Jogosultság ellenőrzése (Admin)
            _jogosultsag.Kovetel(_aktFelhasznalo.Szerep, Role.Admin);

            _adatforras.TorolMindent();
            _megjelenito.Siker("Minden adat törölve.");
        }
    }
}