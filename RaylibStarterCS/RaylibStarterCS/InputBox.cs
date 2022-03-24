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
        public bool isFocused = false;
        public float keyPressCooldown = 5;
        public float keyPressTimer = 0;

        public InputBox(int X, int Y, int length, int height, string text, int FontSize, Color FontColour, string action = "") : base(X, Y, length, height, text, FontSize, FontColour, action = "")
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            keyPressTimer+=deltaTime;

            if (isFocused)
            {
                if(keyPressTimer >= keyPressCooldown)
                {
                    string key = GetCurrentPressedKey();
                    if (key != "")
                    {
                        if(key == "backspace")
                        {
                            buttonText = buttonText.Remove(buttonText.Length-1);
                            key = "";
                        }
                        
                        buttonText += key;

                        MeasureFontText();
                        keyPressTimer = 0;


                    }

                    
                }
                
            }

        }



        public string GetCurrentPressedKey()
        {
            if (IsKeyDown(KeyboardKey.KEY_A)) return "a";
            if (IsKeyDown(KeyboardKey.KEY_B)) return "b";
            if (IsKeyDown(KeyboardKey.KEY_C)) return "c";
            if (IsKeyDown(KeyboardKey.KEY_D)) return "d";
            if (IsKeyDown(KeyboardKey.KEY_E)) return "e";
            if (IsKeyDown(KeyboardKey.KEY_F)) return "f";
            if (IsKeyDown(KeyboardKey.KEY_G)) return "g";
            if (IsKeyDown(KeyboardKey.KEY_H)) return "h";
            if (IsKeyDown(KeyboardKey.KEY_I)) return "i";
            if (IsKeyDown(KeyboardKey.KEY_J)) return "j";
            if (IsKeyDown(KeyboardKey.KEY_K)) return "k";
            if (IsKeyDown(KeyboardKey.KEY_L)) return "l";
            if (IsKeyDown(KeyboardKey.KEY_M)) return "m";
            if (IsKeyDown(KeyboardKey.KEY_N)) return "n";
            if (IsKeyDown(KeyboardKey.KEY_O)) return "o";
            if (IsKeyDown(KeyboardKey.KEY_P)) return "p";
            if (IsKeyDown(KeyboardKey.KEY_Q)) return "q";
            if (IsKeyDown(KeyboardKey.KEY_R)) return "r";
            if (IsKeyDown(KeyboardKey.KEY_S)) return "s";
            if (IsKeyDown(KeyboardKey.KEY_T)) return "t";
            if (IsKeyDown(KeyboardKey.KEY_U)) return "u";
            if (IsKeyDown(KeyboardKey.KEY_V)) return "v";
            if (IsKeyDown(KeyboardKey.KEY_W)) return "w";
            if (IsKeyDown(KeyboardKey.KEY_X)) return "x";
            if (IsKeyDown(KeyboardKey.KEY_Y)) return "y";
            if (IsKeyDown(KeyboardKey.KEY_Z)) return "z";
            if (IsKeyDown(KeyboardKey.KEY_BACKSPACE)) return "backspace";
            if (IsKeyDown(KeyboardKey.KEY_SPACE)) return " ";
            else return "";
        }









        public override string ClickButton()
        {
            isFocused = true;
            return base.ClickButton();
        }
    }
}
