using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class Hiisipeli : PhysicsGame
{
    private static readonly String[] lines_1 = {
                  "     X      X      X    ",
                  "                        ",
                  "Y           P          Y",
                  "                        ",
                  "                        ",
                  "                 H      ",
                  "Y      H               S",
                  "                        ",
                  "                  J     ",
                  "         J              ",
                  "Y                      Y",
                  "                        ",
                  "     X      X      X    ",
                  };

    private static readonly String[] lines_2 = {
                  "     X      R      X    ",
                  "                        ",
                  "Y  P                   Y",
                  "                  J     ",
                  "                        ",
                  "                 H      ",
                  "S                      S",
                  "       H                ",
                  "                  J     ",
                  "              H         ",
                  "Y     J                Y",
                  "                        ",
                  "     X      X      X    ",
                  };

    private static readonly String[] lines_3 = {
                  "     X      R      X    ",
                  "                        ",
                  "Y   P                  Y",
                  "                        ",
                  "    H                   ",
                  "                 H      ",
                  "S    J                 Y",
                  "                        ",
                  "                  J     ",
                  "       H                ",
                  "Y                      Y",
                  "                        ",
                  "     X      R      X    ",
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
    public override void Begin()
    {
        SeuraavaKentta();       
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
        AddCollisionHandler(pelaaja, "nuoli", PelaajaanOsui);
        AddCollisionHandler(pelaaja, "reuna", SeuraavaTaso);
        Add(pelaaja);

        LuoKentta();
        LuoOhjaus();
        Level.Background.CreateGradient(Color.White, Color.Black);
        

    }


    private void LuoKentta()
    {
        List<String[]> tasot = new List<String[]>();

        tasot.Add(lines_1);
        tasot.Add(lines_2);
        tasot.Add(lines_3);

        TileMap tiles = TileMap.FromStringArray(tasot[kenttanro]);
        tiles.SetTileMethod('X', LuoSeinax, Color.DarkGray);
        tiles.SetTileMethod('Y', LuoSeinay, Color.DarkGray);
        tiles.SetTileMethod('H', LuoHiisi, Color.ForestGreen);
        tiles.SetTileMethod('J', LuoJousihiisi, Color.BrightGreen);
        tiles.SetTileMethod('R', LuoRikottavaSeinax, Color.Brown);
        tiles.SetTileMethod('S', LuoRikottavaSeinay, Color.Brown);

        tiles.Execute(tileWidth, tileHeight);
    }

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

        Keyboard.Listen(Key.Up, ButtonState.Pressed, LyoMiekalla, null, 0.0, 80.0);
        /// Keyboard.Listen(Key.Up, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Down, ButtonState.Pressed, LyoMiekalla, null, 0.0, -80.0);
        /// Keyboard.Listen(Key.Down, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Left, ButtonState.Pressed, LyoMiekalla, null, -80.0, 0.0);
        /// Keyboard.Listen(Key.Left, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Right, ButtonState.Pressed, LyoMiekalla, null, 80.0, 0.0);
        /// Keyboard.Listen(Key.Right, ButtonState.Released, PoistaMiekka, null, Miekka);
    }


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


    private void LuoRikottavaSeinay(Vector paikka, double x, double y, Color vari)
    {
        rikottavaseinay = PhysicsObject.CreateStaticObject(80.0, 200.0);
        rikottavaseinay.Position = paikka;
        rikottavaseinay.Color = vari;
        rikottavaseinay.Restitution = 0;
        rikottavaseinay.Tag = "ovi";
        Add(rikottavaseinay);
    }


    private void LuoRikottavaSeinax(Vector paikka, double x, double y, Color vari)
    {
        rikottavaseinax = PhysicsObject.CreateStaticObject(250.0, 100.0);
        rikottavaseinax.Position = paikka;
        rikottavaseinax.Color = vari;
        rikottavaseinax.Restitution = 0;
        rikottavaseinax.Tag = "ovi";
        Add(rikottavaseinax);
    }


    private void LuoPelaaja(Vector paikka, double x, double y, Color vari)
    {
        pelaaja = new PhysicsObject(60.0, 60.0);
        pelaaja.Shape = Shape.Circle;
        pelaaja.Color = Color.Black;
        pelaaja.X = x;
        pelaaja.Y = y;
        pelaaja.Image = LoadImage("ritari1-export.gif");
        pelaaja.IgnoresPhysicsLogics = true;
        pelaaja.Restitution = 0;
        AddCollisionHandler(pelaaja, "hiisi", PelaajaanOsui);
        AddCollisionHandler(pelaaja, "nuoli", PelaajaanOsui);

        Add(pelaaja);
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
        /// Hiisi.Image = "hiisi";
        Add(hiisi);
        hiisia.Value += 1;
    }


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


    private FollowerBrain AmpujaAivot(PhysicsObject pelaaja)
    {
        FollowerBrain aivot = new FollowerBrain(pelaaja);
        aivot.Speed = 100.0;
        aivot.DistanceClose = 400.0;
        aivot.StopWhenTargetClose = true;
        aivot.TargetClose += HiisiAmpuu;
        return aivot;

    }


    private void HiisiAmpuu()
    {
        Timer.Limit(HiisiAmpuu, 2.0);
        Timer.CreateAndStart(5.0, Ampuu);
    }


    private void Ampuu()
    {
        PhysicsObject nuoli = HiidenNuoli(jousihiisi);
        Add(nuoli);
        Timer.CreateAndStart(2.0, nuoli.Destroy);
    }


    private PhysicsObject HiidenNuoli(PhysicsObject jousihiisi)
    {
        PhysicsObject nuoli = new PhysicsObject(10.0, 10.0);
        nuoli.Shape = Shape.Circle;
        nuoli.Color = Color.Red;
        nuoli.X = jousihiisi.X;
        nuoli.Y = jousihiisi.Y;
        nuoli.Velocity = new Vector(pelaaja.X, pelaaja.Y);
        nuoli.Tag = "nuoli";
        nuoli.IgnoresCollisionWith(jousihiisi);
        nuoli.IgnoresCollisionWith(hiisi);
        return nuoli;
    }


    private void TapaPelaaja(IPhysicsObject pelaaja)
    {
        Explosion kuolema = new Explosion(pelaaja.Width * 2);
        kuolema.Position = pelaaja.Position;
        kuolema.UseShockWave = false;
        Add(kuolema);
        Remove(pelaaja);
        Label kuolemateksti = new Label("Kuolit! Paina R aloittaaksesi uuden pelin.");
        kuolemateksti.TextColor = Color.Black;
        kuolemateksti.Y = 100.0;
        Add(kuolemateksti);
    }


    private void TapaHiisi(IPhysicsObject hiisi)
    {
        Explosion kuolema = new Explosion(hiisi.Width * 2);
        kuolema.Position = hiisi.Position;
        kuolema.UseShockWave = false;
        Add(kuolema);
        Remove(hiisi);
    }


    private void PelaajaanOsui(PhysicsObject pelaaja, PhysicsObject hiisi)
    {
        TapaPelaaja(pelaaja);
        kenttanro = 0;
    }


    private void HiiteenOsui(PhysicsObject Miekka, PhysicsObject hiisi)
    {
        TapaHiisi(hiisi);
    }


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
    private void LyoMiekalla(double x, double y)
    {
        // Tähän TeeMiekka- aliohjelman kutsu
        //if (miekka != null)
        //{
        //    Add(miekka);
        //}
        PhysicsObject miekka =  TeeMiekka();

        miekka.X = pelaaja.X + x;
        miekka.Y = pelaaja.Y + y;
        Timer.CreateAndStart(0.1, miekka.Destroy);
        Remove(miekka);
    }


    // TeeMiekka aliohjelma tähän
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


    private PhysicsObject LuoKruunu()
    {
        PhysicsObject kruunu = PhysicsObject.CreateStaticObject(40.0, 20.0);
        kruunu.Shape = Shape.Rectangle;
        kruunu.Color = Color.Gold;
        AddCollisionHandler(pelaaja, kruunu, KoskeKruunua);
        Add(kruunu);
        return kruunu;
    }


    IntMeter LuoTappoLaskuri()
    {
        IntMeter laskuri = new IntMeter(0);
        
        return laskuri;
    }


    IntMeter LuoHiisiLaskuri()
    {
        IntMeter laskuri = new IntMeter(0);

        return laskuri;
    }


    private void LisaaLaskurit()
    {
        tapot = LuoTappoLaskuri();
        hiisia = LuoHiisiLaskuri();
    }


    private void KasittelePisteet(PhysicsObject miekka, PhysicsObject kohde)
    {
        int onkovika = RandomGen.NextInt(5);
        tapot.Value += 1;
        if (tapot.Value == hiisia.Value)
        {

            rikottavaseinay.Destroy();
        }
        if (onkovika == 4 && tapot.Value == hiisia.Value)
        {
            LuoKruunu();
        }
    }


    private void KoskeKruunua(PhysicsObject pelaaja, PhysicsObject kruunu)
    {
        Remove(pelaaja);
        Label voittoteksti = new Label("Löysit kääpiökuninkaan kruunun! Voitit pelin.");
        voittoteksti.TextColor = Color.Black;
        voittoteksti.Y = 100.0;
        Add(voittoteksti);
    }

}

