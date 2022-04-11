using System;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathClasses;

namespace RaylibStarterCS
{
    public class Tank : SceneObject
    {
        public SceneObject turretObject = new SceneObject();
        public SceneObject firePoint = new SceneObject();
        public SceneObject trackPoint = new SceneObject();

        public SpriteObject tankSprite = new SpriteObject();
        public SpriteObject turretSprite = new SpriteObject();
        public Texture2D bulletTexture;

        // Movement values for tank
        public float maxVelocity = 200f;
        public float velocity = 0f;
        public float movingDirection = 0f;
        public float acceleration = 100f;
        public float deceleration = 300;
        public float turretMoveSpeed = 1f;
        public float turnSpeed = 1f;

        // Shooting
        public float shootSpeed = 1000f;
        public float shootingAccuracy = .8f;
        public float shootCooldown = 1f;
        public float shootCooldownCount = 0f;

        // Tracks
        public float trackCooldown = 0.35f;
        public float trackCooldownCount = 0.35f;
        // Keep track of last track spawn point
        public Vector3 lastTrackPoint = new Vector3(0, 0, 0);
        // Distance allowed until next track spawn
        public float newTrackRadius = 100f;

        // Enemy AI
        bool AICanTurn = true;
        bool AICanMove = true;
        bool AITurretTracksPlayer = true;

        bool AIturning = true;
        bool AImoving = true;
        float AImovingDirection = 1f;
        float AIturningDirection = 1f;

        // Save points (for player)
        public int points = 0;
        int totalDestroyedTanks = 0;

        // Constructor
        public Tank(string t) : base()
        {
            tag = t;
        }

        // Initiate tank
        public void Init(float xPos = 0, float yPos = 0)
        {
            // Setup player tanks
            if(tag == "Player" || tag == "Menu"){
                tankSprite.Load("./PNG/Tanks/tankRed_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlack_outline.png");
                bulletTexture = LoadTextureFromImage(LoadImage("./PNG/Bullets/bulletRedSilver_outline.png"));

                // Setup tank light
                Light TankLight = new Light(400, 0.5f, 0f, new Color(255, 100, 0, 255), true);
                AddChild(TankLight);
                Game.lights.Add(TankLight);
            }
            else if(tag == "Enemy")
            {  
                // Initialise random enemy
                EnemyRandomiseInit();
            }
            // Default textures
            else
            {
                tankSprite.Load("./PNG/Tanks/tankBlue_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlue_outline.png");
            }


            // Setup the tank barrel starting rotation
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of turret to be centered in tank sprite
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            // Setup the tank base starting rotation
            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            // Set position of base to be centered in turret sprite
            turretSprite.SetPosition(0, (tankSprite.Width / 4f) - (turretSprite.Width/2));

            //firePoint.SetRotate(-90 * (float)(Math.PI));
            firePoint.SetPosition(turretSprite.Height + 15, 0);
            firePoint.HitWidth = 25;
            firePoint.HitHeight = 25;
            firePoint.hasCollision = true;

            // The point where tracks are spawned
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
            hasCollision = true;
            newTrackRadius = tankSprite.Width*1.25f;

            // Use circle collider for tanks
            SetCollisionType(new CircleCollider(new Vector3(0, 0, 0), HitWidth));

            // Add to game scene
            AddSelfToSceneObjects();
        }

        // Remove tank from scene (Overriden to remove enemies too)
        public override void RemoveSelfFromSceneObjects()
        {
            base.RemoveSelfFromSceneObjects();

            if(tag == "Enemy")
            {
                Game.enemies.Remove(this);
            }
        }

        // Call on update everytime tank scene object is updated
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);


            MoveTank(deltaTime);
            if (movingDirection == 0)
            {
                Decelerate(deltaTime);
            }


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
            
            // Execute enemy AI
            if(tag == "Enemy")
            {
                ExecuteEnemyAI(deltaTime);
            }
            // Update menu tank
            if(tag == "Menu")
            {
                UpdateMenuTank(deltaTime);
            }

