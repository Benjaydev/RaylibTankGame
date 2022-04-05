using System;
using System.Collections.Generic;
using System.Text;
using MathClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public class AABB
    {
        protected Vector3[] boundries = new Vector3[2] { new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity), new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity) };

        public Vector3 min
        {
            get { return boundries[0]; }
            set { boundries[0] = value; }
        }
        public Vector3 max
        {
            get { return boundries[1]; }
            set { boundries[1] = value; }
        }

        public List<Vector3> corners
        {
            get {
                List<Vector3> corners = new List<Vector3>(4);
                corners.Add(min);
                corners.Add(new Vector3(min.x, max.y, min.z));
                corners.Add(max);
                corners.Add(new Vector3(max.x, min.y, min.z));
                return corners;
            }
        }


        // Constructor
        public AABB()
        {
        }

        // Construct with max and min
        public AABB(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        // Copy constructor
        public AABB(AABB copy)
        {
            min = copy.min;
            max = copy.max;
        }

        public void DrawDebug()
        {
            DrawLine((int)corners[0].x, (int)corners[0].y, (int)corners[1].x, (int)corners[1].y, Color.DARKPURPLE);
            DrawLine((int)corners[1].x, (int)corners[1].y, (int)corners[2].x, (int)corners[2].y, Color.DARKPURPLE);
            DrawLine((int)corners[2].x, (int)corners[2].y, (int)corners[3].x, (int)corners[3].y, Color.DARKPURPLE);
            DrawLine((int)corners[3].x, (int)corners[3].y, (int)corners[0].x, (int)corners[0].y, Color.DARKPURPLE);


        }

        public void TranslateAABB(float x, float y)
        {
            min.x += x;
            max.x += x;
            min.y += y;
            max.y += y;
        }


        public Vector3 Center()
        {
            return (min + max) * 0.5f;
        }
        public Vector3 Extents()
        {
            return new Vector3(Math.Abs(max.x - min.x) * 0.5f, Math.Abs(max.y - min.y) * 0.5f, Math.Abs(max.z - min.z) * 0.5f);
        }

        public bool IsEmpty()
        {
            // Check if each value is at it's "Empty" state (Infinity)
            if (float.IsNegativeInfinity(min.x) && float.IsNegativeInfinity(min.y) && float.IsNegativeInfinity(min.z) && float.IsInfinity(max.x) && float.IsInfinity(max.y) && float.IsInfinity(max.z))
            {
                return true;
            }
            return false;
        }

        public void Empty()
        {
            min = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            max = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        // Fit the boundary box around a list of points
        public void Fit(List<Vector3> points)
        {
            // Reset the boundary box
            Empty();

            min = points[0];
            max = points[0];
            // find min and max of the points 
            foreach (Vector3 p in points)
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }

        }
        // Fit the boundary box around an array of points
        public void Fit(Vector3[] points)
        {
            // Reset the boundary box
            Empty();

            min = points[0];
            max = points[0];
            // Find min and max of the points 
            foreach (Vector3 p in points)
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        }

        // Check if a point overlaps this boundry box 
        // Optional: xChange and yChange represent whether this box is moving and an overlap needs to be checked before applying and translations
        public bool Overlaps(Vector3 p, float xChange = 0, float yChange = 0)
        {
            // Test for not overlapping
            return !(p.x + xChange < min.x || p.y + yChange < min.y || p.x + xChange > max.x || p.y + yChange > max.y) ;
        }

        // Check if another boundry box overlaps this boundry box 
        // Optional: xChange and yChange represent whether this box is moving and an overlap needs to be checked before applying and translations
        public bool Overlaps(AABB other, float xChange = 0, float yChange = 0)
        {
            // Test for not overlapping
            return !(max.x + xChange < other.min.x || max.y + yChange < other.min.y || min.x + xChange > other.max.x || min.y + yChange > other.max.y);
        }

        // Check the normals of point against to this box
        // Optional: xChange and yChange represent whether this box is moving and must correct it's max and min positions
        // in order to calculate normals (As box and point must not be overlapping to calculate)
        public Vector3 CalculateNorm(Vector3 p, float xChange = 0, float yChange = 0)
        {
            //      Top
            // Left [ ] Right
            //     Bottom
            Vector3 Norm = new Vector3();
            if (p.x - xChange > min.x)
            {
                // Right
                Norm.x = 1f;
            }
            if (p.y - yChange > min.y)
            {
                // Bottom
                Norm.y = -1f;
            }
            if (p.x - xChange < max.x)
            {
                // Left
                Norm.x = -1f;
            }
            if (p.y - yChange < max.y)
            {
                // Top
                Norm.y = 1f;
            }
            return Norm;

        }

        // Check the normals of another box against to this box
        // Optional: xChange and yChange represent whether this box is moving and must correct it's max and min positions
        // in order to calculate normals (As boxes must not be overlapping to calculate)
        public Vector3 CalculateNorm(AABB other, float xChange = 0, float yChange = 0)
        {
            //      Top
            // Left [ ] Right
            //     Bottom
            Vector3 Norm = new Vector3();
            if (max.x - xChange < other.min.x)
            {
                // Other object is on right of this
                Norm.x = -1f;
            }
            if (max.y - yChange < other.min.y)
            {
                // Other object is on bottom of this
                Norm.y = -1f;
            }
            if (min.x - xChange > other.max.x)
            {
                // Other object is on left of this
                Norm.x = 1f;
            }
            if (min.y - yChange > other.max.y)
            {
                // // Other object is on top of this
                Norm.y = 1f;
            }
            return Norm;
        }




        // Find the closest
        public Vector3 ClosestPoint(Vector3 p)
        {
            return Vector3.Clamp(p, min, max);
        }





        public void SetToTransformedBox(AABB box, Matrix3 m)
        {
            // If box is empty, then exit
            if (box.IsEmpty())
            {
                Empty();
                return;
            }

            // Row 1
            // Column 1
            if (m.m00 > 0.0f)
            {
                min.x += m.m00 * box.min.x;
                max.x += m.m00 * box.max.x;
            }
            else
            {
                min.x += m.m00 * box.max.x;
                max.x += m.m00 * box.min.x;
            }

            // Column 2
            if (m.m01 > 0.0f)
            {
                min.y += m.m01 * box.min.x;
                max.y += m.m01 * box.max.x;
            }
            else
            {
                min.y += m.m01 * box.max.x;
                max.y += m.m01 * box.min.x;
            }

            // Column 3
            if (m.m02 > 0.0f)
            {
                min.z += m.m02 * box.min.x;
                max.z += m.m02 * box.max.x;
            }
            else
            {
                min.z += m.m02 * box.max.x;
                max.z += m.m02 * box.min.x;
            }

            // Row 2
            // Column 1
            if (m.m10 > 0.0f)
            {
                min.x += m.m10 * box.min.y;
                max.x += m.m10 * box.max.y;
            }
            else
            {
                min.x += m.m10 * box.max.y;
                max.x += m.m10 * box.min.y;
            }

            // Column 2
            if (m.m11 > 0.0f)
            {
                min.y += m.m11 * box.min.y;
                max.y += m.m11 * box.max.y;
            }
            else
            {
                min.y += m.m11 * box.max.y;
                max.y += m.m11 * box.min.y;
            }

            // Column 3
            if (m.m12 > 0.0f)
            {
                min.z += m.m12 * box.min.y;
                max.z += m.m12 * box.max.y;
            }
            else
            {
                min.z += m.m12 * box.max.y;
                max.z += m.m12 * box.min.y;
            }

            // Row 3
            // Column 1
            if (m.m20 > 0.0f)
            {
                min.x += m.m20 * box.min.z;
                max.x += m.m20 * box.max.z;
            }
            else
            {
                min.x += m.m20 * box.max.z;
                max.x += m.m20 * box.min.z;
            }

            // Column 2
            if (m.m21 > 0.0f)
            {
                min.y += m.m21 * box.min.z;
                max.y += m.m21 * box.max.z;
            }
            else
            {
                min.y += m.m21 * box.max.z;
                max.y += m.m21 * box.min.z;
            }

            // Column 3
            if (m.m22 > 0.0f)
            {
                min.z += m.m22 * box.min.z;
                max.z += m.m22 * box.max.z;
            }
            else
            {
                min.z += m.m22 * box.max.z;
                max.z += m.m22 * box.min.z;
            }

        }

    }
}
