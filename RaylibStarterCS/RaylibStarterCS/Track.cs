using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    public class Track : SpriteObject
    {
        float lifetime = 0f;
        float lifetimeMax = 2.5f;
        public static Texture2D trackTexture = LoadTextureFromImage(LoadImage("./PNG/Tanks/tracksSmall.png"));

        // Constructor
        public Track()
        {
            Game.sceneObjects.Insert(0, this);
            texture = trackTexture;
        }

        public override void OnUpdate(float deltaTime)
        {
            // Keep track of lifetime
            lifetime += deltaTime;
            // Destroy if lifetime is complete
            if(lifetime > lifetimeMax)
            {
                isWaitingDestroy = true;
            }
        }
    }
}
