using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class angrylego : PhysicsGame
{
    public override void Begin()
    {
        // Kirjoita ohjelmakoodisi tähän
        Level.Background.CreateGradient(Color.Blue, Color.White);
        TileMap tiles = TileMap.FromStringArray(lines);

        tiles.SetTileMethod('X', LuoSeina, Color.Wheat);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
}

