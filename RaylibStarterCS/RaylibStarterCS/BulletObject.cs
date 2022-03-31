using System;
using System.Collections.Generic;
using System.Text;
using static Raylib_cs.Raylib;
using Raylib_cs;
using MathClasses;

namespace RaylibStarterCS
{
    public class BulletObject : SpriteObject
    {
        public float startVelocity = 1000f;
        public float velocityMultiple = 1f;
        public int bounces = 0;
        public int maxBounces = 4;
        public Vector3 ForwardVector;
        public bool hit = false;

        public string bulletTarget = "Enemy";


        public BulletObject(Texture2D bulletTexture, Vector3 facing, float velocity = 1000f, string bt = "Enemy")
        {
            startVelocity = velocity;
            tag = "Bullet";
            bulletTarget = bt;
            ForwardVector = facing * startVelocity;
            texture = bulletTexture;
            
            SetRotate(90 * (float)(Math.PI / 180.0f));
            Rotate(MathF.Atan2(ForwardVector.y, ForwardVector.x));
            hasCollision = true;
        }

        public override void CollideEvent(Vector3 Normal)
        {
            base.CollideEvent(Normal);

            if (!(bounces >= maxBounces))
            {
                Normal.Normalize();
                Vector3 prevpos = new Vector3(globalTransform.m20, globalTransform.m21, 0);
                ForwardVector = ForwardVector - ((2 * (ForwardVector * Normal)) * Normal);

                // Calculate new rotation 
                SetRotate(90 * (float)(Math.PI / 180.0f));
                Rotate(MathF.Atan2(ForwardVector.y, ForwardVector.x));

                // Reset position to where it's meant to be
                SetPosition(prevpos.x, prevpos.y);
                //bounces++;
                //ForwardVector = ForwardVector * velocityMultiple;
            }
                
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            UpdateBullet(deltaTime);
        }
        public override void OnDraw()
        {
            base.OnDraw();
            HitRadius = Width;
        }

        public void UpdateBullet(float deltaTime)
        {
            Vector3 f = ForwardVector * deltaTime;
            velocityMultiple *= 1 - (0.5f * deltaTime);
            if(bounces >= maxBounces || velocityMultiple < (0.05f * (1+deltaTime)))
            {
                isWaitingDestroy = true;
            }
            Translate(f.x* velocityMultiple, f.y* velocityMultiple);
        }



    }
}
