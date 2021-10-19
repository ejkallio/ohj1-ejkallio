using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class Hiisipeli : PhysicsGame
{
    private static readonly String[] lines = {
                  "      X           X     ",
                  "                        ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "                        ",
                  "                        ",
                  "                        ",
                  "                        ",
                  "Y                      Y",
                  "                        ",
                  "                        ",
                  "      X           X     ",
                  };
    
    private static readonly int tileWidth = 2000 / lines[0].Length;
    private static readonly int tileHeight = 1000 / lines.Length;

    Vector nopeusYlos = new Vector(0, 300);
    Vector nopeusAlas = new Vector(0, -300);  
    Vector nopeusOikealle = new Vector(300, 0);
    Vector nopeusVasemmalle = new Vector(-300, -0);

    PhysicsObject pelaaja;

    public override void Begin()
    {       
        LuoKentta();
        LuoOhjaus();
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli"); 
        
        TileMap tiles = TileMap.FromStringArray(lines);

        tiles.SetTileMethod('X', LuoSeinax, Color.Wheat);
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

        

        

        Camera.ZoomToLevel();

        pelaaja = new PhysicsObject(60.0, 60.0);
        pelaaja.Shape = Shape.Circle;
        pelaaja.Color = Color.Black;
        pelaaja.X = 0.0;
        pelaaja.Y = 200.0;

        Add(pelaaja);
    }

    private void LuoOhjaus()
    {
        Keyboard.Listen(Key.W, ButtonState.Down, SaadaNopeus, "Liikuta ritaria ylös", pelaaja, nopeusYlos);
        Keyboard.Listen(Key.W, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.S, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Alas", pelaaja, nopeusAlas);
        Keyboard.Listen(Key.S, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.D, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Oikealle", pelaaja, nopeusOikealle);
        Keyboard.Listen(Key.D, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);

        Keyboard.Listen(Key.A, ButtonState.Down, SaadaNopeus, "Liikuta ritaria Vasemmalle", pelaaja, nopeusVasemmalle);
        Keyboard.Listen(Key.A, ButtonState.Released, SaadaNopeus, null, pelaaja, Vector.Zero);
    }


    private void SaadaNopeus(PhysicsObject pelaaja, Vector nopeus)
    {
        pelaaja.Velocity = nopeus;
    }

    private PhysicsObject LuoSeinay(double x, double y)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(100.0, 1000.0);
        Seina.X = x;
        Seina.Y = y;
        Add(Seina);
        return Seina;
    }

    private PhysicsObject LuoSeinax(double x, double y)
    {
        PhysicsObject Seina = PhysicsObject.CreateStaticObject(2000.0, 150.0);
        Seina.X = x;
        Seina.Y = y;
        Add(Seina);
        return Seina;
    }
}

