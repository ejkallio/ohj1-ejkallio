using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class Hiisipeli : PhysicsGame
{
    private static readonly String[] lines = {
                  "     X             X    ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "              H         ",
                  "                        ",
                  "                        ",
                  "      H                ",
                  "                  H     ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "     X             X    ",
                  };
    
    private static readonly int tileWidth = 1000 / lines[0].Length;
    private static readonly int tileHeight = 800 / lines.Length;

    Vector nopeusYlos = new Vector(0, 300);
    Vector nopeusAlas = new Vector(0, -300);  
    Vector nopeusOikealle = new Vector(300, 0);
    Vector nopeusVasemmalle = new Vector(-300, -0);

    PhysicsObject pelaaja;
    IntMeter tapot;
    public override void Begin()
    {
        ClearGameObjects();
        ClearControls();
        LuoKentta();
        LuoOhjaus();
        LisaaLaskuri();
    }

    /// <summary>
    /// Luodaan kenttä, määritellään pelaaja- ja miekkaoliot
    /// </summary>
    private void LuoKentta()
    {
        Level.Background.CreateGradient(Color.White, Color.Black);

        PhysicsObject YlaReuna = Level.CreateTopBorder();
        YlaReuna.IsVisible = false;

        PhysicsObject AlaReuna = Level.CreateBottomBorder();
        AlaReuna.IsVisible = false;

        PhysicsObject VasenReuna = Level.CreateLeftBorder();
        VasenReuna.IsVisible = false;

        PhysicsObject OikeaReuna = Level.CreateRightBorder();
        OikeaReuna.IsVisible = false;

        /// LuoSeinay(Level.Left, 0.0);
        /// LuoSeinay(Level.Right, 0.0);
        /// LuoSeinax(0.0, Level.Bottom);
        /// LuoSeinax(0.0, Level.Top);

        TileMap tiles = TileMap.FromStringArray(lines);

        tiles.SetTileMethod('X', LuoSeinax, Color.DarkGray);
        tiles.SetTileMethod('Y', LuoSeinay, Color.DarkGray);
        tiles.SetTileMethod('H', LuoHiisi, Color.BrightGreen);


        tiles.Execute(tileWidth, tileHeight);

        //gameobject tai staticobject
        pelaaja = new PhysicsObject(60.0, 60.0);
        pelaaja.Shape = Shape.Circle;
        pelaaja.Color = Color.Black;
        pelaaja.X = 0.0;
        pelaaja.Y = 200.0;
        pelaaja.IgnoresPhysicsLogics = true;
        pelaaja.Restitution = 0;
        AddCollisionHandler(pelaaja, "hiisi", PelaajaanOsui);

        Add(pelaaja);

    }

    private void LuoOhjaus()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä kontrollit");
        Keyboard.Listen(Key.F2, ButtonState.Pressed, Begin, "Uusi Peli");
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

        Keyboard.Listen(Key.Up, ButtonState.Pressed, LyoMiekalla, null, 0.0, 70.0);
        /// Keyboard.Listen(Key.Up, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Down, ButtonState.Pressed, LyoMiekalla, null, 0.0, -70.0);
        /// Keyboard.Listen(Key.Down, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Left, ButtonState.Pressed, LyoMiekalla, null, -70.0, 0.0);
        /// Keyboard.Listen(Key.Left, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Right, ButtonState.Pressed, LyoMiekalla, null, 70.0, 0.0);
        /// Keyboard.Listen(Key.Right, ButtonState.Released, PoistaMiekka, null, Miekka);
    }


    private void SaadaNopeus(PhysicsObject pelaaja, Vector nopeus)
    {
        pelaaja.Velocity += nopeus;
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
        Seina.Tag = "Seinä";
        Add(Seina);
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
        PhysicsObject Hiisi = new PhysicsObject(60.0, 60.0);
        Hiisi.Position = paikka;
        Hiisi.Color = vari;
        Hiisi.Shape = Shape.Triangle;
        /// Hiisi.Image = "hiisi";
        Hiisi.Tag = "hiisi";
        Add(Hiisi);
    }

    private void TapaPelaaja(IPhysicsObject pelaaja)
    {
        Explosion kuolema = new Explosion(pelaaja.Width * 2);
        kuolema.Position = pelaaja.Position;
        kuolema.UseShockWave = false;
        Add(kuolema);
        Remove(pelaaja);
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
    }

    private void HiiteenOsui(PhysicsObject Miekka, PhysicsObject hiisi)
    {
        TapaHiisi(hiisi);
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
    PhysicsObject TeeMiekka()
    {
        PhysicsObject miekka = PhysicsObject.CreateStaticObject(40.0, 60.0);
        miekka.Shape = Shape.Triangle;
        miekka.Color = Color.DarkRed;
        miekka.Tag = "miekka";
        AddCollisionHandler(miekka, "hiisi", HiiteenOsui);
        AddCollisionHandler(miekka, "hiisi", KasittelePisteet);
        Add(miekka);
        return miekka;
    }

    PhysicsObject LuoKruunu()
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
        laskuri.MaxValue = 3;
        
        return laskuri;
    }

    private void LisaaLaskuri()
    {
        tapot = LuoTappoLaskuri();
    }

    private void KasittelePisteet(PhysicsObject miekka, PhysicsObject kohde)
    {
        tapot.Value += 1;
        if (tapot.Value == 3) LuoKruunu();
    }

    private void KoskeKruunua(PhysicsObject pelaaja, PhysicsObject kruunu)
    {
        Remove(pelaaja);
        Label voittoteksti = new Label("Löysit kääpiökuninkaan kruunun! Voitit pelin.");
        voittoteksti.TextColor = Color.Black;
        voittoteksti.Y = 100.0;
        Add(voittoteksti);
    }

    private void PoistaMiekka(IPhysicsObject miekka)
    {
        Remove(miekka);
    }
}

