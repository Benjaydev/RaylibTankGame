using System;
using System.Collections.Generic;
using System.Text;
using MathsClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    // Parent class for all 2D colliders
    public abstract class Collider2D
    {
        // Overlap methods
        public abstract bool Overlaps(AABB other, float xChange = 0, float yChange = 0);
        public abstract bool Overlaps(CircleCollider other, float xChange = 0, float yChange = 0);
        public abstract bool Overlaps(Vector3 p, float xChange = 0, float yChange = 0);

        // Normal calculations
        public abstract Vector3 CalculateNormal(Vector3 p, float xChange = 0, float yChange = 0);
        public abstract Vector3 CalculateNormal(AABB other, float xChange = 0, float yChange = 0);
        public abstract Vector3 CalculateNormal(CircleCollider other, float xChange = 0, float yChange = 0);

        // Empty check
        public abstract bool IsEmpty();

        // Debug method
        public abstract void DrawDebug();

        // Fit collider to points
        public abstract void Fit(Vector3[] points);
        public abstract void Fit(List<Vector3> points);


    }
}
