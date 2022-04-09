using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
    public class Colour
    {
        // Colour represents stores four bytes that represent RGBA
        public UInt32 colour;


        // Setup colour properties
        public byte red
        {
            get { return GetRed(); } 
            set { SetRed(value); }
        }
        public byte green
        {
            get { return GetGreen(); }
            set { SetGreen(value); }
        }
        public byte blue
        {
            get { return GetBlue(); }
            set { SetBlue(value); }
        }
        public byte alpha
        {
            get { return GetAlpha(); }
            set { SetAlpha(value); }
        }

        // Defualt constructor
        public Colour()
        {
            red = 0;
            green = 0;
            blue = 0;
            alpha = 0;
        }

        // Constructor to set each value
        public Colour(byte r, byte g, byte b, byte a = 255)
        {
            red = r;
            green = g;
            blue = b;
            alpha = a;
        }

        // Set all colours
        public void SetColour(byte r, byte g, byte b, byte a = 255)
        {
            red = r;
            green = g;
            blue = b;
            alpha = a;
        }

        // Set individual colours and place their bytes into the respective locations colour
        public void SetRed(byte r)
        {
            // Remove red from the colour varible
            colour &= 0x00ffffff;
            // Take the new red value and bitshift it then place it into the empty location
            colour |= (UInt32)r << 24;
        }
        public void SetGreen(byte g)
        {
            // Remove green from the colour varible
            colour &= 0xff00ffff;
            // Take the new green value and bitshift it then place it into the empty location
            colour |= (UInt32)g << 16;
        }
        public void SetBlue(byte b)
        {
            // Remove blue from the colour varible
            colour &= 0xffff00ff;
            // Take the new blue value and bitshift it then place it into the empty location
            colour |= (UInt32)b << 8;
        }
        public void SetAlpha(byte a)
        {
            // Remove alpha from the colour varible
            colour &= 0xffffff00;
            // Use an or operator to replace the location that was just removed
            colour |= (UInt32)a;
        }

        // Get individual colours
        public byte GetRed()
        {
            // Mask out all values that aren't in red position, then bitshift red back into a single byte
            return (byte) ((colour & 0xff000000) >> 24);
        }
        public byte GetGreen()
        {
            // Mask out all values that aren't in green position, then bitshift green back into a single byte
            return (byte)((colour & 0x00ff0000) >> 16);
        }
        public byte GetBlue()
        {
            // Mask out all values that aren't in blue position, then bitshift blue back into a single byte
            return (byte)((colour & 0x0000ff00) >> 8);
        }
        public byte GetAlpha()
        {
            // Mask out all values that aren't in alpha position and return the byte
            return (byte)(colour & 0x000000ff);
        }
    }
}
