using System;
using System.Diagnostics;
using Raylib_cs;
using System.Collections.Generic;
using System.IO;
using MathClasses;
using System.Linq;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    class Game
    {
        Stopwatch stopwatch = new Stopwatch();

        private long currentTime = 0;
        private long lastTime = 0;
        private float timer = 0;
        private int fps = 1;
        private int frames;
        public static bool ShowFPS = false;
        public static bool FPSCapped = true;

        float enemyCooldown = 50000f;
        float enemyCooldownCount = 0f;

        float debugCooldown = 0.5f;
        float debugCooldownCount = 0f;
        public static bool IsDebugActive = false;
        



        public static float deltaTime = 0.005f;
        public static Vector3[] WorldBoundries = new Vector3[2];

        public static Tank playerTank;
        public static Tank mainMenuTank;

        SpriteObject background = new SpriteObject();
        SpriteObject endGameBackground = new SpriteObject();
        SpriteObject menuBackground = new SpriteObject();
        SpriteObject lightFilter = new SpriteObject();


        public static int gameLifetimeObjectCount;
        public static List<SceneObject> sceneObjects;
        public static List<SceneObject> buttons;
        public static List<Tank> enemies = new List<Tank>();
        public static List<Light> lights = new List<Light>();

        InputBox saveInputBox;
        List<string> scores = new List<string>();

        public delegate void DelegateUpdate(float deltaTime);
        public DelegateUpdate delegateUpdates;

        public delegate void DelegateDestroy();
        public DelegateDestroy delegateDestroy;

        public static Random gameRandom = new Random();

        public bool GameActive = false;
        public bool MainMenu = true;
        public bool GameOver = false;

        public string GameEndOption = "";

        public Game()
        {
        }

        public void Init(int width, int height)
        {
            SetWindowSize(width, height);
            WorldBoundries = new Vector3[] { new Vector3(GetScreenWidth(), GetScreenHeight(), 0), new Vector3(0, 0, 0) };


            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            background.Load("./PNG/Environment/dirt.png");
            background.textureScale = 10;

            endGameBackground.Load("./PNG/Environment/TitleBackground.png");
            endGameBackground.textureScale = 5;
            
            menuBackground.Load("./PNG/Environment/rock.png");
            menuBackground.textureScale = 10;

            lightFilter.Load("./PNG/Environment/black.png");
            lightFilter.colour = ColorAlpha(Color.BLACK, .5f);
            lightFilter.textureScale = 10;

            playerTank = new Tank("Player");
            sceneObjects = new List<SceneObject>();
            buttons = new List<SceneObject>();
            lights = new List<Light>();
            MainMenuScene();

        }

        // Setup Main menu scene
        public void MainMenuScene()
        {
            MainMenu = true;
            GameActive = false;

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
            scores = scores.OrderByDescending(s => int.Parse(s.Split(": ", StringSplitOptions.None)[1])).ToList();


            mainMenuTank = new Tank("Menu");
   
            mainMenuTank.Init(GetScreenWidth()/2, GetScreenHeight()/2);
            mainMenuTank.Scale(0.5f, 0.5f);


        }
        public void EndGameScene()
        {
            GameOver = true;
            GameActive = false;

            int pbLength = 150;
            int pbHeight = 50;

            int ibLength = 200;
            int ibHeight = 25;

            int sbLength = 75;
            int sbHeight = 25;

            sceneObjects = new List<SceneObject>();
            buttons = new List<SceneObject>();
            lights = new List<Light>();


            Button restartButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) - 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Restart", 22, Color.BLACK, "Restart");
            Button closeButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) + 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Close", 22, Color.BLACK, "Close");

            saveInputBox = new InputBox((GetScreenWidth() / 2) - (ibLength / 2), (GetScreenHeight() / 2) - (ibHeight / 2) + 100, ibLength, ibHeight, "Type Name", 20, Color.BLACK, "");

            Button saveButton = new Button((GetScreenWidth() / 2) - (sbLength / 2), (GetScreenHeight() / 2) - (sbHeight / 2) + 150, sbLength, sbHeight, "Save", 16, Color.BLACK, "Save", true);
        }

        public void GameScene()
        {
            MainMenu = false;
            GameActive = true;
            sceneObjects = new List<SceneObject>();
            buttons = new List<SceneObject>();

            // Initiate player
            playerTank.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);

            new GameMap();
         
        }

        // Update game
        public void Update()
        {
            // Calculate the deltatime for update
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
                    playerTank.Accelerate(deltaTime * 2, 1);
                }
                // Move tank backwards
                if (IsKeyDown(KeyboardKey.KEY_S))
                {
                    playerTank.Accelerate(deltaTime * 1.5f, -1);
                }

                // Rotate tank turret to the the left
                if (IsKeyDown(KeyboardKey.KEY_Q))
                {
                    playerTank.RotateTurret(deltaTime * 2, -1);
                }
                // Rotate tank turret to the left
                if (IsKeyDown(KeyboardKey.KEY_E))
                {
                    playerTank.RotateTurret(deltaTime * 2, 1);
                }

                debugCooldownCount += deltaTime;
                // Toggle debug
                if(debugCooldownCount >= debugCooldown)
                {
                    if (IsKeyDown(KeyboardKey.KEY_F1))
                    {
                        IsDebugActive = IsDebugActive ? false : true;
                        debugCooldownCount = 0;
                    }
                    if (IsKeyDown(KeyboardKey.KEY_F2))
                    {
                        ShowFPS = ShowFPS ? false : true;
                        debugCooldownCount = 0;
                    }
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
            // Find all scene objects that are waiting to destroy
            delegateDestroy = null;  
            foreach (SceneObject obj in sceneObjects)
            {
                if (obj.isWaitingDestroy)
                {
                    // If object is enemy, also remove it from enemies list
                    if (obj.tag == "Enemy")
                    {
                        enemies.Remove((Tank)obj);
                    }
                    delegateDestroy += obj.RemoveSelfFromSceneObjects;
                }
            }
            // Add each button awaiting destroy
            foreach (Button button in buttons)
            {
                if (button.isWaitingDestroy)
                {
                    delegateDestroy += button.RemoveSelfFromSceneObjects;
                }
            }
            delegateDestroy?.Invoke();
           

            delegateUpdates = null;
            // Update scene objects (Stored in a delegate to avoid an error if object is destroyed during it's update)
            foreach (SceneObject obj in sceneObjects)
            {
                delegateUpdates += (deltaTime) => obj.Update(deltaTime);
            }
            foreach (Button button in buttons)
            {
                delegateUpdates += (deltaTime) => button.Update(deltaTime);
            }
            delegateUpdates?.Invoke(deltaTime);



            // Test for mouse click and position
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            // check each button for interaction
            foreach (Button button in buttons)
            {
                // Check for overlap
                button.OverlapButton(button.IsPointWithinButton(mouseX, mouseY));

                // If mouse click
                if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    string result = button.AttemptButtonClick(mouseX, mouseY);

                    // Triger button action if it has one
                    if (result != "")
                    {
                        TriggerAction(result);
                    }
                }

                
            }

         
            // Check for restart key
            if (IsKeyDown(KeyboardKey.KEY_R) && GameOver)
            {
                TriggerAction("Restart");

            }
            // Check for close window key
            if (IsKeyDown(KeyboardKey.KEY_ESCAPE))
            {
                TriggerAction("Close");
            }
        }

        // Trigger an action
        public void TriggerAction(string action)
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
            }

            // If closing the window
            if(action == "Close")
            { 
                Raylib.CloseWindow();
            }
        }
        


        // Create enemy tanks
        public void CreateNewEnemy()
        {
            // Do not spawn if any of these conditions are met
            if(MainMenu || GameOver || !GameActive)
            {
                return;
            }

            if(enemies.Count < 5)
            {
                // Create new enemy
                Tank newEnemy = new Tank("Enemy");

                Vector3[] spawnPoints = new Vector3[] { new Vector3(180, 380, 0), new Vector3(710, 615, 0), new Vector3(1050, 100, 0), new Vector3(1050, 280, 0) };
                int randomSelection = gameRandom.Next(spawnPoints.Length);
                Vector3 randomSpawnPoint = spawnPoints[randomSelection];
                newEnemy.Init(randomSpawnPoint.x, randomSpawnPoint.y);

                return;
            }
            
        }


        // Draw function
        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.BLACK);

            
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


            if (ShowFPS)
            {
                DrawText(fps.ToString() + $" ({(FPSCapped ? "Capped 520" : "Uncapped")}, F3 to {(FPSCapped ? "Uncap" : "Cap")})", 10, 10, 24, Color.RED);
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

            // Game screen
            if (!MainMenu && !GameOver)
            {
                // Display fps
                DrawText($"Points: {playerTank.points.ToString()}", GetScreenWidth() - 200, 20, 24, Color.BLUE);
                int keyShowSize = MeasureText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space | Debug: F1, F2", 20);
                DrawText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space | Debug: F1, F2", (GetScreenWidth() / 2) - (keyShowSize / 2), GetScreenHeight() - 30, 20, Color.BLUE);

            }

            // Draw ui objects
            foreach (SceneObject ui in buttons)
            {
                ui.Draw();
            }

            lightFilter.Draw();

            // Draw lights
            BeginBlendMode(BlendMode.BLEND_MULTIPLIED);
            foreach(Light light in lights)
            {
                light.DrawLighting();
            }
            EndBlendMode();
            foreach (Light light in lights)
            {
                light.ApplyColour();
            }
            EndDrawing();
        }

    }
}
