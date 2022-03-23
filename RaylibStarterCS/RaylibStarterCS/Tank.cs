﻿using System;
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

        public float moveSpeed = 100f;
        public float turretMoveSpeed = 1f;
        public float turnSpeed = 1f;
        public float shootSpeed = 1000f;
        public float shootingAccuracy = .8f;

        public float shootCooldown = 1f;
        public float shootCooldownCount = 1f;

        public float trackCooldown = 0.35f;
        public float trackCooldownCount = 0.35f;

        public int points = 0;

        public Tank(string t)
        {
            tag = t;
        }

        // Initiate tank
        public void Init(float xPos = 0, float yPos = 0)
        {
            HitRadius = 5f;
            if(tag == "Player"){
                tankSprite.Load("./PNG/Tanks/tankRed_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlack_outline.png");
            }
            else if(tag == "Enemy")
            {
                EnemyRandomiseInit();
            }
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
            
            if(tag == "Enemy")
            {
                ExecuteEnemyAI(deltaTime);
            }
        }

        int totalDestroyedTanks = 0;
        public void AddDestroyedTankPoints()
        {
            points += (int)MathF.Pow(100, 1 + (totalDestroyedTanks / 100f));
            totalDestroyedTanks++;
        }


        public void EnemyRandomiseInit()
        {
            int chosen = random.Next(0, 4);

            if(chosen == 0)
            {
                tankSprite.Load("./PNG/Tanks/tankBlack_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlack_outline.png");

                shootCooldown = 5f;
                shootSpeed = 2000f;

                moveSpeed = 25f;
                turnSpeed = 0.25f;
                shootingAccuracy = 0.4f;
            }
            else if (chosen == 1)
            {
                tankSprite.Load("./PNG/Tanks/tankBlue_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBlue_outline.png");

                shootCooldown = 1f;
                shootSpeed = 250f;

                moveSpeed = 250f;
                turnSpeed = 2f;
                shootingAccuracy = 0.9f;
            }
            else if (chosen == 2)
            {
                tankSprite.Load("./PNG/Tanks/tankGreen_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelGreen_outline.png");

                shootCooldown = 3f;
                shootSpeed = 600f;

                moveSpeed = 100f;
                turnSpeed = 0.9f;
                shootingAccuracy = 0.8f;
            }
            else if (chosen == 3)
            {
                tankSprite.Load("./PNG/Tanks/tankBeige_outline.png");
                turretSprite.Load("./PNG/Tanks/barrelBeige_outline.png");

                AICanMove = false;
                AITurretTracksPlayer = false;
                shootCooldown = .1f;
                shootSpeed = 50f;

                moveSpeed = 100f;
                turnSpeed = 20f;
                shootingAccuracy = 0.8f;
            }

        }


        bool AICanTurn = true;
        bool AICanMove = true;
        bool AITurretTracksPlayer = true;

        bool AIturning = true;
        bool AImoving = true;
        float AImovingDirection = 1f;
        float AIturningDirection = 1f;


        // The enemy AI
        public void ExecuteEnemyAI(float deltaTime)
        {
            Tank playerTank = Game.playerTank;

            if (AICanTurn)
            {
                // Should AI toggle turn
                if (random.Next(0, 10000) == 1)
                {
                    AIturning = !AIturning;
                }

                // Should AI change turning direction
                if (random.Next(0, 10000) == 1)
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
                if (random.Next(0, 100000) == 1)
                {
                    AImoving = !AImoving;
                }

                if (random.Next(0, 100000) == 1)
                {
                    AImovingDirection = -AImovingDirection;
                }
                if (AImoving)
                {
                    MoveTank(deltaTime, AImovingDirection);
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


            //MoveTank(deltaTime, 1);
        }


        // Draw function
        public override void OnDraw()
        {
            base.OnDraw();
            
        }

        // Function used to move tank in direction adjusted by delta time
        public void MoveTank(float deltaTime, float direction = 1f)
        {
            // Find the facing direction of tank
            Vector3 facing = new Vector3(LocalTransform.m00, LocalTransform.m01, 1);
            // Create a new track
            MakeTrack(facing);
            // Calculate the direction and time adjusted movement vector
            Vector3 f = facing * (direction*moveSpeed) * deltaTime;
            // Translate position
            Translate(f.x, f.y);
        }

        // Rotate tank turret adjusted by delta time
        public void RotateTurret(float deltaTime, float direction = 1f)
        {
            turretObject.Rotate(deltaTime*direction*turretMoveSpeed);
        }

        // Shoot bullet
        public void ShootBullet(string target = "Enemy")
        {
            // If the shooting cooldown is complete
            if(shootCooldownCount >= shootCooldown)
            {
                Vector3 turretFacing = new Vector3(firePoint.GlobalTransform.m00, firePoint.GlobalTransform.m01, 1); ;
                switch (random.Next(0, 1))
                {
                    case 0:
                        // Find the direction that the turret is facing
                        turretFacing.x *= shootingAccuracy;
                        break;
                    case 1:
                        // Find the direction that the turret is facing
                        turretFacing.y *= shootingAccuracy;
                        break;
                }
                
                // Create new bullet
                BulletObject newbullet = new BulletObject(turretFacing, shootSpeed, target);
                // Set position of new bullet to the tank fire point
                newbullet.SetPosition(firePoint.GlobalTransform.m20, firePoint.GlobalTransform.m21);
                // Add to bullet list to keep track of it
                Game.sceneObjects.Add(newbullet);

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
                Track track = new Track();

                // Rotate sprite to face same direction as tank
                track.SetRotate(90 * (float)(Math.PI / 180.0f));
                track.Rotate(MathF.Atan2(facing.y, facing.x));

                // Set position of track to track spawn point
                track.SetPosition(trackPoint.GlobalTransform.m20, trackPoint.GlobalTransform.m21);

                // Reset cooldown
                trackCooldownCount = 0f;
            }
        }
    }
}
