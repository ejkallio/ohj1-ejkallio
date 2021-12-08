using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

///
/// @author ejkallio
/// @version 8.12.2021
/// <summary>
/// Peli, jossa edetään huoneesta huoneeseen ja tuhotaan
/// hyökkäävät viholliset. Lopputavoitteena löytää kruunu, jota
/// koskettamalla voitetaan peli.
/// </summary>
/// <remarks>
/// To-do:
/// - Jousihiisien ampuminen kuntoon
/// - Miekan grafiikan/muodon parantelu
/// - Parempi grafiikka
/// - Lista nopeimmista ajoista
/// </remarks>
public class Hiisipeli : PhysicsGame
{
    private static readonly String[] lines_1 = {
                  "     X      X      X    ",
                  "                        ",
                  "Y           P          Y",
                  "                        ",
                  "                        ",
                  "                 H     S",
                  "Y      H               Y",
                  "                        ",
                  "                  J     ",
                  "         J              ",
                  "Y                      Y",
                  "                        ",
                  "     X      R      X    ",
                  };

    private static readonly String[] lines_2 = {
                  "     X      XR     X    ",
                  "                        ",
                  "Y  P                   Y",
                  "                  J     ",
                  "                        ",
                  "                 H      ",
                  "Y                      S",
                  "       H                ",
                  "                  J     ",
                  "              H         ",
                  "Y     J                Y",
                  "                        ",
                  "     X      X      X    ",
                  };

    private static readonly String[] lines_3 = {
                  "     X      X      X    ",
                  "                        ",
                  "Y   P                  Y",
                  "                        ",
                  "    H                   ",
                  "S                H      ",
                  "Y    J                 Y",
                  "                        ",
                  "                  J     ",
                  "       H                ",
                  "Y                      Y",
                  "                        ",
                  "     X      R      X    ",
                  };

    private static readonly String[] lines_4 = {
                  "     X      XR     X    ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "                        ",
                  "                        ",
                  "S                      Y",
                  "                        ",
                  "    H     H    H    H   ",
                  "       H                ",
                  "Y            J    H    Y",
                  "                        ",
                  "     X      X      X    ",
                  };

    private static readonly String[] lines_5 = {
                  "     X      X      X    ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "                        ",
                  "                        ",
                  "Y                      Y",
                  "    H               H   ",
                  "                        ",
                  "        H   J   H       ",
                  "Y    J             J   Y",
                  "                        ",
                  "     X      X      X    ",
                  };
    // lisää kenttiä!!!


    private static readonly int tileWidth = 1000 / lines_1[0].Length;
    private static readonly int tileHeight = 800 / lines_1.Length;

    Vector nopeusYlos = new Vector(0, 300);
    Vector nopeusAlas = new Vector(0, -300);  
    Vector nopeusOikealle = new Vector(300, 0);
    Vector nopeusVasemmalle = new Vector(-300, -0);

    PhysicsObject pelaaja;
    PhysicsObject hiisi;
    PhysicsObject jousihiisi;
    PhysicsObject rikottavaseinax;
    PhysicsObject rikottavaseinay;
    IntMeter tapot;
    IntMeter hiisia;
    int kenttanro = 0;


    /// <summary>
    /// Aloitusvalikko josta päästään varsinaiseen peliin
    /// </summary>
    public override void Begin()
    {
        MultiSelectWindow alkuValikko = new MultiSelectWindow("Hiisipeli", "Aloita peli", "Lopeta");
        alkuValikko.AddItemHandler(0, SeuraavaKentta);
        alkuValikko.AddItemHandler(1, Exit);
        alkuValikko.Color = Color.Green;
        Level.BackgroundColor = Color.Gray;
        Add(alkuValikko);               
    }


