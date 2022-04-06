using System;
using System.Collections.Generic;
using System.Text;
using MathClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public abstract class Collider2D
    {
        
        public Type ColliderType;

        /*
        public bool Overlaps(Collider2D other, float xChange = 0, float yChange = 0)
        {
            if (other.ColliderType.Equals(typeof(CircleCollider)))
            { 
                return Overlaps((CircleCollider)other, xChange, yChange);
            }

            return Overlaps((AABB)other, xChange, yChange);
        }*/


        public abstract bool Overlaps(AABB other, float xChange = 0, float yChange = 0);
        public abstract bool Overlaps(CircleCollider other, float xChange = 0, float yChange = 0);
        public abstract bool Overlaps(Vector3 p, float xChange = 0, float yChange = 0);

        //public abstract void Fit(Vector3[] points);
        //public abstract void Fit(List<Vector3> points);

        /*
        public Vector3 CalculateNorm(Collider2D other, float xChange = 0, float yChange = 0)
        {
            if (other.ColliderType.Equals(typeof(AABB)))
            {
                return CalculateNorm((AABB)other, xChange, yChange);
            }

            return new Vector3(0, 0, 0);

        }*/

        public abstract Vector3 CalculateNorm(Vector3 p, float xChange = 0, float yChange = 0);
        public abstract Vector3 CalculateNorm(AABB other, float xChange = 0, float yChange = 0);

        public abstract Vector3 CalculateNorm(CircleCollider other, float xChange = 0, float yChange = 0);

        public abstract bool IsEmpty();

        public abstract void DrawDebug();
        
    }
}