            movingDirection = 0f;
        }

        // Add points
        public void AddDestroyedTankPoints()
        {
            points += (int)MathF.Pow(100, 1 + (totalDestroyedTanks / 100f));
            totalDestroyedTanks++;
        }

        // Randomise enemy tank initialisation
        public void EnemyRandomiseInit()
        {
            int chosen = Game.gameRandom.Next(0, 3);

            // Black tank, slow moving, slow shooter, long distance shooter, and fast bullets
            if(chosen == 0)
            {
                tankSprite.Load("./PNG/Tanks/tankBlack_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlack_outline.png");
                bulletTexture = LoadTextureFromImage(LoadImage("./PNG/Bullets/bulletSilverSilver_outline.png"));

                shootCooldown = 5f;
                shootSpeed = 2000f;

                maxVelocity = 100f;
                acceleration = 75f;
                deceleration = 50f;

                turnSpeed = 0.25f;
                shootingAccuracy = 0.4f;
            }
            // Blue tank, fast moving, fast shooter, short distance shooter, and slow bullets
            else if (chosen == 1)
            {
                tankSprite.Load("./PNG/Tanks/tankBlue_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlue_outline.png");
                bulletTexture = LoadTextureFromImage(LoadImage("./PNG/Bullets/bulletBlueSilver_outline.png"));

                shootCooldown = 1f;
                shootSpeed = 250f;

                maxVelocity = 250f;
                acceleration = 150f;
                deceleration = 100f;

                turnSpeed = 2f;
                shootingAccuracy = 0.9f;
            }
            // Green tank, average movement speed, average speed shooter, average distance shooter, average speed bullets
            else if (chosen == 2)
            {
                tankSprite.Load("./PNG/Tanks/tankGreen_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelGreen_outline.png");
                bulletTexture = LoadTextureFromImage(LoadImage("./PNG/Bullets/bulletGreenSilver_outline.png"));

                shootCooldown = 3f;
                shootSpeed = 600f;

                maxVelocity = 200f;
                acceleration = 200f;
                deceleration = 100f;

                turnSpeed = 0.9f;
                shootingAccuracy = 0.8f;
            }
            // Beige tank, spins really fast, shoots really fast, short distance shooter, no movement (Unused)
            else if (chosen == 3)
            {
                tankSprite.Load("./PNG/Tanks/tankBeige_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBeige_outline.png");
                bulletTexture = LoadTextureFromImage(LoadImage("./PNG/Bullets/bulletBeigeSilver_outline.png"));

                AICanMove = false;
                AITurretTracksPlayer = false;
                shootCooldown = .1f;
                shootSpeed = 50f;

                turnSpeed = 20f;
                shootingAccuracy = 0.8f;
            }

            // Add enemy to scene
            Game.enemies.Add(this);
        }


        float lastAngle = 0f;
        public void UpdateMenuTank(float deltaTime)
        {
            trackCooldownCount = 0;

            float difX = GetMouseX() - GlobalTransform.m20;
            float difY = GetMouseY() - GlobalTransform.m21;

            Vector3 diff = new Vector3(difX, difY, 0);

            // Move tank towards mouse if far enough away
            if (diff.MagnitudeSqr() >= 30*30) {
                velocity = 100f;
                MoveTank(deltaTime);
            }
            // If too close, move backwards
            else
            {
                velocity = -100f;
                MoveTank(deltaTime);
            }

            // Calculate the angle to face mouse
            float angle = MathF.Atan2(difY, difX);
            // Reset angle from current
            Rotate(-lastAngle);
            // Rotate to face mouse
            Rotate(angle);
            // Store this angle for later reset
            lastAngle = angle;


            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                ShootBullet();
            }

        }


        // The enemy AI
        public void ExecuteEnemyAI(float deltaTime)
        {
            Tank playerTank = Game.playerTank;

            if (AICanTurn)
            {
                // Should AI toggle turn
                if (Game.gameRandom.Next(0, 10000) == 1)
                {
                    AIturning = !AIturning;
                }

                // Should AI change turning direction
                if (Game.gameRandom.Next(0, 10000) == 1)
                {
                    AIturningDirection = -AIturningDirection;
                }
                if (AIturning)
                {
                    Rotate(AIturningDirection * deltaTime * turnSpeed);
                }
            }

            if (AICanMove)
            {
                // Should AI toggle movement
                if (Game.gameRandom.Next(0, 100000) == 1)
                {
                    AImoving = !AImoving;
                }

                if (Game.gameRandom.Next(0, 100000) == 1)
                {
                    AImovingDirection = -AImovingDirection;
                }
                if (AImoving)
                {
                    Accelerate(deltaTime, AImovingDirection);
                }
            }

            if (AITurretTracksPlayer)
            {
                // Calculate the angle to face player
                float angle = MathF.Atan2( (playerTank.globalTransform.m21-turretObject.GlobalTransform.m21), (playerTank.globalTransform.m20 - turretObject.GlobalTransform.m20));
                // Find the angle that the main body of the tank has rotated (Used to correct turret rotation)
                float angle2 = MathF.Atan2(GlobalTransform.m01, GlobalTransform.m00);

                turretObject.SetRotate(angle - angle2);
            }
            
         
            ShootBullet("Player");

        }


        // Draw function
        public override void OnDraw()
        {
            base.OnDraw();
            HitHeight = tankSprite.HitHeight;
            HitWidth = tankSprite.HitWidth;

        }

        // Add velocity
        public void Accelerate(float deltaTime, float direction = 1f)
        {
            movingDirection = direction;
            velocity = Math.Clamp(velocity + (movingDirection*(acceleration * deltaTime)), -maxVelocity, maxVelocity);
        }

        // Slow tank down
        public void Decelerate(float deltaTime)
        {
            if(velocity == 0f)
            {
                return;
            }
            float direction = velocity / Math.Abs(velocity);

            // If the tank is trying to move forward and velocity is negative, clamp between lowest possible velocity and 0
            velocity = Math.Clamp(velocity - (direction * (deceleration * deltaTime)), -maxVelocity, maxVelocity);
        }

        // Function used to move tank in direction adjusted by delta time
        public void MoveTank(float deltaTime)
        {
            // Find the facing direction of tank
            Vector3 facing = new Vector3(LocalTransform.m00, LocalTransform.m01, 1);
            
            // Calculate the direction and time adjusted movement vector
            Vector3 f = facing * (velocity) * deltaTime;

            // Translate position
            Translate(f.x, f.y);

            // Create a new track
            MakeTrack(facing);
        }

        // Rotate tank turret adjusted by delta time
        public void RotateTurret(float deltaTime, float direction = 1f)
        {
            turretObject.Rotate(deltaTime*direction*turretMoveSpeed);
        }

        // Shoot bullet
        public void ShootBullet(string target = "Enemy")
        {
            // If the shooting cooldown is not complete or firepoint is colliding, do not fire bullet
            if (!(shootCooldownCount >= shootCooldown) || firePoint.CheckCollision(0, 0))
            {
                return;
            }

            // Find the direction that the turret is facing
            Vector3 turretFacing = new Vector3(firePoint.GlobalTransform.m00, firePoint.GlobalTransform.m01, 1);
            // Alter shooting accuracy randomly on x or y axis
            switch (Game.gameRandom.Next(0, 1))
            {
                case 0:  
                    turretFacing.x *= shootingAccuracy;
                    break;
                case 1:
                    turretFacing.y *= shootingAccuracy;
                    break;
            }
                
            // Create new bullet
            BulletObject newbullet = new BulletObject(bulletTexture, turretFacing, shootSpeed, target);

            // Set position of new bullet to the tank fire point
            newbullet.SetPosition(firePoint.GlobalTransform.m20, firePoint.GlobalTransform.m21);

            // Add to bullet list to keep track of it
            newbullet.AddSelfToSceneObjects();

            // Reset cooldown
            shootCooldownCount = 0f;
            
        }


        // Make tank track
        public void MakeTrack(Vector3 facing)
        {
            
            // Check distance between current point and last track point
            Vector3 diff = lastTrackPoint - new Vector3(globalTransform.m20, globalTransform.m21, 0);
            // If not yet far enough, do not make new track
            if( !(diff.MagnitudeSqr() >= newTrackRadius * newTrackRadius))
            {
                return;
            }

            // Create new track sprite
            Track track = new Track();

            // Rotate sprite to face same direction as tank
            track.SetRotate(90 * (float)(Math.PI / 180.0f));
            track.Rotate(MathF.Atan2(facing.y, facing.x));

            // Set position of track to track spawn point
            track.SetPosition(trackPoint.GlobalTransform.m20, trackPoint.GlobalTransform.m21);

            lastTrackPoint = new Vector3(globalTransform.m20, globalTransform.m21, 0);
            // Reset cooldown
            trackCooldownCount = 0f;
        }
    }
}
