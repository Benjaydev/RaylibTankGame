using System;
using System.Collections.Generic;
using System.Text;
using static Raylib_cs.Raylib;
using Raylib_cs;
using MathClasses;

namespace RaylibStarterCS
{
    public class BulletObject : SceneObject
    {
        public float startVelocity = 1000f;
        public float velocityMultiple = 1f;
        public Vector3 ForwardVector;
        public bool hit = false;

        SpriteObject bulletSprite = new SpriteObject();
        public string bulletTarget = "Enemy";

        // Constructor
        public BulletObject(Texture2D bulletTexture, Vector3 facing, float velocity = 1000f, string bt = "Enemy") : base()
        {
            startVelocity = velocity;
            tag = "Bullet";
            bulletTarget = bt;
            ForwardVector = facing * startVelocity;
            hasCollision = true;
            bulletSprite.texture = bulletTexture;
            bulletSprite.SetPosition(-(bulletSprite.Width / 2), -(bulletSprite.Height / 2));
            HitWidth = bulletSprite.Width;

            // Setup default rotation
            SetRotate(90 * (float)(Math.PI / 180.0f));
            // Then rotate to the forward vector
            Rotate(MathF.Atan2(ForwardVector.y, ForwardVector.x));
            // Use circle collider
            SetCollisionType(new CircleCollider(new Vector3(0, 0, 0), HitWidth));
            AddChild(bulletSprite);

            // Setup light for bullet
            Light BulletLight = new Light(15, 1f, .15f, new Color(255, 100, 0, 255));
            BulletLight.SetPosition(0, bulletSprite.Height/2);
            AddChild(BulletLight);
            Game.lights.Add(BulletLight);
        }

        // Called when bullet collides
        public override void CollideEvent(Vector3 Normal)
        {
            base.CollideEvent(Normal);

            // Reset last collide
            lastCollide = -1;

            // Make sure normal is normalised
            Normal.Normalize();

            // Record the poisition of the bullet at hit
            Vector3 prevpos = new Vector3(globalTransform.m20, globalTransform.m21, 0);

            // Calculate reflected forward vector
            ForwardVector = ForwardVector - ((2 * (ForwardVector * Normal)) * Normal);

            // Calculate new rotation 
            SetRotate(90 * (float)(Math.PI / 180.0f));
            // Rotate to forward vector
            Rotate(MathF.Atan2(ForwardVector.y, ForwardVector.x));


            // Reset position to where it's meant to be
            SetPosition(prevpos.x, prevpos.y);
        }

        // 
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            // Get facing direction
            Vector3 f = ForwardVector * deltaTime;
            // Calculate new sloweed velocity
            velocityMultiple *= 1 - (0.5f * deltaTime);

            // Destroy bullet when it has slowed down enough
            if (velocityMultiple < (0.05f * (1 + deltaTime)))
            {
                
                isWaitingDestroy = true;
            }
            // Move bullet
            Translate(f.x * velocityMultiple, f.y * velocityMultiple);
        }

        public override void OnDraw()
        {
            base.OnDraw();
        }




    }
}
