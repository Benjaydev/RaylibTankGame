using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public class BulletObject : SpriteObject
    {
        public float velocity = 1000f;
        public int bounces = 0;
        public int maxBounces = 4;
        public Vector3 ForwardVector;
        public bool hit = false;


        public BulletObject(Vector3 facing)
        {
            ForwardVector = facing * velocity;
            Load("./PNG/Bullets/bulletBlueSilver_outline.png");
            SetRotate(90 * (float)(Math.PI / 180.0f));
            Rotate(MathF.Atan2(ForwardVector.y, ForwardVector.x));
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
                bounces++;
                ForwardVector = ForwardVector * 0.75f;
            }
                
        }

        public bool UpdateBullet(Vector3 trans, float deltaTime)
        {
            if(bounces >= maxBounces)
            {
                return true;
            }
            Translate(trans.x, trans.y);

            return false;
        }



    }
}