    /// <summary>
    /// Luodaan kenttä, määritellään pelaaja- ja miekkaoliot
    /// </summary>
    private void SeuraavaKentta()
    {
        ClearAll();

        LisaaLaskurit();

        PhysicsObject YlaReuna = Level.CreateTopBorder();
        YlaReuna.IsVisible = false;
        YlaReuna.Tag = "reuna";

        PhysicsObject AlaReuna = Level.CreateBottomBorder();
        AlaReuna.IsVisible = false;
        AlaReuna.Tag = "reuna";

        PhysicsObject VasenReuna = Level.CreateLeftBorder();
        VasenReuna.IsVisible = false;
        VasenReuna.Tag = "reuna";

        PhysicsObject OikeaReuna = Level.CreateRightBorder();
        OikeaReuna.IsVisible = false;
        OikeaReuna.Tag = "reuna";

        pelaaja = new PhysicsObject(60.0, 60.0);
        pelaaja.Shape = Shape.Circle;
        pelaaja.Color = Color.Black;
        pelaaja.X = 0.0;
        pelaaja.Y = 200.0;
        pelaaja.IgnoresPhysicsLogics = true;
        pelaaja.Restitution = 0;
        AddCollisionHandler(pelaaja, "hiisi", PelaajaanOsui);
        // AddCollisionHandler(pelaaja, "nuoli", PelaajaanOsui);
        AddCollisionHandler(pelaaja, "reuna", SeuraavaTaso);
        Add(pelaaja);

        LuoKentta();
        LuoOhjaus();
        Level.Background.CreateGradient(Color.White, Color.Black);
    }


    /// <summary>
    /// Määritellään pelin kentät 
    /// </summary>
    private void LuoKentta()
    {
        List<String[]> tasot = new List<String[]>();

        tasot.Add(lines_1);
        tasot.Add(lines_2);
        tasot.Add(lines_3);
        tasot.Add(lines_4);
        tasot.Add(lines_5);

        TileMap tiles = TileMap.FromStringArray(tasot[kenttanro]);
        tiles.SetTileMethod('X', LuoSeinax, Color.DarkGray);
        tiles.SetTileMethod('Y', LuoSeinay, Color.DarkGray);
        tiles.SetTileMethod('H', LuoHiisi, Color.ForestGreen);
        tiles.SetTileMethod('J', LuoJousihiisi, Color.BrightGreen);
        tiles.SetTileMethod('R', LuoRikottavaSeinax, Color.Brown);
        tiles.SetTileMethod('S', LuoRikottavaSeinay, Color.Brown);

        tiles.Execute(tileWidth, tileHeight);
    }


    /// <summary>
    /// Luodaan pelin ohjaukset
    /// </summary>
    private void LuoOhjaus()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä kontrollit");
        Keyboard.Listen(Key.R, ButtonState.Pressed, SeuraavaKentta, "Uusi Peli");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli"); 

        Keyboard.Listen(Key.W, ButtonState.Pressed, SaadaNopeus, "Liikuta ritaria ylös", pelaaja, nopeusYlos);
        Keyboard.Listen(Key.W, ButtonState.Released, SaadaNopeus, null, pelaaja, -nopeusYlos);

        Keyboard.Listen(Key.S, ButtonState.Pressed, SaadaNopeus, "Liikuta ritaria Alas", pelaaja, nopeusAlas);
        Keyboard.Listen(Key.S, ButtonState.Released, SaadaNopeus, null, pelaaja, -nopeusAlas);

        Keyboard.Listen(Key.D, ButtonState.Pressed, SaadaNopeus, "Liikuta ritaria Oikealle", pelaaja, nopeusOikealle);
        Keyboard.Listen(Key.D, ButtonState.Released, SaadaNopeus, null, pelaaja, -nopeusOikealle);

        Keyboard.Listen(Key.A, ButtonState.Pressed, SaadaNopeus, "Liikuta ritaria Vasemmalle", pelaaja, nopeusVasemmalle);
        Keyboard.Listen(Key.A, ButtonState.Released, SaadaNopeus, null, pelaaja, -nopeusVasemmalle);

        Keyboard.Listen(Key.Up, ButtonState.Pressed, LyoMiekalla, null, 0.0, 95.0, 0.0);

        Keyboard.Listen(Key.Down, ButtonState.Pressed, LyoMiekalla, null, 0.0, -95.0, 180.0);

        Keyboard.Listen(Key.Left, ButtonState.Pressed, LyoMiekalla, null, -80.0, 0.0, -90.0);

