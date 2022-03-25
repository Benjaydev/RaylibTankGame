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

        public Button(int X, int Y, int length, int height, string text, int FontSize, Color FontColour, string action = "", bool SingleUse = false)
        {
            SetPosition(X, Y);
            fontSize = FontSize;
            fontColour = FontColour;

            centre = new Vector3(X + (length/2), Y + (height / 2), 0);
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
        }

        public override void RemoveSelfFromSceneObjects()
        {
            Game.buttons.Remove(this);
        }

        public void MeasureFontText()
        {
            textSize = MeasureText(buttonText, fontSize);
        }


        public bool IsPointWithinButton(float x, float y)
        {
            if( (x >= corners.x && x <= corners.z) && (y >= corners.y && y <= corners.w))
            {
                return true;
            }
            return false;
        }

        public virtual string AttemptButtonClick(float x, float y)
        {
            if(IsPointWithinButton(x, y))
            {
                isFocused = true;
                return ClickButton();
            }
            isFocused = false;
            return "";
        }


        public string ClickButton()
        {
            if (isSingleUse)
            {
                isWaitingDestroy = true;
            }
            return buttonAction;
        }

        public void OverlapButton(bool state)
        {
            if (isFocused)
            {
                texture = pressedTexture;
            }
            else if (state)
            {
                texture = hoverTexture;
            }
            else
            {
                texture = defaultTexture;
            }
            Width = buttonWidth;
            Height = buttonHeight;
        }


        public override void OnDraw()
        {
            base.OnDraw();
    

            DrawText($"{buttonText}", (int)centre.x - (textSize/2), Convert.ToInt32(centre.y - (fontSize/ 2.5f)), fontSize, fontColour);
        }

    }
}
