using MeterologiaiAdatbazis.Model;
using MeterologiaiAdatbazis.Services.Generalas;
using MeterologiaiAdatbazis.Services.Importalas;
using MeterologiaiAdatbazis.Services.Mertekegyseg;
using MeterologiaiAdatbazis.Services.Statisztika;
using MeterologiaiAdatbazis.Services.Szures;
using MeterologiaiAdatbazis.UserManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Xunit;

// A tesztosztályok a tesztelendõ komponensek alapján vannak elnevezve
public class StatisztikaElemzoTests
{
    private readonly StatisztikaElemzo _elemzo = new();
    private readonly List<Adat> _mintaAdatok = new()
    {
        new Adat(new DateTime(2023, 10, 15, 10, 0, 0), 10.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 15, 11, 0, 0), 20.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 16, 12, 0, 0), 30.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 16, 13, 0, 0), 40.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 17, 14, 0, 0), 50.0, "°C", AdatEredet.Imported)
    };

    [Fact]
    public void Statisztika_Atlag_HelyesErtek()
    {
        // Összeg: 10 + 20 + 30 + 40 + 50 = 150. Darabszám: 5. Átlag: 30.0
        double atlag = _elemzo.Atlag(_mintaAdatok);
        Assert.Equal(30.0, atlag);
    }

    [Fact]
    public void Statisztika_MinMax_HelyesErtek()
    {
        double min = _elemzo.Min(_mintaAdatok);
        double max = _elemzo.Max(_mintaAdatok);
        Assert.Equal(10.0, min);
        Assert.Equal(50.0, max);
    }

    [Fact]
    public void Statisztika_NapiStatisztika_HelyesCsoportositas()
    {
        // A kód ellenõrzi, hogy a csoportosítás napra (Ido.Date) történik-e
        var napiStat = _elemzo.NapiStatisztika(_mintaAdatok);

        Assert.Equal(3, napiStat.Count);
        Assert.Equal(10.0, napiStat[new DateTime(2023, 10, 15)].Min);
        Assert.Equal(35.0, napiStat[new DateTime(2023, 10, 16)].Atlag); // (30 + 40) / 2 = 35
    }
}

public class SzuroTests
{
    private readonly Szuro _szuro = new();
    private readonly List<Adat> _mintaAdatok = new()
    {
        new Adat(new DateTime(2023, 10, 15, 10, 0, 0), 10.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 15, 11, 0, 0), 20.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 16, 12, 0, 0), 30.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 17, 13, 0, 0), 40.0, "°C", AdatEredet.Imported),
        new Adat(new DateTime(2023, 10, 18, 14, 0, 0), 50.0, "°C", AdatEredet.Imported)
    };

    [Fact]
    public void Szuro_IdoEsErtekKombinaltSzures()
    {
        // Feltételek: 2023-10-16 00:00:00-tól, MAX 35.0 érték
        var tol = new DateTime(2023, 10, 16);
        double? max = 35.0;

        var eredmeny = _szuro.Szures(_mintaAdatok, tol, null, null, max);

        // Elvárt: 2023-10-16 12:00:00, 30.0
        Assert.Single(eredmeny);
        Assert.Equal(30.0, eredmeny.First().Ertek);
    }

    [Fact]
    public void Szuro_ErtekTartomanySzures()
    {
        double? min = 20.0;
        double? max = 40.0;

        var eredmeny = _szuro.Szures(_mintaAdatok, null, null, min, max);

        // Elvárt: 20.0, 30.0, 40.0 (3 találat)
        Assert.Equal(3, eredmeny.Count());
        Assert.True(eredmeny.All(a => a.Ertek >= 20.0 && a.Ertek <= 40.0));
    }

    [Fact]
    public void Szuro_UresAdathalmazEsVisszateresiLista()
    {
        var uresLista = new List<Adat>();
        var eredmeny = _szuro.Szures(uresLista, new DateTime(2020, 1, 1), null, 0.0, null);

        // Üres listára szûrve is üres listát kell visszaadnia, nem null-t
        Assert.Empty(eredmeny);
        Assert.NotNull(eredmeny);
    }
}

public class JogosultsagKezeloTests
{
    private readonly JogosultsagKezelo _jogosultsagKezelo = new();

    [Theory]
    [InlineData(Role.Admin, Role.Felhasznalo)]
    [InlineData(Role.Admin, Role.Admin)]
    [InlineData(Role.Felhasznalo, Role.Felhasznalo)]
    public void JogosultsagKezelo_SikeresEllenorzes(Role aktualis, Role szukseges)
    {
        bool eredmeny = _jogosultsagKezelo.Ellenoriz(aktualis, szukseges);
        Assert.True(eredmeny);
    }

    [Fact]
    public void JogosultsagKezelo_TiltottFelhasznaloiMuvelet()
    {
        // Elvárás: Felhasználó szerepkörrel Admin mûveletet megkövetelve kivételt kell dobni
        var aktualisSzerep = Role.Felhasznalo;
        var szuksegesSzerep = Role.Admin;

        Assert.Throws<UnauthorizedAccessException>(() =>
        {
            _jogosultsagKezelo.Kovetel(aktualisSzerep, szuksegesSzerep);
        });
    }
}

