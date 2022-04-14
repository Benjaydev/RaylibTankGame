using System;
using System.Diagnostics;
using Raylib_cs;
using System.Collections.Generic;
using System.IO;
using MathsClasses;
using System.Linq;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    class Game
    {
        // ------------
        // Timer and fps
        Stopwatch stopwatch = new Stopwatch();
        private long currentTime = 0;
        private long lastTime = 0;
        private float timer = 0;
        private int fps = 1;
        private int frames;
        public static float deltaTime = 0.005f;

        public static float screenScale = 1f;

        // Debug
        float debugCooldown = 0.5f;
        float debugCooldownCount = 0f;
        public static bool IsDebugActive = false;
        public static bool ShowFPS = false;
        public static bool FPSCapped = true;

        
        public static Vector3[] WorldBoundries = new Vector3[2];

        // Backgrounds
        SpriteObject background = new SpriteObject();
        SpriteObject endGameBackground = new SpriteObject();
        SpriteObject menuBackground = new SpriteObject();
        SpriteObject lightFilter = new SpriteObject();

        // Leaderboard scores
        List<string> scores = new List<string>();

        // Player tanks
        public static Tank playerTank;
        public static Tank mainMenuTank;

        // Keep track of scene objects
        public static List<SceneObject> sceneObjects;
        public static int gameLifetimeObjectCount;

        // Enemies
        public static List<Tank> enemies = new List<Tank>();
        Vector3[] spawnPoints = new Vector3[] { new Vector3(180, 380, 0), new Vector3(710, 615, 0), new Vector3(1050, 100, 0), new Vector3(1050, 280, 0) };
        float enemyCooldown = 5f;
        float enemyCooldownCount = 0f;

        // UI
        public static List<SceneObject> buttons;
        
        // Lighting
        public static List<Light> lights = new List<Light>();

        // Save input box for end screen
        InputBox saveInputBox;

        // Delegate used for storing and calling updates each frame
        public delegate void DelegateUpdate(float deltaTime = 0f);
        public DelegateUpdate delegateUpdateStore;

        // Delegate used for storing and calling destruction of objects each frame
        public delegate void DelegateDestroy();
        public DelegateDestroy delegateDestroyStore;

        // Randomiser for game
        public static Random gameRandom = new Random();

        // Scene states
        public bool GameActive = false;
        public bool MainMenu = true;
        public bool GameOver = false;

        // State of end game (Closing, restarting, etc)
        public string GameEndOption = "";
        // ------------


        // Game constructor
        public Game()
        {
        }

        // Initialise game
        public void Init(int width, int height)
        {
           
            // Setup window and game world size
            SetWindowSize(width, height);
            WorldBoundries = new Vector3[] { new Vector3(GetScreenWidth(), GetScreenHeight(), 0), new Vector3(0, 0, 0) };
            // Start world time
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            // Load backgrounds for later use
            // Main game background
            background.Load("./PNG/Environment/dirt.png");
            background.textureScale = 20;
            // End menu background
            endGameBackground.Load("./PNG/Environment/TitleBackground.png");
            endGameBackground.textureScale = 5;
            // Main menu background
            menuBackground.Load("./PNG/Environment/rock.png");
            menuBackground.textureScale = 10;

            // Create darkness filter game (For lights to be applied on top)
            lightFilter.Load("./PNG/Environment/black.png");
            lightFilter.colour = ColorAlpha(Color.BLACK, .5f);
            lightFilter.textureScale = 10;

            // Setup player tank
            playerTank = new Tank("Player");
            

            // Reset all scene objects, UI, and Lighting
            resetAllOUL();

            // Start main menu
            MainMenuScene();

        }
        public void resetAllOUL()
        {
            // Reset all scene objects, UI, and Lighting
            sceneObjects = new List<SceneObject>();
            enemies = new List<Tank>();
            buttons = new List<SceneObject>();
            lights = new List<Light>();
        }

        // Setup Main menu scene
        public void MainMenuScene()
        {
            // Set state of game
            MainMenu = true;
            GameActive = false;
            
            // Create play button
            int pbLength = 150;
            int pbHeight = 50;
            Button playButton = new Button( (pbLength/2)+25, 150, pbLength, pbHeight, "Play Game", 22, Color.BLACK, "ExitMainMenu");

            // Setup the file reader
            StreamReader reader = new StreamReader("Scores.txt");
            // Read top ten scores
            for(int i = 0; i < 10; i++)
            {
                string line = reader.ReadLine();
                if(line != null)
                {
                    scores.Add(line);
                }
            }
            reader.Close();
            // Save scores into list
            scores = scores.OrderByDescending(s => int.Parse(s.Split(": ", StringSplitOptions.None)[1])).ToList();

            // Setup meu tank
            mainMenuTank = new Tank("Menu");
            mainMenuTank.Init(GetScreenWidth()/2, GetScreenHeight()/2);
            mainMenuTank.Scale(0.5f, 0.5f);

            // Setup back light for menu
            Light menuLight = new Light(new Vector3(1000, 1000, 1000), 1f, 1f, new Color(255, 255, 255, 255), true);
            menuLight.SetPosition(GetScreenWidth() / 2, GetScreenHeight() / 2);
            lights.Add(menuLight);

        }
        public void EndGameScene()
        {
            // Set state of game
            GameOver = true;
            GameActive = false;
            IsDebugActive = false;

            // Reset all scene objects, UI, and Lighting
            resetAllOUL();

            // Store button positions
            int pbLength = 150;
            int pbHeight = 50;

            int ibLength = 200;
            int ibHeight = 25;

            int sbLength = 75;
            int sbHeight = 25;

            // Setup end game button
            Button restartButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) - 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Restart", 22, Color.BLACK, "Restart");
            Button closeButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) + 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Close", 22, Color.BLACK, "Close");
            Button saveButton = new Button((GetScreenWidth() / 2) - (sbLength / 2), (GetScreenHeight() / 2) - (sbHeight / 2) + 150, sbLength, sbHeight, "Save", 16, Color.BLACK, "Save", true);
            // Create save score input box
            saveInputBox = new InputBox((GetScreenWidth() / 2) - (ibLength / 2), (GetScreenHeight() / 2) - (ibHeight / 2) + 100, ibLength, ibHeight, "Type Name", 20, Color.BLACK, "");

            
            // Setup end game back lighting
            Light endMenuLight = new Light(new Vector3(1000, 200, 1100), 0.1f, 1f, new Color(255, 0, 0, 255));
            endMenuLight.SetPosition(GetScreenWidth() / 2, GetScreenHeight() / 2);
            lights.Add(endMenuLight);
        }

        public void GameScene()
        {
            // Set state of game
            MainMenu = false;
            GameActive = true;

            // Reset all scene objects, UI, and Lighting
            resetAllOUL();

            // Initiate player
            playerTank.Init(1100, 630);
            playerTank.Rotate(-90*DEG2RAD);

            // Create main game map
            new GameMap();
        }

        // Create delegates for game objects
        public void CreateStoreUpdateDelegate()
        {
            delegateUpdateStore = null;
            // Update scene objects (Stored in a delegate to avoid an error if object is destroyed during it's update)
            foreach (SceneObject obj in sceneObjects)
            {
                delegateUpdateStore += (deltaTime) => obj.Update(deltaTime);
                // If object is waiting to be destroyed
                if (obj.isWaitingDestroy)
                {
                    delegateDestroyStore += obj.RemoveSelfFromSceneObjects;
                }
            }
            
        }


        // Update game
        public void Update()
        {
            // Calculate the deltatime for this update
            currentTime = stopwatch.ElapsedMilliseconds;
            deltaTime = (currentTime - lastTime) / 1000.0f;

            // Calculate the fps
            timer += deltaTime;
            if (timer >= 1)
            {
                fps = frames;
                frames = 0;
                timer -= 1;
            }
            frames++;
            lastTime = currentTime;


            // Main game active update
            if (GameActive && !MainMenu && !GameOver)
            { 
                // End game if player has died
                if (playerTank.isWaitingDestroy)
                {
                    EndGameScene();
                }

                // Spawn new enemies after cooldown has completed
                enemyCooldownCount += deltaTime;
                if (enemyCooldownCount >= enemyCooldown)
                { 
                    CreateNewEnemy();
                    enemyCooldownCount = 0f;
                }

                // Shoot bullet
                if (IsKeyDown(KeyboardKey.KEY_SPACE))
                {
                    playerTank.ShootBullet();
                }

                // Rotate the tank to the left
                if (IsKeyDown(KeyboardKey.KEY_A))
                {
                    playerTank.Rotate(-deltaTime * 2);
                }
                // Rotate the tank to the right
                if (IsKeyDown(KeyboardKey.KEY_D))
                {
                    playerTank.Rotate(deltaTime * 2);
                }
                // Move tank forward
                if (IsKeyDown(KeyboardKey.KEY_W))
                {
                    playerTank.Accelerate(1);
                }
                // Move tank backwards
                if (IsKeyDown(KeyboardKey.KEY_S))
                {
                    playerTank.Accelerate(-1);
                }

                // Rotate tank turret to the the left
                if (IsKeyDown(KeyboardKey.KEY_Q))
                {
                    playerTank.RotateTurret(deltaTime * 2, -1);
                }
                // Rotate tank turret to the right
                if (IsKeyDown(KeyboardKey.KEY_E))
                {
                    playerTank.RotateTurret(deltaTime * 2, 1);
                }


                // Toggle debug
                debugCooldownCount += deltaTime;
                if (debugCooldownCount >= debugCooldown)
                {
                    // Turn on collision display wireframe view
                    if (IsKeyDown(KeyboardKey.KEY_F1))
                    {
                        IsDebugActive = IsDebugActive ? false : true;
                        debugCooldownCount = 0;
                    }

                    // Toggle on and off fps view
                    if (IsKeyDown(KeyboardKey.KEY_F2))
                    {
                        ShowFPS = ShowFPS ? false : true;
                        debugCooldownCount = 0;
                    }
                    // Toggle fps capped or not capped
                    if (IsKeyDown(KeyboardKey.KEY_F3))
                    {
                        if (FPSCapped)
                        {
                            FPSCapped = false;
                            SetTargetFPS(0);
                        }
                        else
                        {
                            FPSCapped = true;
                            SetTargetFPS(520);
                        }
                        debugCooldownCount = 0;
                    }
                }

            }

            // Check for restart key
            if (IsKeyDown(KeyboardKey.KEY_R) && GameOver)
            {
                TriggerAction("Restart");

            }
            
            // Call updates and destroys for each scene object
            CreateStoreUpdateDelegate();
            delegateUpdateStore?.Invoke(deltaTime);
            delegateDestroyStore?.Invoke();
            delegateDestroyStore = null;

            // Get mouse position
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            // Check each button for interaction
            foreach (Button button in buttons)
            {
                // Check for overlap
                button.OverlapButton(button.IsPointWithinButton(mouseX, mouseY));

                // If mouse click
                if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    string result = button.AttemptButtonClick(mouseX, mouseY);

                    // Trigger button action if it has one
                    if (result != "")
                    {
                        if (TriggerAction(result))
                        {
                            break;
                        }
                    }
                }
                // Update button
                button.Update(deltaTime);
            }
        }

        // Trigger an action (Return whether executions after this action should be avoided. e.g. Calling update button after closing game, leading to errors)
        public bool TriggerAction(string action)
        {
            // Exit main menu action
            if(action == "ExitMainMenu")
            {
                GameScene();
            }

            else if (action == "Save")
            {
                // Setup the file reader
                StreamReader reader = new StreamReader("Scores.txt");
                // Get all current text
                string text = reader.ReadToEnd();
                reader.Close();
                // Write new score to the end
                File.WriteAllText("Scores.txt", text + $"{saveInputBox.storedText}: {playerTank.points.ToString()}\n");
            }
            // Restart or close game action
            else if(action == "Restart" || action == "Close")
            {
                // Option checked in outside loop (Inside program.cs)
                GameEndOption = action;
                return true;
            }

            // If closing the window
            if(action == "Close")
            { 
                Raylib.CloseWindow();
                return true;
            }
            return false;
        }
        


        // Create enemy tanks
        public void CreateNewEnemy()
        {
            // Do not spawn if any of these conditions are met
            if(MainMenu || GameOver || !GameActive || enemies.Count >= 5)
            {
                return;
            }

            // Create new enemy
            Tank newEnemy = new Tank("Enemy");

            // Select random spawn point for enemy
            int randomSelection = gameRandom.Next(spawnPoints.Length);
            Vector3 randomSpawnPoint = spawnPoints[randomSelection];
            // Initialise enemy
            newEnemy.Init(randomSpawnPoint.x, randomSpawnPoint.y);  
        }


        // Draw function
        public void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.BLACK);

            // Draw game background
            background.Draw();

            // Draw the main menu
            if (MainMenu)
            {
                menuBackground.Draw();
                DrawText("Tank Game", 50, 50, 50, Color.ORANGE);
                DrawText("Made by Ben Wharton", 20, GetScreenHeight() - 30, 25, Color.ORANGE);

                int leaderboardSize = MeasureText("Leaderboard:", 50);
                DrawText("Leaderboard:", GetScreenWidth() - leaderboardSize - 120, 50, 50, Color.ORANGE);

                // Draw leaderboard scores
                for (int i = 1; i < scores.Count + 1; i++)
                {
                    string score = scores[i - 1];
                    int scoreSize = MeasureText($"{i} - {score}", 35);
                    DrawText($"{i} - {score}", GetScreenWidth() - 120 - leaderboardSize, 50 + (50 * i), 35, Color.ORANGE);
                }
            }

            // Draw scene objects
            foreach (SceneObject obj in sceneObjects)
            {
                if (obj.parent == null)
                {
                    obj.Draw();
                }
            }

            // Debug texts
            if (ShowFPS)
            {
                DrawText(fps.ToString() + $" ({(FPSCapped ? "Capped 520" : "Uncapped")}, F3 to {(FPSCapped ? "Uncap" : "Cap")})", 10, 10, 24, Color.BLUE);
            }
            if (IsDebugActive)
            {
                DrawText($"MouseX: {GetMouseX()}, MouseY: {GetMouseY()}", 10, 45, 24, Color.RED);
            }
            

            // Game over screen
            if (GameOver)
            {
                endGameBackground.Draw();

                int gameOverSize = MeasureText("Game Over!", 50);
                int pointsSize = MeasureText($"Points: {playerTank.points.ToString()}", 50);
                int saveSize = MeasureText("Add to Leaderboard:", 25);

                DrawText("Game Over!", (GetScreenWidth() / 2) - (gameOverSize / 2), (GetScreenHeight() / 2)-200, 50, Color.RED);
                DrawText($"Points: {playerTank.points.ToString()}", (GetScreenWidth() / 2) - (pointsSize / 2), (GetScreenHeight() / 2)-100, 50, Color.RED);
                DrawText($"Add to Leaderboard:", (GetScreenWidth() / 2) - (saveSize / 2), (GetScreenHeight() / 2)+55, 25, Color.RED);
            }


            // Draw lights
            // Draw dark background filter to simulate lack of light
            lightFilter.Draw();
            BeginBlendMode(BlendMode.BLEND_MULTIPLIED);
            // Remove areas from filter using multiply blend
            foreach (Light light in lights)
            {
                light.DrawLighting();
            }
            BeginBlendMode(BlendMode.BLEND_ALPHA);
            // Add colour to the removed areas for each light
            foreach (Light light in lights)
            {
                light.ApplyColour();
            }
            EndBlendMode();

            // Game screen text
            if (!MainMenu && !GameOver)
            {
                // Display fps
                DrawText($"Points: {playerTank.points.ToString()}", GetScreenWidth() - 200, 20, 24, Color.BLUE);
                int keyShowSize = MeasureText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space | Debug: F1, F2", 20);
                DrawText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space | Debug: F1, F2", (GetScreenWidth() / 2) - (keyShowSize / 2), GetScreenHeight() - 30, 20, Color.BLUE);

            }

            // Draw button objects
            foreach (SceneObject button in buttons)
            {
                button.Draw();
            }

            EndDrawing();
        }

    }
}
