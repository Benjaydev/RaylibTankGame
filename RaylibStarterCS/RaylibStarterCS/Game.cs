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

        Tank tankObject = new Tank();
        //SceneObject turretObject = new SceneObject();
        //SceneObject firePoint = new SceneObject();
        //SceneObject trackPoint = new SceneObject();

        //SpriteObject tankSprite = new SpriteObject();
        //SpriteObject turretSprite = new SpriteObject();

        List<Smoke> smoke = new List<Smoke>();
        List<Smoke> removeSmoke = new List<Smoke>();
    

        public Game()
        {
        }

        bool initiated = false;
        public void Init()
        {

            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            

            /*
            tankSprite.Load("./PNG/Tanks/tankRed_outline.png");
            // Setup the tank barrel starting rotation
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of turret to be centered in tank sprite
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            turretSprite.Load("./PNG/Tanks/barrelBlack_outline.png");
            // Setup the tank base starting rotation
            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of base to be centered in turret sprite
            turretSprite.SetPosition(0, turretSprite.Width / 2.0f);

            firePoint.SetRotate(-90 * (float)(Math.PI));
            firePoint.SetPosition(turretSprite.Height+30, (turretSprite.Width / 2.0f)-17.5f);

            trackPoint.SetRotate(-90 * (float)(Math.PI));
            trackPoint.SetPosition(tankSprite.Height-35, -(tankSprite.Width/2));

            // Set up the scene object hierarchy - Add turretSprite to turretObject, then add tankSprite and turretObject to tankObject
            turretObject.AddChild(turretSprite);
            turretObject.AddChild(firePoint);
            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);
            tankObject.AddChild(trackPoint);

            // Now tankObject position can be changed without effecting the children
            tankObject.SetPosition(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);*/
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
                tankObject = new Tank();
                tankObject.Init(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
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
                tankObject.ShootBullet();
            }

          


            // Move and rotate the tank
            if (IsKeyDown(KeyboardKey.KEY_A))
            {
                tankObject.Rotate(-deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                tankObject.Rotate(deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                tankObject.MoveTank(deltaTime,1);  
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                tankObject.MoveTank(deltaTime, -1);
            }

            // Move barrrel
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                tankObject.RotateTurret(deltaTime, -1);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                tankObject.RotateTurret(deltaTime, 1);
            }

            tankObject.Update(deltaTime); 
        }



        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);

            // Display fps
            DrawText(fps.ToString(), 10, 10, 12, Color.RED);

            // Draw tank
            tankObject.Draw();

            EndDrawing();
        }

    }
}
