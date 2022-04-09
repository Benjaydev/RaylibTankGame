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
        public int size = 20;
        public float brightness = 1f;
        public float sourceFadoff = 0.5f;

        public Light(int Size, float Brightness, float SourceFadeoff, Color Colour, bool HasNoColour = false)
        {
            colour = Colour;
            fadeColour = Colour;
            size = Size;
            brightness = Brightness;
            sourceFadoff = SourceFadeoff;
            hasNoColour = HasNoColour;
        }

        public void SetFadeColour(Color colour)
        {
            fadeColour = colour;
        }

        // Copy constructor
        public Light(Light copy) : base(copy)
        {
            position = copy.position;
            size = copy.size;
            colour = copy.colour;
            brightness = copy.brightness;
            fadeColour = copy.fadeColour;
            sourceFadoff = copy.sourceFadoff;
            hasNoColour = copy.hasNoColour;
        }

        public override void RemoveSelfFromSceneObjects()
        {
            base.RemoveSelfFromSceneObjects();

            Game.lights.Remove(this);
        }


        /// <summary>
        /// Draws the colour of the light (This function should be called after DrawLighting)
        /// </summary>
        public void ApplyColour()
        {
            if (!Raylib.WindowShouldClose() && !Game.IsDebugActive && !hasNoColour)
            {
                // Draw drop off
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, size, ColorAlpha(colour, brightness-0.1f), ColorAlpha(fadeColour, 0f));

                // Draw centre source
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, size*sourceFadoff, ColorAlpha(colour, brightness), ColorAlpha(fadeColour, 0f));
            }

        }
        /// <summary>
        /// Will remove darkess in the light radius (BlendMode.BLEND_MULTIPLIED must be active. Refer to BeginBlendMode)
        /// </summary>
        public void DrawLighting()
        {
            if (!Raylib.WindowShouldClose() && !Game.IsDebugActive)
            {
                DrawCircleGradient((int)globalTransform.m20, (int)globalTransform.m21, size, ColorAlpha(Color.WHITE, 0), ColorAlpha(Color.WHITE, 1f));
            }
        }


    }
}
