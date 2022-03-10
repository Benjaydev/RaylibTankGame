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

        SceneObject tankObject = new SceneObject();
        SceneObject turretObject = new SceneObject();
        SceneObject firePoint = new SceneObject();

        SpriteObject tankSprite = new SpriteObject();
        SpriteObject turretSprite = new SpriteObject();

        List<BulletObject> bullets = new List<BulletObject>();
        List<BulletObject> removeBullets = new List<BulletObject>();
        List<Smoke> smoke = new List<Smoke>();
        List<Smoke> removeSmoke = new List<Smoke>();

        public Game()
        {
        }

        public void Init()
        {

            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            tankSprite.Load("./PNG/Tanks/tankBlue_outline.png");
            // Setup the tank barrel starting rotation
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of turret to be centered in tank sprite
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            turretSprite.Load("./PNG/Tanks/barrelBlue.png");
            // Setup the tank base starting rotation
            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of base to be centered in turret sprite
            turretSprite.SetPosition(0, turretSprite.Width / 2.0f);

            firePoint.SetRotate(-90 * (float)(Math.PI));
            firePoint.SetPosition(turretSprite.Height+30, (turretSprite.Width / 2.0f)-17.5f);

            // Set up the scene object hierarchy - Add turretSprite to turretObject, then add tankSprite and turretObject to tankObject
            turretObject.AddChild(turretSprite);
            turretObject.AddChild(firePoint);
            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);

            // Now tankObject position can be changed without effecting the children
            tankObject.SetPosition(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
        }

        public void Shutdown()
        {
        }

        public float shootCooldown = 0.5f;
        public float cooldownCount = 0.5f;
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

            if(cooldownCount <= shootCooldown)
            {
                cooldownCount += deltaTime;
            }

            // Shoot bullet
            Vector3 facing = new Vector3(tankObject.LocalTransform.m00, tankObject.LocalTransform.m01, 1);
            if (IsKeyDown(KeyboardKey.KEY_SPACE) && cooldownCount >= shootCooldown)
            {
                Vector3 turretFacing = new Vector3(firePoint.GlobalTransform.m00, firePoint.GlobalTransform.m01, 1);
                BulletObject newbullet = new BulletObject(turretFacing);
                newbullet.SetPosition(firePoint.GlobalTransform.m20, firePoint.GlobalTransform.m21);
                bullets.Add(newbullet);

                cooldownCount = 0f;
            }

            foreach(BulletObject bullet in bullets)
            {
                Vector3 f = bullet.ForwardVector * deltaTime;
                if(bullet.UpdateBullet(f, deltaTime))
                {
                    removeBullets.Add(bullet);
                }
            }
            foreach(BulletObject bullet in removeBullets)
            {
                bullets.Remove(bullet);
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
                Vector3 f = facing * 100 * deltaTime;
                tankObject.Translate(f.x, f.y);
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                Vector3 f = facing * -100 * deltaTime;
                tankObject.Translate(f.x, f.y);
            }

            // Move barrrel
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                turretObject.Rotate(-deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                turretObject.Rotate(deltaTime);
            }


            tankObject.Update(deltaTime);
            foreach (BulletObject bullet in bullets)
            {
                bullet.Update(deltaTime);
            }
            // Update smoke
            foreach (Smoke s in smoke)
            {
                if (s.Update(deltaTime))
                {
                    removeSmoke.Add(s);
                }
            }
            // Remove all finished smoke
            foreach (Smoke rs in removeSmoke)
            {
                smoke.Remove(rs);
            }
        }

        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);

            // Display fps
            DrawText(fps.ToString(), 10, 10, 12, Color.RED);

            // Draw tank
            tankObject.Draw();

            foreach(BulletObject bullet in bullets)
            {
                bullet.Draw();
            }
            // Draw smoke
            foreach (Smoke s in smoke)
            {
                s.Draw();
            }
           
            removeSmoke = new List<Smoke>();

            EndDrawing();
        }

    }
}
