using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathClasses;

namespace RaylibStarterCS
{
    public class Light : SceneObject
    {
        public Color colour = Color.WHITE;
        public Color fadeColour = Color.WHITE;
        public bool hasNoColour = false;
        public Vector3 position = new Vector3();
       
        // Current size, min size, and max size
        public Vector3 sizes = new Vector3(20f, 5f, 30f);

        public float size
        {
            get { return sizes.x; }
            set { sizes.x = value; }
        }

        public float changeDirection = 0;
        public float changeSpeed = 25f;


        public float brightness = 1f;
        public float sourceFadoff = 0.5f;


        // Constructor
        // Sizes represents (Current size, min size, and max size)
        public Light(Vector3 Sizes, float Brightness, float SourceFadeoff, Color Colour, bool HasNoColour = false)
        {
            colour = Colour;
            fadeColour = Colour;
            sizes = Sizes;
            brightness = Brightness;
            sourceFadoff = SourceFadeoff;
            hasNoColour = HasNoColour;
        }

        // Copy constructor
        public Light(Light copy) : base(copy)
        {
            position = copy.position;
            sizes = copy.sizes;
            colour = copy.colour;
            brightness = copy.brightness;
            fadeColour = copy.fadeColour;
            sourceFadoff = copy.sourceFadoff;
            hasNoColour = copy.hasNoColour;
        }

        // Change the colour that the light fades into
        public void SetFadeColour(Color colour)
        {
            fadeColour = colour;
        }

        // Remove self from game lighting
        public override void RemoveSelfFromSceneObjects()
        {
            base.RemoveSelfFromSceneObjects();

            Game.lights.Remove(this);
        }
        public override void AddSelfToSceneObjects()
        {
            base.AddSelfToSceneObjects();

            Game.lights.Add(this);
        }


        public void RandomLightSizeVariation(float deltaTime)
        {
            if (Game.gameRandom.Next(1, 100) == 1)
            {
                changeDirection = Game.gameRandom.Next(-1, 2);
            }
            size = Math.Clamp(size + (changeSpeed * changeDirection * deltaTime), sizes.y, sizes.z);
        }



        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            RandomLightSizeVariation(deltaTime);

        }



        /// <summary>
        /// Draws the colour of the light (This function should be called after DrawLighting)
        /// </summary>
        public void ApplyColour()
        {
            if (!Raylib.WindowShouldClose() && !Game.IsDebugActive && !hasNoColour)
            {
                // Draw drop off
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, (int)size, ColorAlpha(colour, brightness-0.1f), ColorAlpha(fadeColour, 0f));

                // Draw centre source
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, (int) (size *sourceFadoff), ColorAlpha(colour, brightness), ColorAlpha(fadeColour, 0f));
            }

        }
        /// <summary>
        /// Will remove darkess in the light radius (BlendMode.BLEND_MULTIPLIED must be active. Refer to BeginBlendMode)
        /// </summary>
        public void DrawLighting()
        {
            if (!Raylib.WindowShouldClose() && !Game.IsDebugActive)
            {
                // Multiplying a colour by the colour equivalent of 0 (Invisible colour) will result in that colour being removed (Removing dark spots in this case)
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, (int)size, ColorAlpha(Color.WHITE, 0), ColorAlpha(Color.WHITE, 1f));
            }
        }


    }
}
