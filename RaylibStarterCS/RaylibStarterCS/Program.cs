using System;
using Raylib_cs;

namespace RaylibStarterCS
{
	class Program
	{
		static void Main(string[] args)
        {
            Game game = new Game();

            Raylib.InitWindow(640, 480, "Tank Game - Ben Wharton");

            game.Init();

            while (!Raylib.WindowShouldClose())
            {
                if (game.GameActive)
                {
                    game.Update();
                    game.Draw();
                }
                
            }

            game.Shutdown();

            Raylib.CloseWindow();
        }
	}
}
