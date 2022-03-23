using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
    public class Vector3
    {
        // Initialise vector values
        public float x, y, z;

        // Default Constructor
        public Vector3()
        {
            x = y = z = 0;
        }

        // Constructor
        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        public Vector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        // Calculate and return the dot product of this vector and another vector
        public float Dot(Vector3 v)
        {
            return (x * v.x) + (y * v.y) + (z * v.z);
        }

        // Calculate the maginitude of vector (Length)
        public float Magnitude()
        {
            return (float) Math.Sqrt(this.Dot(this));
        }

        // Normalise this vector
        public void Normalize()
        {
            float magnitude = Magnitude();
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
        }

        // Calculate and return the cross product of this vector and another vector
        public Vector3 Cross(Vector3 v)
        {
            return new Vector3((y * v.z) - (z * v.y), (z * v.x) - (x * v.z), (x * v.y) - (y * v.x));
        }

        // Overload addition operator for adding vector and vector (Translation)
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        // Overload subtract operator for subtracting vector and vector (Translation)
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        // Overload negative base operator to set vector values to negative
        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }
        // Overload multiply operator for multiplying vector and vector 
        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
        // Overload multiply operator for multiplying vector by float (Scale)
        public static Vector3 operator *(Vector3 v, float f) 
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }
        // Overload multiply operator for multiplying float by vector (Scale)
        public static Vector3 operator *(float f, Vector3 v)
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }
        

    }
}
