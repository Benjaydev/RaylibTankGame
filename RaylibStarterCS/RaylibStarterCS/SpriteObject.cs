using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    public class SpriteObject : SceneObject
    {
        public Texture2D texture = new Texture2D();
        public Image image = new Image();
        public float scale = 1f;

        public float Width
        {
            get { return texture.width; }
        }

        public float Height
        {
            get { return texture.height; }
        }

        public SpriteObject()
        {
            
        }

        // Load image for this sprite texture
        public void Load(string filename)
        {
            Image img = LoadImage(filename);
            texture = LoadTextureFromImage(img);
        }

        public override void OnDraw()
        {
            // Using local x and y axis
            float rotation = (float)Math.Atan2(globalTransform.m01, globalTransform.m00);

            // Draw sprite to screen using raylib
            DrawTextureEx(texture, new Vector2(globalTransform.m20, globalTransform.m21), rotation * (float)(180.0f / Math.PI), scale, Color.WHITE);
        }
    }
}