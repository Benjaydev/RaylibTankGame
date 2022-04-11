using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathClasses;

namespace RaylibStarterCS
{
    public class Button : SpriteObject
    {
        public bool isFocused = false;
        public bool isSingleUse = false;

        public string buttonText;
        public string buttonAction;

        public int buttonWidth;
        public int buttonHeight;

        public int fontSize;
        public int textSize;
        public Color fontColour;
        public Vector3 centre;
        public Vector4 corners;

        public static Texture2D hoverTexture = LoadTextureFromImage(LoadImage("./PNG/Buttons/ButtonHover.png"));
        public static Texture2D defaultTexture = LoadTextureFromImage(LoadImage("./PNG/Buttons/ButtonDefault.png"));
        public static Texture2D pressedTexture = LoadTextureFromImage(LoadImage("./PNG/Buttons/ButtonPressed.png"));

        // Constructor
        public Button(int X, int Y, int length, int height, string text, int FontSize, Color FontColour, string action = "", bool SingleUse = false)
        {
            SetPosition(X, Y);
            fontSize = FontSize;
            fontColour = FontColour;

            centre = new Vector3(X + (length/2), Y + (height / 2), 0);
            // Represent corners as two points (Stored in Vector4 - minX, minY, maxX, maxY)
            corners = new Vector4(X, Y, X + length, Y + height);

            buttonText = text;
            buttonAction = action;

            texture = defaultTexture;
            buttonWidth = length;
            buttonHeight = height;
            Width = length;
            Height = height;
            
            MeasureFontText();

            isSingleUse = SingleUse;
            hasCollision = false;
            Game.buttons.Add(this);
            Game.sceneObjects.Add(this);
        }

        // Remove from buttons
        public override void RemoveSelfFromSceneObjects()
        {
            base.RemoveSelfFromSceneObjects();

            Game.buttons.Remove(this);
        }

        // Measure the length of the text inside this button (Used to centre text when drawing)
        public void MeasureFontText()
        {
            textSize = MeasureText(buttonText, fontSize);
        }

        // Check if point is overlapping the button
        public bool IsPointWithinButton(float x, float y)
        {
            if( (x >= corners.x && x <= corners.z) && (y >= corners.y && y <= corners.w))
            {
                return true;
            }
            return false;
        }

        // Attempt a button click at point
        public virtual string AttemptButtonClick(float x, float y)
        {
            // If point is overlapping button
            if(IsPointWithinButton(x, y))
            {
                // Focus button
                isFocused = true;
                return ClickButton();
            }
            isFocused = false;
            return "";
        }

        // Click button
        public string ClickButton()
        {
            // If button is single use, destroy it after clicked
            if (isSingleUse)
            {
                isWaitingDestroy = true;
            }
            // Return back the necessary action
            return buttonAction;
        }

        // Trigger overlap (Change texture based on state)
        public void OverlapButton(bool state)
        {
            // If button has been pressed, show pressed texture
            if (isFocused)
            {
                texture = pressedTexture;
            }
            // Show hovered texture
            else if (state)
            {
                texture = hoverTexture;
            }
            // Reset to default texture
            else
            {
                texture = defaultTexture;
            }
            // Keep the width and height the same between each texture
            Width = buttonWidth;
            Height = buttonHeight;
        }


        public override void OnDraw()
        {
            base.OnDraw();
    
            // Draw text inside the button
            DrawText($"{buttonText}", (int)centre.x - (textSize/2), Convert.ToInt32(centre.y - (fontSize/ 2.5f)), fontSize, fontColour);
        }

    }
}
