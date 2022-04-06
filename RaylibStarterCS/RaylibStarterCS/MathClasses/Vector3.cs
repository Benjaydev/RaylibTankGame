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
        public float MagnitudeSqr()
        {
            return (float)this.Dot(this);
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

        // Minimum function to find the minimum value between each Vector3
        public static Vector3 Min(Vector3 p1, Vector3 p2)
        {
            float x = p1.x <= p2.x ? p1.x : p2.x;
            float y = p1.y <= p2.y ? p1.y : p2.y;
            float z = p1.z <= p2.z ? p1.z : p2.z;
            return new Vector3(x, y, z);
        }
        
        // Minimum function to find the maximum value between each Vector3
        public static Vector3 Max(Vector3 p1, Vector3 p2)
        {
            float x = p1.x >= p2.x ? p1.x : p2.x;
            float y = p1.y >= p2.y ? p1.y : p2.y;
            float z = p1.z >= p2.z ? p1.z : p2.z;
            return new Vector3(x, y, z);
        }

        public static Vector3 Clamp(Vector3 p, Vector3 min, Vector3 max)
        {
            float x = p.x <= min.x ? min.x : p.x;
            float y = p.y <= min.y ? min.y : p.y;
            float z = p.z <= min.z ? min.z : p.z;

            x = x >= max.x ? max.x : x;
            y = y >= max.y ? max.y : y;
            z = z >= max.z ? max.z : z;
            return new Vector3(x, y, z);
        }

        public float Distance(Vector3 v)
        {
            return (float)Math.Sqrt(Math.Pow(v.x - x, 2f) + Math.Pow(v.y - y, 2f) + Math.Pow(v.z - z, 2f));
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
        

    }
}
