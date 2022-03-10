using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    public class Tank : SceneObject
    {
        public SceneObject turretObject = new SceneObject();
        public SceneObject firePoint = new SceneObject();
        public SceneObject trackPoint = new SceneObject();

        public SpriteObject tankSprite = new SpriteObject();
        public SpriteObject turretSprite = new SpriteObject();


        public List<BulletObject> bullets = new List<BulletObject>();
        public List<BulletObject> removeBullets = new List<BulletObject>();

        public List<SpriteObject> tracks = new List<SpriteObject>();

        public float shootCooldown = 0.5f;
        public float shootCooldownCount = 0.5f;

        public float trackCooldown = 0.5f;
        public float trackCooldownCount = 0.5f;
        public int maxTrackCount = 15;


        public Tank()
        {
        }

        // Initiate tank
        public void Init(float xPos = 0, float yPos = 0)
        {
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
            firePoint.SetPosition(turretSprite.Height + 30, (turretSprite.Width / 2.0f) - 17.5f);

            trackPoint.SetRotate(-90 * (float)(Math.PI));
            trackPoint.SetPosition(tankSprite.Height - 35, -(tankSprite.Width / 2));

            // Set up the scene object hierarchy - Add turretSprite (And firePoint) to turretObject,
            // then add tankSprite, turretObject, and trackPoint to tankObject.
            turretObject.AddChild(turretSprite);
            turretObject.AddChild(firePoint);
            AddChild(tankSprite);
            AddChild(turretObject);
            AddChild(trackPoint);

            SetPosition(xPos, yPos);
        }

        // Call on update everytime tank scene object is updated
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            // Check if shooting cooldown is not complete
            if (shootCooldownCount <= shootCooldown)
            {
                shootCooldownCount += deltaTime;
            }
            // Check if track cooldown is not complete
            if (trackCooldownCount <= trackCooldown)
            {
                trackCooldownCount += deltaTime;
            }

            // Update each active bullet
            foreach (BulletObject bullet in bullets)
            {
                Vector3 f = bullet.ForwardVector * deltaTime;
                // Check if bullet has finished
                if (bullet.UpdateBullet(f, deltaTime))
                {
                    // Keep track of bullets needing to be removed
                    removeBullets.Add(bullet);
                }
            }

            // Remove needed bullets
            foreach (BulletObject bullet in removeBullets)
            {
                bullets.Remove(bullet);
            }
        }

        // Draw function
        public override void OnDraw()
        {
            base.OnDraw();
            // Draw each bullet
            foreach (BulletObject bullet in bullets)
            {
                bullet.Draw();
            }
            // Draw each track
            foreach (SpriteObject track in tracks)
            {
                track.Draw();
            }
        }

        // Function used to move tank in direction adjusted by delta time
        public void MoveTank(float deltaTime, float direction = 1f)
        {
            // Find the facing direction of tank
            Vector3 facing = new Vector3(LocalTransform.m00, LocalTransform.m01, 1);
            // Create a new track
            MakeTrack(facing);
            // Calculate the direction and time adjusted movement vector
            Vector3 f = facing * (direction*100f) * deltaTime;
            // Translate position
            Translate(f.x, f.y);
        }

        // Rotate tank turret adjusted by delta time
        public void RotateTurret(float deltaTime, float direction = 1f)
        {
            turretObject.Rotate(deltaTime*direction);
        }

        // Shoot bullet
        public void ShootBullet()
        {
            // If the shooting cooldown is complete
            if(shootCooldownCount >= shootCooldown)
            {
                // FInd the direction that the turret is facing
                Vector3 turretFacing = new Vector3(firePoint.GlobalTransform.m00, firePoint.GlobalTransform.m01, 1);
                // Create new bullet
                BulletObject newbullet = new BulletObject(turretFacing);
                // Set position of new bullet to the tank fire point
                newbullet.SetPosition(firePoint.GlobalTransform.m20, firePoint.GlobalTransform.m21);
                // Add to bullet list to keep track of it
                bullets.Add(newbullet);

                // Reset cooldown
                shootCooldownCount = 0f;
            }
            
        }


        // Make tank track
        public void MakeTrack(Vector3 facing)
        {
            // If track cooldown is complete
            if (trackCooldownCount >= trackCooldown)
            {
                // Create new track sprite
                SpriteObject track = new SpriteObject();
                track.Load("./PNG/Tanks/tracksSmall.png");

                // Rotate sprite to face same direction as tank
                track.SetRotate(90 * (float)(Math.PI / 180.0f));
                track.Rotate(MathF.Atan2(facing.y, facing.x));
                // Set position of track to track spawn point
                track.SetPosition(trackPoint.GlobalTransform.m20, trackPoint.GlobalTransform.m21);
                // Add to tracks list to keep track of it
                tracks.Add(track);
                // Reset cooldown
                trackCooldownCount = 0f;

                // Remove tracks after reaching maximum amount
                if (tracks.Count >= maxTrackCount)
                {
                    tracks.RemoveAt(0);
                }
            }
        }
    }
}