        Keyboard.Listen(Key.Right, ButtonState.Pressed, LyoMiekalla, null, 80.0, 0.0, 90.0);
    }


    /// <summary>
    /// Mahdollistetaan pelaajan liikkuminen 8 suuntaan
    /// (Rikkoo tällä hetkellä pelin fysiikkaa sen verran että pelaajahahmo lähtee törmättyään
    /// esineeseen poukkoilemaan vastakkaiseen suuntaan.)
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="nopeus"></param>
    private void SaadaNopeus(PhysicsObject pelaaja, Vector nopeus)
    {
        pelaaja.Velocity += nopeus;
        if (pelaaja.Velocity.X > 300.0) pelaaja.Velocity = new Vector(300.0, pelaaja.Velocity.Y);
        if (pelaaja.Velocity.X < -300.0) pelaaja.Velocity = new Vector(-300.0, pelaaja.Velocity.Y);
        if (pelaaja.Velocity.Y > 300.0) pelaaja.Velocity = new Vector(pelaaja.Velocity.X, 300.0);
        if (pelaaja.Velocity.Y < -300.0) pelaaja.Velocity = new Vector(pelaaja.Velocity.X, -300.0);

    }
    

    /// <summary>
    /// Luodaan pituussuuntaan seinä
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vari"></param>
    private void LuoSeinay(Vector paikka, double x, double y, Color vari)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(80.0, 300.0);
        Seina.Position = paikka;
        Seina.Color = vari;
        Seina.Restitution = 0;
        Seina.Tag = "rakenne";
        Add(Seina);
    }


    /// <summary>
    /// Luodaan leveyssuuntaan seinä
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vari"></param>
    private void LuoSeinax(Vector paikka, double x, double y, Color vari)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(400.0, 100.0);
        Seina.Position = paikka;
        Seina.Color = vari;
        Seina.Restitution = 0;
        Seina.Tag = "rakenne";
        Add(Seina);
    }


    /// <summary>
    /// Ovi pituussuunnassa
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vari"></param>
    private void LuoRikottavaSeinay(Vector paikka, double x, double y, Color vari)
    {
        rikottavaseinay = PhysicsObject.CreateStaticObject(80.0, 200.0);
        rikottavaseinay.Position = paikka;
        rikottavaseinay.Color = vari;
        rikottavaseinay.Restitution = 0;
        rikottavaseinay.Tag = "ovi";
        Add(rikottavaseinay, -1);
    }


    /// <summary>
    /// Ovi leveyssuunnassa
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vari"></param>
    private void LuoRikottavaSeinax(Vector paikka, double x, double y, Color vari)
    {
        rikottavaseinax = PhysicsObject.CreateStaticObject(250.0, 100.0);
        rikottavaseinax.Position = paikka;
        rikottavaseinax.Color = vari;
        rikottavaseinax.Restitution = 0;
        rikottavaseinax.Tag = "ovi";
        Add(rikottavaseinax, -1);
    }


    /// <summary>
    /// Luodaan pelin vihollinen eli hiisi,
    /// joka koskettaessaan pelaajaa tappaa hänet
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vari"></param>
    private void LuoHiisi(Vector paikka, double x, double y, Color vari)
    {
        hiisi = new PhysicsObject(60.0, 60.0);
        hiisi.Position = paikka;
        hiisi.Color = vari;
        hiisi.Shape = Shape.Triangle;
        hiisi.IgnoresPhysicsLogics = true;
        hiisi.CanRotate = false;
        hiisi.Tag = "hiisi";
        hiisi.Brain = Aivot(pelaaja);
        // hiisi.Image = ;
        Add(hiisi);
        hiisia.Value += 1;
    }


    /// <summary>
    /// Perushiisien aivoaliohjelma
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <returns></returns>
    private FollowerBrain Aivot(PhysicsObject pelaaja)
    {
        FollowerBrain aivot = new FollowerBrain(pelaaja);
        aivot.Speed = 150.0;
       
        return aivot;
    }


    /// <summary>
    /// tähän 2. hiisityyppi (ampuva hiisi)
    /// </summary>
    private void LuoJousihiisi(Vector paikka, double x, double y, Color vari)
    {
        jousihiisi = new PhysicsObject(60.0, 60.0);
        jousihiisi.Position = paikka;
        jousihiisi.Color = vari;
        jousihiisi.Shape = Shape.Triangle;
        jousihiisi.CanRotate = false;
        jousihiisi.Tag = "hiisi";
        jousihiisi.Brain = AmpujaAivot(pelaaja);
        ///Timer.CreateAndStart(2.0, Ampuu);
        Add(jousihiisi);
        hiisia.Value += 1;
    }


    /// <summary>
    /// Aivoaliohjelma ampuville hiisille
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <returns></returns>
    private FollowerBrain AmpujaAivot(PhysicsObject pelaaja)
    {
        FollowerBrain aivot = new FollowerBrain(pelaaja);
        aivot.Speed = 100.0;
        aivot.DistanceClose = 400.0;
        aivot.StopWhenTargetClose = true;
        aivot.TargetClose += Ampuu;
        return aivot;

    }


    /// <summary>
    /// Aliohjelma jonka on tarkoitus mahdollistaa jousihiisien
    /// hyökkäämisen ampumalla.
    /// </summary>
    private void Ampuu()
    {
        // Timer.Limit(Ampuu, 2.0);
        Timer ampumisLaskuri = new Timer();
        ampumisLaskuri.Interval = 2.0;
        ampumisLaskuri.Timeout += Ampuu;
        PhysicsObject nuoli = HiidenNuoli(jousihiisi);
        Timer.CreateAndStart(2.0, nuoli.Destroy);
    }


    /// <summary>
    /// Funktio jossa määritellään jousihiisien käyttämät ammukset.
    /// </summary>
    /// <param name="jousihiisi"></param>
    /// <returns></returns>
    private PhysicsObject HiidenNuoli(PhysicsObject jousihiisi)
    {
        PhysicsObject nuoli = new PhysicsObject(10.0, 10.0);
        nuoli.Shape = Shape.Circle;
        nuoli.Color = Color.Red;
        nuoli.X = jousihiisi.X;
        nuoli.Y = jousihiisi.Y;
        nuoli.Restitution = 0;
        nuoli.Velocity = new Vector(pelaaja.X, pelaaja.Y);
        nuoli.Tag = "nuoli";
        nuoli.IgnoresCollisionWith(jousihiisi);
        nuoli.IgnoresCollisionWith(hiisi);

        return nuoli;
    }


    /// <summary>
    /// Luodaan räjähdys joka tapahtuu pelaajan tuhoutuessa
    /// </summary>
    /// <param name="pelaaja"></param>
    private void TapaPelaaja(IPhysicsObject pelaaja)
    {
        Explosion kuolema = new Explosion(pelaaja.Width * 2);
        kuolema.Position = pelaaja.Position;
        kuolema.UseShockWave = true;
        Add(kuolema);
        Remove(pelaaja);
        Label kuolemateksti = new Label("Kuolit! Paina R aloittaaksesi uuden pelin.");
        kuolemateksti.TextColor = Color.Black;
        kuolemateksti.Y = 100.0;
        Add(kuolemateksti);
    }


    /// <summary>
    /// Luodaan räjähdys joka tapahtuu vihollisen tuhoutuessa
    /// </summary>
    /// <param name="hiisi"></param>
    private void TapaHiisi(IPhysicsObject hiisi)
    {
        Explosion kuolema = new Explosion(hiisi.Width * 2);
        kuolema.Position = hiisi.Position;
        kuolema.UseShockWave = false;
        Add(kuolema);
        Remove(hiisi);
    }


    /// <summary>
    /// Käsitellään vihollisen osumista pelaajaan
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="hiisi"></param>
    private void PelaajaanOsui(PhysicsObject pelaaja, PhysicsObject hiisi)
    {
        TapaPelaaja(pelaaja);
        kenttanro = 0;
    }


    /// <summary>
    /// Käsitellään miekan osumista viholliseen
    /// </summary>
    /// <param name="Miekka"></param>
    /// <param name="hiisi"></param>
    private void HiiteenOsui(PhysicsObject Miekka, PhysicsObject hiisi)
    {
        TapaHiisi(hiisi);
    }


    /// <summary>
    /// Kentän vaihtumista käsittelevä aliohjelma
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="Reuna"></param>
    private void SeuraavaTaso(PhysicsObject pelaaja, PhysicsObject Reuna)
    {
        kenttanro++;
        SeuraavaKentta();
    }


    /// <summary>
    /// Luodaan miekanlyönti joka tapahtuu nuolinäppäimistä
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="miekka"></param>
    private void LyoMiekalla(double x, double y, double kulma)
    {
        PhysicsObject miekka =  TeeMiekka();

        miekka.X = pelaaja.X + x;
        miekka.Y = pelaaja.Y + y;
        Angle miekanKulma = miekka.Angle;
        Timer.CreateAndStart(0.1, miekka.Destroy);
        Remove(miekka);
    }


    /// <summary>
    /// Funktio joka palauttaa miekka-fysiikkaolion 
    /// </summary>
    /// <returns></returns>
    private PhysicsObject TeeMiekka()
    {
        PhysicsObject miekka = PhysicsObject.CreateStaticObject(40.0, 60.0);
        miekka.Shape = Shape.Triangle;
        miekka.Color = Color.SkyBlue;
        miekka.Tag = "miekka";
        miekka.IgnoresPhysicsLogics = true;
        miekka.Restitution = 0;
        AddCollisionHandler(miekka, "hiisi", HiiteenOsui);
        AddCollisionHandler(miekka, "hiisi", KasittelePisteet);
        Add(miekka);
        return miekka;
    }


    /// <summary>
    /// Funktio luo peliin kruunun jota koskettamalla voitetaan peli.
    /// </summary>
    /// <returns></returns>
    private PhysicsObject LuoKruunu()
    {
        PhysicsObject kruunu = PhysicsObject.CreateStaticObject(40.0, 20.0);
        kruunu.Shape = Shape.Rectangle;
        kruunu.Color = Color.Gold;
        AddCollisionHandler(pelaaja, kruunu, KoskeKruunua);
        Add(kruunu);
        return kruunu;
    }


    /// <summary>
    /// Laskuri joka laskee pelaajan tuhoamat viholliset
    /// </summary>
    /// <returns></returns>
    private IntMeter LuoTappoLaskuri()
    {
        IntMeter laskuri = new IntMeter(0);
        
        return laskuri;
    }


    /// <summary>
    /// Laskuri joka laskee jokaisen kentän vihollismäärän
    /// </summary>
    /// <returns></returns>
     private IntMeter LuoHiisiLaskuri()
    {
        IntMeter laskuri = new IntMeter(0);

        return laskuri;
    }


    /// <summary>
    /// Määritellään laskurit
    /// </summary>
    private void LisaaLaskurit()
    {
        tapot = LuoTappoLaskuri();
        hiisia = LuoHiisiLaskuri();
    }


    /// <summary>
    /// Miekan osumista viholliseen käsittelevä aliohjelma
    /// </summary>
    /// <param name="miekka"></param>
    /// <param name="kohde"></param>
    private void KasittelePisteet(PhysicsObject miekka, PhysicsObject kohde)
    {
        tapot.Value += 1;
        if (tapot.Value == hiisia.Value && kenttanro != 4)
        {
            // TÄMÄ SILMUKAKSI !!!
            //!!!
            //!!!
            Remove(rikottavaseinax);
            Remove(rikottavaseinay);
            Remove(rikottavaseinax);
            Remove(rikottavaseinay);
        }
        if (kenttanro == 4 && tapot.Value == hiisia.Value)
        {
            LuoKruunu();
        }
    }


    /// <summary>
    /// Kruunun koskettamisen määrittely
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="kruunu"></param>
    private void KoskeKruunua(PhysicsObject pelaaja, PhysicsObject kruunu)
    {
        Remove(pelaaja);
        Label voittoteksti = new Label("Löysit kääpiökuninkaan kruunun! Voitit pelin.");
        voittoteksti.TextColor = Color.Black;
        voittoteksti.Y = 100.0;
        Add(voittoteksti);

        Timer.CreateAndStart(5.0, Exit);
    }

}

