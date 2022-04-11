using System;
using Raylib_cs;

namespace RaylibStarterCS
{
	class Program
	{
		static void Main(string[] args)
        {
            // Setup game
            Game game = new Game();
            // Initialise game
            Raylib.InitWindow(640, 480, "Tank Game - Ben Wharton");
            game.Init(1200, 700);
            Raylib.SetTargetFPS(520);

            while (!Raylib.WindowShouldClose())
            {
                // Call game update
                game.Update();

                // Check if game should end
                if (game.GameEndOption == "Close")
                {
                    break;
                }
                // Check if game should restart
                if (game.GameEndOption == "Restart")
                {
                    // Re-initialise game
                    game = new Game();
                    game.Init(1200, 700);
                }
                // Call game draw
                game.Draw();
                
            }
        }
	}
}
