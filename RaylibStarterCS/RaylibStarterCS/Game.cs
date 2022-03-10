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

        Tank playerTank = new Tank("Player");
        SpriteObject background = new SpriteObject();
        public static List<SceneObject> sceneObjects = new List<SceneObject>();

        public Game()
        {
        }

        bool initiated = false;
        public void Init()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            background.Load("./PNG/Environment/dirt.png");
            background.scale = 5;
        }

        public void Shutdown()
        {
        }

        public float shootCooldown = 0.5f;
        public float cooldownCount = 0.5f;

        public float trackCooldown = 0.5f;
        public float trackCooldownCount = 0.5f;
        public int trackCount = 0;

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


            // Shoot bullet
     
            if (IsKeyDown(KeyboardKey.KEY_SPACE))
            {
                playerTank.ShootBullet();
            }

          


            // Move and rotate the tank
            if (IsKeyDown(KeyboardKey.KEY_A))
            {
                playerTank.Rotate(-deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                playerTank.Rotate(deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                playerTank.MoveTank(deltaTime,1);  
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                playerTank.MoveTank(deltaTime, -1);
            }

            // Move barrrel
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                playerTank.RotateTurret(deltaTime, -1);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                playerTank.RotateTurret(deltaTime, 1);
            }

            playerTank.Update(deltaTime); 
        }



        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);
            background.Draw();

            // Display fps
            DrawText(fps.ToString(), 10, 10, 12, Color.RED);

            // Draw tank
            playerTank.Draw();

            EndDrawing();
        }

    }
}
