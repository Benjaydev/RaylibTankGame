using System;
using System.Collections.Generic;
using MathsClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public class AABB : Collider2D
    {
        protected Vector3[] boundries = new Vector3[2] { new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity), new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity) };

        // Property to get min values of bouundry
        public Vector3 min
        {
            get { return boundries[0]; }
            set { boundries[0] = value; }
        }
        // Property to get max values of bouundry
        public Vector3 max
        {
            get { return boundries[1]; }
            set { boundries[1] = value; }
        }

        // Get corners of AABB
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

        // Draw the debug wireframe of AABB
        public override void DrawDebug()
        {
            DrawLine((int)corners[0].x, (int)corners[0].y, (int)corners[1].x, (int)corners[1].y, Color.RED);
            DrawLine((int)corners[1].x, (int)corners[1].y, (int)corners[2].x, (int)corners[2].y, Color.RED);
            DrawLine((int)corners[2].x, (int)corners[2].y, (int)corners[3].x, (int)corners[3].y, Color.RED);
            DrawLine((int)corners[3].x, (int)corners[3].y, (int)corners[0].x, (int)corners[0].y, Color.RED);

            Vector3 center = Center();
            DrawCircle((int)min.x, (int)min.y, 3, Color.VIOLET);
            DrawCircle((int)max.x, (int)min.y, 3, Color.GREEN);
            DrawCircle((int)min.x, (int)max.y, 3, Color.BLUE);
            DrawCircle((int)max.x, (int)max.y, 3, Color.YELLOW);

        }

        // Translate AABB by x and y
        public void TranslateAABB(float x, float y)
        {
            min.x += x;
            max.x += x;
            min.y += y;
            max.y += y;
        }


        // Find the center of the AABB
        public Vector3 Center()
        {
            return (min + max) * 0.5f;
        }

        // Get the extents of the AABB
        public Vector3 Extents()
        {
            return new Vector3(Math.Abs(max.x - min.x) * 0.5f, Math.Abs(max.y - min.y) * 0.5f, Math.Abs(max.z - min.z) * 0.5f);
        }

        // Check if this AABB is invalid or unassigned
        public override bool IsEmpty()
        {
            // Check if each value is at it's "Empty" state (Infinity)
            if (float.IsNegativeInfinity(min.x) && float.IsNegativeInfinity(min.y) && float.IsNegativeInfinity(min.z) && float.IsInfinity(max.x) && float.IsInfinity(max.y) && float.IsInfinity(max.z))
            {
                return true;
            }
            return false;
        }

        // Set the AABB to empty
        public void Empty()
        {
            min = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            max = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        // Fit the boundary box around a list of points
        public override void Fit(List<Vector3> points)
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
        public override void Fit(Vector3[] points)
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
        public override bool Overlaps(Vector3 p, float xChange = 0, float yChange = 0)
        {
            // Test for not overlapping
            return !(p.x + xChange < min.x || p.y + yChange < min.y || p.x + xChange > max.x || p.y + yChange > max.y) ;
        }

        // Check if another boundry box overlaps this boundry box 
        // Optional: xChange and yChange represent whether this box is moving and an overlap needs to be checked before applying and translations
        public override bool Overlaps(AABB other, float xChange = 0, float yChange = 0)
        {
            // Test for not overlapping
            return !(max.x + xChange < other.min.x || max.y + yChange < other.min.y || min.x + xChange > other.max.x || min.y + yChange > other.max.y);
        }

        // Check if a circle collider overlaps this boundry box 
        // Optional: xChange and yChange represent whether this box is moving and an overlap needs to be checked before applying and translations
        public override bool Overlaps(CircleCollider cc, float xChange = 0, float yChange = 0)
        {
            // Call the overlap method inside the circle collider
            return cc.Overlaps(this, xChange, yChange);
        }

        // Check the normals of point against to this box
        // Optional: xChange and yChange represent whether this box is moving and must correct it's max and min positions
        public override Vector3 CalculateNormal(Vector3 p, float xChange = 0, float yChange = 0)
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
        public override Vector3 CalculateNormal(AABB other, float xChange = 0, float yChange = 0)
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

        // Check the normals of an AABB against this circle collider
        // Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions
        public override Vector3 CalculateNormal(CircleCollider cc, float xChange = 0, float yChange = 0)
        {
            // Call the normal calculation inside circle collider
            return cc.CalculateNormal(this, xChange, yChange);
        }



        // Find the closest point on AABB to another point 
        public Vector3 ClosestPoint(Vector3 p)
        {
            return Vector3.Clamp(p, min, max);
        }

    }
}
