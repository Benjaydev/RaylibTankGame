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

        public static Tank playerTank = new Tank("Player");
        SpriteObject background = new SpriteObject();
        public static List<SceneObject> sceneObjects = new List<SceneObject>();
        public List<Tank> enemies = new List<Tank>();

        public delegate void DelegateUpdate(float deltaTime);
        public DelegateUpdate delegateUpdates;

        Random random = new Random();

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
            background.scale = 10;
        }

        public void Shutdown()
        {
        }

        float enemyCooldown = 5f;
        float enemyCooldownCount = 0f;
        public void Update()
        {
            if (!initiated)
            {
                sceneObjects.Add(playerTank);
                playerTank.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
                initiated = true;
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
            if(enemyCooldownCount >= enemyCooldown)
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
                playerTank.Rotate(-deltaTime*2);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                playerTank.Rotate(deltaTime*2);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                playerTank.MoveTank(deltaTime*2,1);  
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                playerTank.MoveTank(deltaTime*1.5f, -1);
            }

            // Move barrrel
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                playerTank.RotateTurret(deltaTime*2, -1);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                playerTank.RotateTurret(deltaTime*2, 1);
            }

            // Find all scene objects that are waiting to destroy
            List<SceneObject> waitingDetroy = new List<SceneObject>();
            foreach (SceneObject obj in sceneObjects)
            {
                if (obj.waitingDestroy)
                {
                    if(obj.tag == "Enemy")
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
            delegateUpdates?.Invoke(deltaTime);
        }

        // Create enemy tanks
        public void CreateNewEnemy()
        {
            if(enemies.Count < 1)
            {
                Tank newEnemy = new Tank("Enemy");
                sceneObjects.Add(newEnemy);
                enemies.Add(newEnemy);
                newEnemy.Init(random.Next(20, GetScreenWidth()-20), random.Next(20, GetScreenHeight() - 20));
                //newEnemy.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
            }
            
        }

        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);
            background.Draw();

            // Display fps
            DrawText(fps.ToString(), 10, 10, 12, Color.RED);


            DrawText($"Points: {playerTank.points.ToString()}", GetScreenWidth()-200, 20, 24, Color.BLUE);
                
            // Draw tank
            foreach (SceneObject obj in sceneObjects)
            {
                obj.Draw();
            }
            //playerTank.Draw();
            //enemyTank.Draw();

            EndDrawing();
        }

    }
}
