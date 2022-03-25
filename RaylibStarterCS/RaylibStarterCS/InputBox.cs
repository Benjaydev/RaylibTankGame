using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathClasses;

namespace RaylibStarterCS
{
    public class InputBox : Button
    {
        public float keyPressCooldown = .2f;
        public float keyPressTimer = 0;
        public float maxCharacters = 10;
        public string defaultText = "Type Here";
        public string storedText = "";

        public InputBox(int X, int Y, int length, int height, string text, int FontSize, Color FontColour, string action = "") : base(X, Y, length, height, text, FontSize, FontColour, action = "")
        {
            defaultText = text;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            keyPressTimer += deltaTime;

            if (storedText.Length == 0)
            {
                buttonText = defaultText;
                MeasureFontText();
            }

            if (isFocused)
            { 
                
                if (keyPressTimer >= keyPressCooldown)
                {
                    string key = GetCurrentPressedKey();
                    
                    if (key != "")
                    {
                        buttonText = storedText;
                        // Delete characters
                        if (key == "backspace")
                        {
                            // Check if there are any characters left
                            if (buttonText.Length > 0)
                            {
                                // Remove last character
                                buttonText = buttonText.Remove(buttonText.Length - 1);
                            }
                            key = "";
                        }

                        // Check if length is less than maximum
                        if (buttonText.Length < maxCharacters)
                        {
                            buttonText += key;
                            storedText = buttonText;
                        }


                        MeasureFontText();
                        keyPressTimer = 0;

                    }
                }
            }
        }



        public string GetCurrentPressedKey()
        {
            string pressedKey = "";
            if (IsKeyDown(KeyboardKey.KEY_A)) pressedKey += "a";
            if (IsKeyDown(KeyboardKey.KEY_B)) pressedKey += "b";
            if (IsKeyDown(KeyboardKey.KEY_C)) pressedKey += "c";
            if (IsKeyDown(KeyboardKey.KEY_D)) pressedKey += "d";
            if (IsKeyDown(KeyboardKey.KEY_E)) pressedKey += "e";
            if (IsKeyDown(KeyboardKey.KEY_F)) pressedKey += "f";
            if (IsKeyDown(KeyboardKey.KEY_G)) pressedKey += "g";
            if (IsKeyDown(KeyboardKey.KEY_H)) pressedKey += "h";
            if (IsKeyDown(KeyboardKey.KEY_I)) pressedKey += "i";
            if (IsKeyDown(KeyboardKey.KEY_J)) pressedKey += "j";
            if (IsKeyDown(KeyboardKey.KEY_K)) pressedKey += "k";
            if (IsKeyDown(KeyboardKey.KEY_L)) pressedKey += "l";
            if (IsKeyDown(KeyboardKey.KEY_M)) pressedKey += "m";
            if (IsKeyDown(KeyboardKey.KEY_N)) pressedKey += "n";
            if (IsKeyDown(KeyboardKey.KEY_O)) pressedKey += "o";
            if (IsKeyDown(KeyboardKey.KEY_P)) pressedKey += "p";
            if (IsKeyDown(KeyboardKey.KEY_Q)) pressedKey += "q";
            if (IsKeyDown(KeyboardKey.KEY_R)) pressedKey += "r";
            if (IsKeyDown(KeyboardKey.KEY_S)) pressedKey += "s";
            if (IsKeyDown(KeyboardKey.KEY_T)) pressedKey += "t";
            if (IsKeyDown(KeyboardKey.KEY_U)) pressedKey += "u";
            if (IsKeyDown(KeyboardKey.KEY_V)) pressedKey += "v";
            if (IsKeyDown(KeyboardKey.KEY_W)) pressedKey += "w";
            if (IsKeyDown(KeyboardKey.KEY_X)) pressedKey += "x";
            if (IsKeyDown(KeyboardKey.KEY_Y)) pressedKey += "y";
            if (IsKeyDown(KeyboardKey.KEY_Z)) pressedKey += "z";
            if (IsKeyDown(KeyboardKey.KEY_BACKSPACE)) return "backspace";
            if(IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) pressedKey = pressedKey.ToUpper();

            return pressedKey;
        }
    }
}
