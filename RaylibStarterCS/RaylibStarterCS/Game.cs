using System;
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

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
        SpriteObject background = new SpriteObject();

        SpriteObject endGameBackground = new SpriteObject();
        SpriteObject menuBackground = new SpriteObject();

        public static List<SceneObject> sceneObjects;
        public static List<SceneObject> buttons;

        public List<Tank> enemies = new List<Tank>();

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

            int pbLength = 300;
            int pbHeight = 100;
            Button playButton = new Button( (GetScreenWidth()/2)-(pbLength/2), (GetScreenHeight() / 2) - (pbHeight / 2) - 50, pbLength, pbHeight, "Play Game", 22, Color.BLACK, "ExitMainMenu");
            InputBox test = new InputBox( (GetScreenWidth()/2)-(pbLength/2), (GetScreenHeight() / 2) - (pbHeight / 2) + 100, pbLength, pbHeight, "Play Game", 22, Color.BLACK, "ExitMainMenu");
        }


        float enemyCooldown = 5f;
        float enemyCooldownCount = 0f;
        public void Update()
        {

            if (GameActive && !MainMenu && !GameOver)
            { 
                if (!initiated)
                {
                    sceneObjects.Add(playerTank);
                    playerTank.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
                    initiated = true;
                }
                if (playerTank.waitingDestroy)
                {
                    EndGame();
                }


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
                if (obj.waitingDestroy)
                {
                    if (obj.tag == "Enemy")
                    {
                        enemies.Remove((Tank)obj);
                    }
                    waitingDetroy.Add(obj);
                }
            }
            // Destroy each object
            foreach (SceneObject obj in waitingDetroy)
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




            int mouseX = GetMouseX();
            int mouseY = GetMouseY();
            Button OverlappedButton = null;

            foreach (Button button in buttons)
            {
                if (button.IsPointWithinButton(mouseX, mouseY))
                {
                    button.OverlapButton(true);
                    OverlappedButton = button;
                    continue;
                }
                button.OverlapButton(false);
            }

            // Mouse click
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                if(OverlappedButton != null)
                {
                    string result = OverlappedButton.AttemptButtonClick(mouseX, mouseY);
                    if (result != "")
                    {
                        TriggerAction(result);
                    }
                }
                
            }

            if (IsKeyDown(KeyboardKey.KEY_R) && GameOver)
            {
                GameEndOption = "Restart";

            }
            if (IsKeyDown(KeyboardKey.KEY_ESCAPE))
            {
                Raylib.CloseWindow();
                GameEndOption = "Close";
            }
        }

        public void TriggerAction(string action)
        {
            if(action == "ExitMainMenu")
            {
                MainMenu = false;
                GameActive = true;
                sceneObjects = new List<SceneObject>();
                buttons = new List<SceneObject>();
            }
            else if(action == "Restart" || action == "Close")
            {
                GameEndOption = action;
            }
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
            sceneObjects = new List<SceneObject>();
            buttons = new List<SceneObject>();
            Button restartButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) - 100, (GetScreenHeight() / 2) - (pbHeight / 2) + 100, pbLength, pbHeight, "Restart", 22, Color.BLACK, "Restart");
            Button closeButton = new Button((GetScreenWidth() / 2) - (pbLength / 2) + 100, (GetScreenHeight() / 2) - (pbHeight / 2) + 100, pbLength, pbHeight, "Close", 22, Color.BLACK, "Close");

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
                sceneObjects.Add(newEnemy);
                enemies.Add(newEnemy);

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

            if (MainMenu)
            {
                menuBackground.Draw();

                int mainMenuTitleSize = MeasureText("Tank Game", 50);
                DrawText("Tank Game", (GetScreenWidth() / 2) - (mainMenuTitleSize / 2), 50, 50, Color.ORANGE);
            }

            // If game over
            if (GameOver)
            {
                endGameBackground.Draw();

                int gameOverSize = MeasureText("Game Over!", 50);
                int pointsSize = MeasureText($"Points: {playerTank.points.ToString()}", 50);
                int keySize = MeasureText("Press R to Restart or press ESC to close.", 25);

                DrawText("Game Over!", (GetScreenWidth() / 2) - (gameOverSize / 2), (GetScreenHeight() / 2) - 100, 50, Color.RED);
                DrawText($"Points: {playerTank.points.ToString()}", (GetScreenWidth() / 2) - (pointsSize / 2), (GetScreenHeight() / 2), 50, Color.RED);
                //DrawText("Press R to Restart or press ESC to close.", (GetScreenWidth() / 2) - (keySize / 2), (GetScreenHeight() / 2) + 50, 25, Color.RED);

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
