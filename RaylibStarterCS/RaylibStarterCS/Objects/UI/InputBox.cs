using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathsClasses;

namespace RaylibStarterCS
{
    public class InputBox : Button
    {
        public float keyPressCooldown = .2f;
        public float keyPressTimer = 0;
        public float maxCharacters = 10;
        public string defaultText = "Type Here";
        public string storedText = "";

        // Constructor (Uses base button constructor for most values)
        public InputBox(int X, int Y, int length, int height, string text, int FontSize, Color FontColour, string action = "") : base(X, Y, length, height, text, FontSize, FontColour, action = "")
        {
            defaultText = text;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            keyPressTimer += deltaTime;

            // If box is empty, use default text
            if (storedText.Length == 0)
            {
                buttonText = defaultText;
                MeasureFontText();
            }

            // Leave method if not text box is not focused or typing cooldown has not completed
            if (!isFocused || !(keyPressTimer >= keyPressCooldown))
            {
                return;
            }
             
            // Find the key being presseed
            string key = GetCurrentPressedKeys();    
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
                    // Add character
                    buttonText += key;
                    storedText = buttonText;
                }

                // Measure text to re-center
                MeasureFontText();
                // Reset typing cooldown
                keyPressTimer = 0;
            }
        }


        // Return pressed keys
        public string GetCurrentPressedKeys()
        {
            string pressedKeys = "";
            if (IsKeyDown(KeyboardKey.KEY_A)) pressedKeys += "a";
            if (IsKeyDown(KeyboardKey.KEY_B)) pressedKeys += "b";
            if (IsKeyDown(KeyboardKey.KEY_C)) pressedKeys += "c";
            if (IsKeyDown(KeyboardKey.KEY_D)) pressedKeys += "d";
            if (IsKeyDown(KeyboardKey.KEY_E)) pressedKeys += "e";
            if (IsKeyDown(KeyboardKey.KEY_F)) pressedKeys += "f";
            if (IsKeyDown(KeyboardKey.KEY_G)) pressedKeys += "g";
            if (IsKeyDown(KeyboardKey.KEY_H)) pressedKeys += "h";
            if (IsKeyDown(KeyboardKey.KEY_I)) pressedKeys += "i";
            if (IsKeyDown(KeyboardKey.KEY_J)) pressedKeys += "j";
            if (IsKeyDown(KeyboardKey.KEY_K)) pressedKeys += "k";
            if (IsKeyDown(KeyboardKey.KEY_L)) pressedKeys += "l";
            if (IsKeyDown(KeyboardKey.KEY_M)) pressedKeys += "m";
            if (IsKeyDown(KeyboardKey.KEY_N)) pressedKeys += "n";
            if (IsKeyDown(KeyboardKey.KEY_O)) pressedKeys += "o";
            if (IsKeyDown(KeyboardKey.KEY_P)) pressedKeys += "p";
            if (IsKeyDown(KeyboardKey.KEY_Q)) pressedKeys += "q";
            if (IsKeyDown(KeyboardKey.KEY_R)) pressedKeys += "r";
            if (IsKeyDown(KeyboardKey.KEY_S)) pressedKeys += "s";
            if (IsKeyDown(KeyboardKey.KEY_T)) pressedKeys += "t";
            if (IsKeyDown(KeyboardKey.KEY_U)) pressedKeys += "u";
            if (IsKeyDown(KeyboardKey.KEY_V)) pressedKeys += "v";
            if (IsKeyDown(KeyboardKey.KEY_W)) pressedKeys += "w";
            if (IsKeyDown(KeyboardKey.KEY_X)) pressedKeys += "x";
            if (IsKeyDown(KeyboardKey.KEY_Y)) pressedKeys += "y";
            if (IsKeyDown(KeyboardKey.KEY_Z)) pressedKeys += "z";
            if(IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) pressedKeys = pressedKeys.ToUpper();
            if (IsKeyDown(KeyboardKey.KEY_BACKSPACE)) return "backspace";

            return pressedKeys;
        }
    }
}
