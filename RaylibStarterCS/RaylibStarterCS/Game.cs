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

        float enemyCooldown = 50000f;
        float enemyCooldownCount = 0f;

        private float deltaTime = 0.005f;
        public Vector3[] sceneBoundries = new Vector3[2] {new Vector3(GetScreenWidth() / 2, GetScreenHeight() / 2, 0), new Vector3(-GetScreenWidth() / 2, -GetScreenHeight() / 2, 0) };

        public static Tank playerTank;
        public static Tank mainMenuTank;

        SpriteObject background = new SpriteObject();
        SpriteObject endGameBackground = new SpriteObject();
        SpriteObject menuBackground = new SpriteObject();

        public static List<SceneObject> sceneObjects;
        public static List<SceneObject> buttons;
        public static List<Tank> enemies = new List<Tank>();

        InputBox saveInputBox;
        List<string> scores = new List<string>();

        public delegate void DelegateUpdate(float deltaTime);
        public DelegateUpdate delegateUpdates;

        public delegate void DelegateDestroy();
        public DelegateDestroy delegateDestroy;

        Random random = new Random();

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

            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            background.Load("./PNG/Environment/dirt.png");
            background.textureScale = 10;

            endGameBackground.Load("./PNG/Environment/TitleBackground.png");
            endGameBackground.textureScale = 5;
            
            menuBackground.Load("./PNG/Environment/rock.png");
            menuBackground.textureScale = 10;

            playerTank = new Tank("Player");
            sceneObjects = new List<SceneObject>();
            buttons = new List<SceneObject>();


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

            // Setup barrel obstacles
            for (int i = 0; i < 10; i++)
            {
                // Choose random point on screen
                int randomX = random.Next(100, GetScreenWidth() - 100);
                int randomY = random.Next(100, GetScreenHeight() - 100);

                // Calculate the distance between the player and randomised point to avoid objects spawing inside player
                // Setup barrel sprite
                SpriteObject barrelSprite = new SpriteObject();
                barrelSprite.Load("./PNG/Obstacles/barrelGreen_up.png");
                barrelSprite.SetPosition(-(barrelSprite.Width / 2), -(barrelSprite.Height / 2));

                // Setup barrel object
                SceneObject barrel = new SceneObject();
                barrel.AddChild(barrelSprite);
                barrel.hasCollision = true;
                barrel.tag = "CollideAll";
                barrel.movable = true;
                barrel.SetPosition(randomX, randomY);
                barrel.HitRadius = 25;

                barrel.SeperateIntersectingObjects(new List<string>() { "CollideAll", "Player" } );
                // Add to scene
                sceneObjects.Add(barrel);
            }
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
                    playerTank.MoveTank(deltaTime * 2, 1);
                }
                // Move tank backwards
                if (IsKeyDown(KeyboardKey.KEY_S))
                {
                    playerTank.MoveTank(deltaTime * 1.5f, -1);
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

            
            if(enemies.Count < 100)
            {
                // Create new enemy
                Tank newEnemy = new Tank("Enemy");

                // Attempt to spawn
                for(int attempts = 0; attempts < 100; attempts++)
                {
                    int randomX = random.Next(20, GetScreenWidth() - 20);
                    int randomY = random.Next(20, GetScreenHeight() - 20);

                    
                    if (!playerTank.IsCollidingWithObject(newEnemy))
                    {
                        newEnemy.Init(randomX, randomY);
                        newEnemy.SeperateIntersectingObjects(new List<string> { "CollideAll", "Player" });
                        return;
                    }
                    
                }
                newEnemy.Init(20, 20);
            }
            
        }

        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);

            
            background.Draw();

            // Draw the main menu
            if (MainMenu)
            {
                menuBackground.Draw();     
                DrawText("Tank Game", 50 , 50, 50, Color.ORANGE);
                DrawText("Made by Ben Wharton", 20, GetScreenHeight() - 30, 25, Color.ORANGE);

                int leaderboardSize = MeasureText("Leaderboard:", 50);
                DrawText("Leaderboard:", GetScreenWidth() - leaderboardSize - 120, 50, 50, Color.ORANGE);
                
                // Draw leaderboard scores
                for(int i = 1; i < scores.Count+1; i++)
                {
                    string score = scores[i-1];
                    int scoreSize = MeasureText($"{i} - {score}", 35);
                    DrawText($"{i} - {score}", GetScreenWidth() - 120 - leaderboardSize , 50+(50*i), 35, Color.ORANGE);
                }
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
                DrawText(fps.ToString(), 10, 10, 24, Color.RED);
                DrawText($"Points: {playerTank.points.ToString()}", GetScreenWidth() - 200, 20, 24, Color.BLUE);
                int keyShowSize = MeasureText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space", 20);
                DrawText("Move: W,S | Rotate Tank: A,D | Rotate Turret: Q,E | Shoot: Space", (GetScreenWidth() / 2) - (keyShowSize / 2), GetScreenHeight() - 30, 20, Color.BLUE);

            }


            // Draw scene objects
            foreach (SceneObject obj in sceneObjects)
            {
                obj.Draw();
            }

            // Draw ui objects
            foreach (SceneObject ui in buttons)
            {
                ui.Draw();
            }

            EndDrawing();
        }

    }
}
