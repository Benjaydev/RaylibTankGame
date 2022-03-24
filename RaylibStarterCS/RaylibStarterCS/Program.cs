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
                game.Update();
                if (game.GameEndOption == "Close")
                {
                    break;
                }
                if (game.GameEndOption == "Restart")
                {
                    game = new Game();
                    game.Init();
                }
                game.Draw();
                
            }
        }
	}
}
