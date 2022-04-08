﻿using System;
using Raylib_cs;
using MathClasses;
using static Raylib_cs.Raylib;


namespace RaylibStarterCS
{
    public class SpriteObject : SceneObject
    {
        public Texture2D texture = new Texture2D();
        public Image image = new Image();
        public float textureScale = 1f;
        public bool hasScaled = false;
        public Color colour = Color.WHITE;
        public float Width
        {
            get { return texture.width; }
            set { texture.width = (int)value; }
        }

        public float Height
        {
            get { return texture.height; }
            set { texture.height = (int)value; }
        }

        public float defaultWidth;
        public float defaultHeight;

        public SpriteObject() : base()
        {
        }

        // Copy constructor
        public SpriteObject(SpriteObject copy, SceneObject passParent = null) : base(copy, passParent)
        {
            
            texture = copy.texture;
            textureScale = copy.textureScale;
            hasScaled = copy.hasScaled;
            defaultWidth = copy.defaultWidth;
            defaultHeight = copy.defaultHeight;
        }


        public override void UpdateTransform()
        {
            base.UpdateTransform();

            
            // Update sprite scale based on transform scale
            Vector3 scale = GetGlobalScale();

            if (!hasScaled)
            {
                defaultWidth = Width;
                defaultHeight = Height;
                hasScaled = true;
            }
            Width = defaultWidth * scale.x;
            Height = defaultHeight * scale.y;

        }


        // Load image for this sprite texture
        public void Load(string filename)
        {
            Image img = LoadImage(filename);
            texture = LoadTextureFromImage(img);
            defaultWidth = Width;
            defaultHeight = Height;
        }

        public override void OnDraw()
        {
            base.OnDraw();

            HitRadius = Width;
            HitWidth = Width;
            HitHeight = Height;
            float rotation = (float)Math.Atan2(globalTransform.m01, globalTransform.m00);

            if (!Raylib.WindowShouldClose() && !Game.IsDebugActive)
            {
                // Draw sprite to screen using raylib
                DrawTextureEx(texture, new System.Numerics.Vector2(globalTransform.m20, globalTransform.m21), rotation * (float)(180.0f / Math.PI), textureScale, colour);
            }
           


        }
    }
}