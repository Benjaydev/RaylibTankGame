using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
    public struct Vector4
    {
        // Initialise vector values
        public float x, y, z, w;

        // Contructor
        public Vector4(float X, float Y, float Z, float W)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        // Calculate and return the dot product of this vector and another vector
        public float Dot(Vector4 v)
        {
            return (x * v.x) + (y * v.y) + (z * v.z) + (w * v.w);
        }

        // Calculate the maginitude of vector (Length)
        public float Magnitude()
        {
            return (float)Math.Sqrt(this.Dot(this));
        }

        // Normalise this vector
        public void Normalize()
        {
            float magnitude = Magnitude();
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
            w /= magnitude;
        }

        // Calculate and return the cross product of this vector and another vector
        public Vector4 Cross(Vector4 v)
        {
            return new Vector4((y * v.z) - (z * v.y), (z * v.x) - (x * v.z), (x * v.y) - (y * v.x), 0);
        }

        // Overload addition operator for adding vector and vector (Translation)
        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }
        // Overload subtract operator for subtracting vector and vector (Translation)
        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }
        // Overload negative base operator to set vector values to negative
        public static Vector4 operator -(Vector4 v)
        {
            return new Vector4(-v.x, -v.y, -v.z, -v.w);
        }
        // Overload multiply operator for multiplying vector and vector
        public static Vector4 operator *(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }
        // Overload multiply operator for multiplying vector by float (Scale)
        public static Vector4 operator *(Vector4 v, float f)
        {
            return new Vector4(v.x * f, v.y * f, v.z * f, v.w * f);
        }
        // Overload multiply operator for multiplying float by vector (Scale)
        public static Vector4 operator *(float f, Vector4 v)
        {
            return new Vector4(v.x * f, v.y * f, v.z * f, v.w * f);
        }


    }
}
