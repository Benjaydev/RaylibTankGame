using System;
using System.Collections.Generic;
using System.Text;
using MathClasses;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace RaylibStarterCS
{
    public class CircleCollider : Collider2D
    {
        public Vector3 center = new Vector3(0,0,0);
        public float radius = 0;


        // Constructor
        public CircleCollider()
        {
        }

        // Construct using point and radius
        public CircleCollider(Vector3 p, float r)
        {
            this.center = p;
            this.radius = r;
        }

        /// <summary>
        /// Draw debug wireframe of circle
        /// </summary>
        public override void DrawDebug()
        {
            
            DrawCircleLines((int)center.x, (int)center.y, radius, Color.BLUE);
        }

        /// <summary>
        /// Fit the collider to an array of points
        /// </summary>
        public override void Fit(Vector3[] points)
        {
            // invalidate extents 
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // find min and max of the points 
            for (int i = 0; i < points.Length; ++i)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            // put a circle around the min/max box 
            center = (min + max) * 0.5f;
            radius = center.Distance(max);
        }
        
        /// <summary>
        /// Fit the collider to a list of points
        /// </summary>
        public override void Fit(List<Vector3> points)
        {
            // invalidate extents 
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // find min and max of the points 
            for (int i = 0; i < points.Count; ++i)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            // put a circle around the min/max box 
            center = (min + max) * 0.5f;
            radius = center.Distance(max);
        }

        public override bool IsEmpty()
        {
            return radius == 0;
        }

        /// <summary>
        /// Check if a point overlaps this circle collider
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions </para>
        /// </summary>
        public override bool Overlaps(Vector3 p, float xChange = 0, float yChange = 0)
        {
            Vector3 point = new Vector3(center.x + xChange, center.y + yChange, 0);
            Vector3 toPoint = p - point;
            return toPoint.MagnitudeSqr() <= (radius * radius);
        }

        /// <summary>
        /// Check if another circle collider overlaps this circle collider 
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions </para>
        /// </summary>
        public override bool Overlaps(CircleCollider other, float xChange = 0, float yChange = 0)
        {
            
            Vector3 point = new Vector3(center.x + xChange, center.y + yChange, 0);
            Vector3 diff = other.center - point;
            // compare distance between spheres to combined radii 
            float r = radius + other.radius;
            return diff.MagnitudeSqr() <= (r * r);
        }

        /// <summary>
        /// Check if an AABB overlaps this circle collider
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions </para>
        /// </summary>
        public override bool Overlaps(AABB aabb, float xChange = 0, float yChange = 0)
        {
            Vector3 point = new Vector3(center.x+xChange, center.y + yChange, 0);
            Vector3 closest = aabb.ClosestPoint(point);
            
            return Overlaps(closest, xChange, yChange);
        }


        /// <summary>
        /// Check the normals of an AABB against this circle collider
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions</para>
        /// </summary>
        public override Vector3 CalculateNormal(AABB aabb, float xChange = 0, float yChange = 0)
        {
            Vector3 direction = aabb.ClosestPoint(center) - center;
            direction.Normalize();
            return direction;
        }
        /// <summary>
        /// Check the normals of another circle collider against this circle collider
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions</para>
        /// </summary>
        public override Vector3 CalculateNormal(CircleCollider other, float xChange = 0, float yChange = 0)
        {
            Vector3 direction = other.center - center;
            direction.Normalize();
            return direction;
        }


        /// <summary>
        /// Check the normals of another point against this circle collider
        ///  <para>Optional: xChange and yChange represent whether this circle is moving and must correct it's max and min positions</para>
        /// </summary>
        public override Vector3 CalculateNormal(Vector3 p, float xChange = 0, float yChange = 0)
        {
            Vector3 direction = p - center;
            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// Find the closest point on circle to point
        /// </summary>
        public Vector3 ClosestPoint(Vector3 p)
        {
            // Distance from center 
            Vector3 toPoint = p - center;

            // If length is outside of radius bring it back to the radius 
            if (toPoint.MagnitudeSqr() > radius * radius)
            {
                toPoint.Normalize();
                toPoint *= radius;
            }
            return center + toPoint;
        }
    }
   
}
