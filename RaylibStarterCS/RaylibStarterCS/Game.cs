using System;
using System.Diagnostics;
using Raylib_cs;
using System.Collections.Generic;
using System.IO;
using MathClasses;
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

        private float deltaTime = 0.005f;
        public Vector3[] sceneBoundries = new Vector3[2] {new Vector3(GetScreenWidth() / 2, GetScreenHeight() / 2, 0), new Vector3(-GetScreenWidth() / 2, -GetScreenHeight() / 2, 0) };

        public static Tank playerTank;
        public static Tank mainMenuTank;

        SpriteObject background = new SpriteObject();

        SpriteObject endGameBackground = new SpriteObject();
        SpriteObject menuBackground = new SpriteObject();

        InputBox saveInputBox;
        List<string> scores = new List<string>();

        public static List<SceneObject> sceneObjects;
        public static List<SceneObject> buttons;

        public static List<Tank> enemies = new List<Tank>();

        public delegate void DelegateUpdate(float deltaTime);
        public DelegateUpdate delegateUpdates;

        Random random = new Random();

        public bool GameActive = false;
        public bool MainMenu = true;
        public bool GameOver = false;

        public string GameEndOption = "";

        public Game()
        {
        }

        bool initiated = false;
        public void Init()
        {
            SetWindowSize(900, 600);

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
                scores.Add(reader.ReadLine());
            }
            reader.Close();


            mainMenuTank = new Tank("Menu");
   
            mainMenuTank.Init(GetScreenWidth()/2, GetScreenHeight()/2);
            mainMenuTank.Scale(0.5f, 0.5f);


        }


        float enemyCooldown = 5f;
        float enemyCooldownCount = 0f;
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
                if (!initiated)
                {
                    playerTank.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);

                    for(int i = 0; i < 5; i++)
                    {
                        int randomX = random.Next(100, GetScreenWidth() - 100);
                        int randomY = random.Next(100, GetScreenHeight() - 100);
                        float dist = MathF.Sqrt(Math.Abs(randomY - randomX) + Math.Abs(playerTank.GlobalTransform.m21 - playerTank.GlobalTransform.m20));

                        
                        if (dist > 20)
                        {
                            SpriteObject barrelSprite = new SpriteObject();
                            SceneObject barrel = new SceneObject();
                            barrelSprite.Load("./PNG/Obstacles/barrelGreen_up.png");
                            barrel.AddChild(barrelSprite);
                            barrel.hasCollision = true;
                            barrel.tag = "CollideAll";

                            barrel.SetPosition(randomX, randomY);
                            barrel.HitRadius = 3f;
                            barrelSprite.SetPosition(-(barrelSprite.Width / 2), -(barrelSprite.Height / 2));
                            sceneObjects.Add(barrel);
                        }
                    }
                    initiated = true;
                }
                if (playerTank.isWaitingDestroy)
                {
                    EndGame();
                }



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

                // Move and rotate the tank
                if (IsKeyDown(KeyboardKey.KEY_A))
                {
                    playerTank.Rotate(-deltaTime * 2);
                }
                if (IsKeyDown(KeyboardKey.KEY_D))
                {
                    playerTank.Rotate(deltaTime * 2);
                }
                if (IsKeyDown(KeyboardKey.KEY_W))
                {
                    playerTank.MoveTank(deltaTime * 2, 1);
                }
                if (IsKeyDown(KeyboardKey.KEY_S))
                {
                    playerTank.MoveTank(deltaTime * 1.5f, -1);
                }

                // Move barrrel
                if (IsKeyDown(KeyboardKey.KEY_Q))
                {
                    playerTank.RotateTurret(deltaTime * 2, -1);
                }
                if (IsKeyDown(KeyboardKey.KEY_E))
                {
                    playerTank.RotateTurret(deltaTime * 2, 1);
                }

               
            }
            // Find all scene objects that are waiting to destroy
            List<SceneObject> waitingDetroy = new List<SceneObject>();
            foreach (SceneObject obj in sceneObjects)
            {
                if (obj.isWaitingDestroy)
                {
                    if (obj.tag == "Enemy")
                    {
                        enemies.Remove((Tank)obj);
                    }
                    waitingDetroy.Add(obj);
                }
            }
            // Add each button awaiting destroy
            foreach (Button button in buttons)
            {
                if (button.isWaitingDestroy)
                {
                    waitingDetroy.Add(button);
                }
            }

            // Destroy each object
            foreach (var obj in waitingDetroy)
            {
                obj.RemoveSelfFromSceneObjects();
            }



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


            foreach (Button button in buttons)
            {
                button.OverlapButton(button.IsPointWithinButton(mouseX, mouseY));

                // If mouse click
                if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    string result = button.AttemptButtonClick(mouseX, mouseY);
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

        // Trigger an action (Used by buttons)
        public void TriggerAction(string action)
        {
            // Exit main menu action
            if(action == "ExitMainMenu")
            {
                MainMenu = false;
                GameActive = true;
                sceneObjects = new List<SceneObject>();
                buttons = new List<SceneObject>();

                


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
                GameEndOption = action;
            }

            // If closing the window
            if(action == "Close")
            { 
                Raylib.CloseWindow();
            }
        }


        public void ShutDown()
        {
            sceneObjects.Clear();
            buttons.Clear();
        }

        public void EndGame()
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

            new Button((GetScreenWidth() / 2) - (pbLength / 2) - 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Restart", 22, Color.BLACK, "Restart");
            new Button((GetScreenWidth() / 2) - (pbLength / 2) + 100, (GetScreenHeight() / 2) - (pbHeight / 2), pbLength, pbHeight, "Close", 22, Color.BLACK, "Close");
            
            saveInputBox = new InputBox((GetScreenWidth() / 2) - (ibLength / 2), (GetScreenHeight() / 2) - (ibHeight / 2) + 100, ibLength, ibHeight, "Type Name", 20, Color.BLACK, "");
            
            new Button((GetScreenWidth() / 2) - (sbLength / 2), (GetScreenHeight() / 2) - (sbHeight / 2) + 150, sbLength, sbHeight, "Save", 16, Color.BLACK, "Save", true);

        }


        // Create enemy tanks
        public void CreateNewEnemy()
        {
            if(MainMenu || GameOver || !GameActive)
            {
                return;
            }
            if(enemies.Count < 5)
            {
                Tank newEnemy = new Tank("Enemy");

                for(int attempts = 0; attempts < 100; attempts++)
                {
                    int randomX = random.Next(20, GetScreenWidth() - 20);
                    int randomY = random.Next(20, GetScreenHeight() - 20);
                    float dist = MathF.Sqrt(Math.Abs(randomY - randomX) + Math.Abs(playerTank.GlobalTransform.m21 - playerTank.GlobalTransform.m20));

                    if (dist > 30)
                    {
                        newEnemy.Init(randomX, randomY);
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
                DrawText("Leaderboard:", GetScreenWidth() - leaderboardSize - 50, 50, 50, Color.ORANGE);
                
                for(int i = 1; i < scores.Count+1; i++)
                {
                    string score = scores[i-1];
                    int scoreSize = MeasureText($"{i} - {score}", 35);
                    DrawText($"{i} - {score}", GetScreenWidth() - leaderboardSize, 50+(50*i), 35, Color.ORANGE);
                }
            }

            // If game over
            if (GameOver)
            {
                endGameBackground.Draw();

                int gameOverSize = MeasureText("Game Over!", 50);
                int pointsSize = MeasureText($"Points: {playerTank.points.ToString()}", 50);
                int saveSize = MeasureText("Add to Leaderboard:", 25);

                DrawText("Game Over!", (GetScreenWidth() / 2) - (gameOverSize / 2), (GetScreenHeight() / 2)-200, 50, Color.RED);
                DrawText($"Points: {playerTank.points.ToString()}", (GetScreenWidth() / 2) - (pointsSize / 2), (GetScreenHeight() / 2)-100, 50, Color.RED);
                DrawText($"Add to Leaderboard:", (GetScreenWidth() / 2) - (saveSize / 2), (GetScreenHeight() / 2)+35, 25, Color.RED);


            }

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
