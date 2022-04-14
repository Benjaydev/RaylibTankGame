using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathsClasses
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

        // Copy constructor
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

        // Calculate the squared maginitude of vector (Squared Length)
        public float MagnitudeSqr()
        {
            return this.Dot(this);
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

        // Return the normalised vector
        public Vector3 Normalized()
        {
            float magnitude = Magnitude();
            return new Vector3(x / magnitude, y / magnitude, z / magnitude);
        }

        // Static function to find the minimum value between two Vectors
        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            float x = v1.x <= v2.x ? v1.x : v2.x;
            float y = v1.y <= v2.y ? v1.y : v2.y;
            float z = v1.z <= v2.z ? v1.z : v2.z;
            return new Vector3(x, y, z);
        }
        
        // Static function to find the maximum value between each Vectors
        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            float x = v1.x >= v2.x ? v1.x : v2.x;
            float y = v1.y >= v2.y ? v1.y : v2.y;
            float z = v1.z >= v2.z ? v1.z : v2.z;
            return new Vector3(x, y, z);
        }

        // Static function to find the value of a vector locked within a minimum and maximum range
        public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
        {
            float x = v.x <= min.x ? min.x : v.x;
            float y = v.y <= min.y ? min.y : v.y;
            float z = v.z <= min.z ? min.z : v.z;

            x = x >= max.x ? max.x : x;
            y = y >= max.y ? max.y : y;
            z = z >= max.z ? max.z : z;
            return new Vector3(x, y, z);
        }

        // Find the distance between this vector and another vector
        public float Distance(Vector3 v)
        {
            return (float)Math.Sqrt(Math.Pow(v.x - x, 2) + Math.Pow(v.y - y, 2) + Math.Pow(v.z - z, 2));
        }


        // Return if vector has values all equal to 0
        public bool IsEmpty()
        {
            return (x == 0 && y == 0 && z == 0);
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
        // Overload multiply operator for dividing vector by float
        public static Vector3 operator /(Vector3 v, float f)
        {
            return new Vector3(v.x / f, v.y / f, v.z / f);
        }
        // Overload multiply operator for dividing float by vector
        public static Vector3 operator /(float f, Vector3 v)
        {
            return new Vector3(v.x / f, v.y / f, v.z / f);
        }

        // Overload multiply operator for dividing vector by vector
        public static Vector3 operator /(Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
        }


    }
}
