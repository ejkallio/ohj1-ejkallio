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
    PhysicsObject Miekka;

    public override void Begin()
    {
        ClearGameObjects();
        ClearControls();
        LuoKentta();
        LuoOhjaus();
    }


    void LuoKentta()
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


        pelaaja = new PhysicsObject(60.0, 60.0);
        pelaaja.Shape = Shape.Circle;
        pelaaja.Color = Color.Black;
        pelaaja.X = 0.0;
        pelaaja.Y = 200.0;
        AddCollisionHandler(pelaaja, "hiisi", PelaajaanOsui);

        Add(pelaaja);

        Miekka = PhysicsObject.CreateStaticObject(40.0, 60.0);
        Miekka.Shape = Shape.Triangle;
        Miekka.Color = Color.DarkRed;
        Miekka.Tag = "miekka";
        AddCollisionHandler(Miekka, "hiisi", HiiteenOsui);
    }

    private void LuoOhjaus()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä kontrollit");
        Keyboard.Listen(Key.F2, ButtonState.Pressed, Begin, "Uusi Peli");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli"); 

        Keyboard.Listen(Key.W, ButtonState.Down, SaadaNopeus, "Liikuta ritaria ylös", pelaaja, nopeusYlos);
        Keyboard.Listen(Key.W, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.S, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Alas", pelaaja, nopeusAlas);
        Keyboard.Listen(Key.S, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.D, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Oikealle", pelaaja, nopeusOikealle);
        Keyboard.Listen(Key.D, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.A, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Vasemmalle", pelaaja, nopeusVasemmalle);
        Keyboard.Listen(Key.A, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.Up, ButtonState.Pressed, LyoMiekalla, null, 0.0, 70.0, Miekka);
        /// Keyboard.Listen(Key.Up, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Down, ButtonState.Pressed, LyoMiekalla, null, 0.0, -70.0, Miekka);
        /// Keyboard.Listen(Key.Down, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Left, ButtonState.Pressed, LyoMiekalla, null, -70.0, 0.0, Miekka);
        /// Keyboard.Listen(Key.Left, ButtonState.Released, PoistaMiekka, null, Miekka);

        Keyboard.Listen(Key.Right, ButtonState.Pressed, LyoMiekalla, null, 70.0, 0.0, Miekka);
        /// Keyboard.Listen(Key.Right, ButtonState.Released, PoistaMiekka, null, Miekka);
    }


    private void SaadaNopeus(PhysicsObject pelaaja, Vector nopeus)
    {
        pelaaja.Velocity = nopeus;
    }

    private void LuoSeinay(Vector paikka, double x, double y, Color vari)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(80.0, 300.0);
        Seina.Position = paikka;
        Seina.Color = vari;
        Seina.Tag = "rakenne";
        Add(Seina);
    }

    private void LuoSeinax(Vector paikka, double x, double y, Color vari)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(400.0, 100.0);
        Seina.Position = paikka;
        Seina.Color = vari;
        Seina.Tag = "Seinä";
        Add(Seina);
    }

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

    private void LyoMiekalla(double x, double y, PhysicsObject miekka)
    {
        miekka.X = pelaaja.X + x;
        miekka.Y = pelaaja.Y + y;
        Add(miekka);
        Timer.CreateAndStart(0.1, miekka.Destroy);
    }

    private void PoistaMiekka(IPhysicsObject miekka)
    {
        Remove(miekka);
    }
}