public class MertekegysegKezeloTests
{
    // A konverziós logika tesztelése
    [Fact]
    public void MertekegysegKezelo_Konverzio_C_to_F()
    {
        var kezdetiEgyseg = "°C";
        var kezdoErtek = 100.0; // 100°C = 212°F

        var kezdegys = new MertekegysegKezelo(kezdetiEgyseg);
        var konverziok = kezdegys.Modosit("°F");

        // 1. Ellenõrizzük, hogy a konverziós szabály létrejött-e
        Assert.True(konverziok.ContainsKey(kezdetiEgyseg));

        // 2. Végrehajtjuk a konverziót
        var konvertaloFuggveny = konverziok[kezdetiEgyseg];
        var konvertaltErtek = konvertaloFuggveny(kezdoErtek);

        // 3. Ellenõrizzük az eredményt (Floating point hiba miatt 2 tizedesjegyre kerekítünk)
        Assert.Equal(212.0, Math.Round(konvertaltErtek, 2));
    }

    [Fact]
    public void MertekegysegKezelo_Konverzio_NincsSzabaly()
    {
        var kezdegys = new MertekegysegKezelo("°C");
        var ujEgyseg = "hPa"; // Nyomás mértékegység, nincs definiált konverzió

        // Elvárás: Kivételt kell dobni, ha a célmértékegységre nincs definiálva szabály
        Assert.Throws<InvalidOperationException>(() =>
        {
            kezdegys.Modosit(ujEgyseg);
        });
    }
}

public class AdatHalmazTests
{
    [Fact]
    public void AdatHalmaz_HozzaadTobb_Es_UresEllenorzes()
    {
        var halmaz = new AdatHalmaz();
        Assert.True(halmaz.Ures); // Kezdetben üres

        var ujAdatok = new List<Adat>
        {
            new Adat(DateTime.Now, 1.0, "A", AdatEredet.Generated),
            new Adat(DateTime.Now, 2.0, "A", AdatEredet.Generated)
        };

        halmaz.HozzaadTobb(ujAdatok);

        Assert.False(halmaz.Ures);
        Assert.Equal(2, halmaz.Adatok.Count);
    }
}

public class GeneraloTests
{
    private readonly Generalo _generalo = new();

    [Fact]
    public void Generalo_HelyesDarabszam()
    {
        var tol = new DateTime(2023, 1, 1);
        var ig = new DateTime(2023, 1, 2);
        int darab = 15;

        var adatok = _generalo.General(darab, tol, ig, 0, 100, "°C");

        Assert.Equal(darab, adatok.Count);
    }

    [Fact]
    public void Generalo_ErtekekAMegadottTartomanyban()
    {
        var tol = new DateTime(2023, 1, 1);
        var ig = new DateTime(2023, 1, 2);
        double min = 5.0;
        double max = 15.0;

        var adatok = _generalo.General(100, tol, ig, min, max, "°C");

        Assert.All(adatok, adat => Assert.InRange(adat.Ertek, min, max));
    }
}

// A Service Locator (ImportaloSzolgaltatas) hibakezelésének tesztje
public class ImportaloSzolgaltatasTests
{
    private readonly ImportaloSzolgaltatas _szolgaltatas = new();

    [Fact]
    public void ImportaloSzolgaltatas_NemTamogatottFajlTipus()
    {
        // .pdf kiterjesztés nincs támogatva
        Assert.Throws<NotSupportedException>(() =>
        {
            _szolgaltatas.Importal("bemenet.pdf");
        });
    }
}

public class TerhelesesTeljesitmenyTests
{
    // --- PARAMÉTEREK ---
    private const int AdatokDarabszama = 50000;
    // A maximális elfogadható idõkorlát 500 ms (fél másodperc) 50 000 adat feldolgozására.
    private const int MaxMegengedettMs = 500;

    // Ez a teszt alapértelmezetten ki van kapcsolva!
    // A futtatáshoz a 'Skip' részt ki kell törölni: [Fact]
    [Fact]
    public void TerhelesesTeszt_NapiStatisztika_Gyorsasaga()
    {
        // Függõségek inicializálása
        var generalo = new Generalo();
        var elemzo = new StatisztikaElemzo();
        var stopper = new Stopwatch();

        Console.WriteLine($"\n--- Terheléses teszt indítása ({AdatokDarabszama} adat) ---");

        // 1. ADATGENERÁLÁS (Elõkészítés)
        // Szimulálunk 5 évnyi adatot, hogy legyen elég nap a csoportosításhoz.
        stopper.Start();
        var adatok = generalo.General(
            AdatokDarabszama,
            DateTime.Now.AddYears(-5),
            DateTime.Now,
            -20.0,
            40.0,
            "°C");
        stopper.Stop();

        Console.WriteLine($"Generálás ideje: {stopper.ElapsedMilliseconds} ms");

        // 2. TERHELÉSI MÛVELET (Napi statisztika)
        // A leginkább terhelõ mûvelet a GroupBy és az aggregátumok miatt.
        stopper.Reset();
        stopper.Start();
        var napiStat = elemzo.NapiStatisztika(adatok);
        stopper.Stop();

        long elteltIdoMs = stopper.ElapsedMilliseconds;

        Console.WriteLine($"Statisztika számítás ideje: {elteltIdoMs} ms");
        Console.WriteLine($"Eredmény: {napiStat.Count} napra csoportosítva.");


        // 3. EREDMÉNY ÉS ASSZERCIÓ

        // Elõször ellenõrizzük, hogy a logika helyesen futott-e le
        Assert.True(napiStat.Count > 1, "A teszt nem generált elegendõ napi statisztikát.");

        // Ez a kritikus lépés: Ellenõrizzük, hogy a futási idõ az elfogadott határon belül van-e.
        Assert.True(elteltIdoMs < MaxMegengedettMs,
            $"HIBA: A teszt túllépte az idõkorlátot. {AdatokDarabszama} adat elemzése {elteltIdoMs} ms-ig tartott, az elfogadott maximum {MaxMegengedettMs} ms.");

        Console.WriteLine($"--- Teszt SIKERES: A feldolgozás {elteltIdoMs} ms alatt teljesült. ---");
    }
}

