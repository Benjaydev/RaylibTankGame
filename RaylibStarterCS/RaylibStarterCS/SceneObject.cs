using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using MathClasses;

namespace RaylibStarterCS
{
    public class SceneObject
    {

        public string tag = "";
        public int id = 0;

        protected SceneObject parent = null;
        protected List<SceneObject> children = new List<SceneObject>();

        protected Matrix3 localTransform = new Matrix3(1);
        protected Matrix3 globalTransform = new Matrix3(1);

        // Right top and left bottom
        public bool hasCollision = false;
        public bool movable = false;
        public float HitRadius = 5f;
        public float HitWidth = 5f;
        public float HitHeight = 5f;
        public AABB collisionBoundary = new AABB();
        public int lastCollide = -1;


        public bool isWaitingDestroy = false;

        public Random random = new Random();

        public Matrix3 LocalTransform
        {
            get { return localTransform; }
        }

        public Matrix3 GlobalTransform
        {
            get { return globalTransform; }
        }

        public SceneObject Parent
        {
            get { return parent; }
        }


        // Constructor
        public SceneObject()
        {
            Game.gameLifetimeObjectCount++;
            id = Game.gameLifetimeObjectCount;
            Console.WriteLine(Game.gameLifetimeObjectCount);
        }

        // Copy Constructor
        public SceneObject(SceneObject copy, SceneObject passParent = null)
        {
            if(copy.parent != null && passParent == null)
            {
                parent = new SceneObject(copy.parent);
            }
            else
            {
                parent = passParent;
            }


            // Get each child
            foreach (var child in copy.children)
            {
                // If child is a sprite object, cast to that
                if (child.GetType() == typeof(SpriteObject))
                {
                    // Pass this as parent for the child class to avoid infinite recursion (Child -> Creates new parent in copy constructor -> That parent recreates the same child -> Then the loop continues)
                    AddChild(new SpriteObject((SpriteObject)child, this));
                    continue;
                }
                // Create a sceneobject
                AddChild(new SceneObject(child, this));
            }
            hasCollision = copy.hasCollision;
            movable = copy.movable;
            tag = copy.tag;
            HitRadius = copy.HitRadius;
           

            localTransform = new Matrix3(copy.localTransform);
            globalTransform = new Matrix3(copy.globalTransform);
        }


        // Deconstruct the sceneObject
        ~SceneObject()
        {
            // Remove self from parent
            if (parent != null)
            {
                parent.RemoveChild(this);
            }

            // Remove each child from this sceneObject
            foreach (SceneObject so in children)
            {
                so.parent = null;
            }
        }
        public virtual void RemoveSelfFromSceneObjects()
        {
            Game.sceneObjects.Remove(this);
        }

        // Called on every update
        public virtual void OnUpdate(float deltaTime) 
        {
            if (hasCollision)
            {
                UpdateBoundingBox();
            }
        }
            
        
        // Called on every draw
        public virtual void OnDraw() 
        {
            collisionBoundary.DrawDebug();
        }

        public void Update(float deltaTime)
        { 
            // Call OnUpdate 
            OnUpdate(deltaTime);

            // Update all children of this sceneObject
            foreach (SceneObject child in children)
            {
                child.Update(deltaTime);
            }

        }

        // Draw SceneObject
        public void Draw()
        {
            // Call OnDraw 
            OnDraw();

            // Draw all children of this sceneObject
            foreach (SceneObject child in children)
            {
                child.Draw();
            }
        }


        public void UpdateBoundingBox()
        {
            // Get non-transformed  corners of object
            Vector3 corner = new Vector3((HitWidth / 2) + (HitWidth * 0.05f), (HitHeight / 2) + (HitHeight * 0.05f), 0);
            collisionBoundary.Fit(new Vector3[2] { new Vector3(globalTransform.m20 - corner.x, globalTransform.m21 - corner.y, 0), new Vector3(globalTransform.m20 + corner.x, globalTransform.m21 + corner.y, 0) });
        }
        // Update the transform of this sceneObject. This is called everytime the sceneObjects transformation is changed
        public virtual void UpdateTransform()
        {

            // If this sceneObject has a parent, calculate the globalTransform
            if (parent != null)
            {
                globalTransform = parent.globalTransform * localTransform;
            }  
            // Default to localTransform
            else
            {
                globalTransform = localTransform;
            }
            
            // Update transform for each child in this sceneObject
            foreach (SceneObject child in children)
            {
                child.UpdateTransform();
            }
           

        }

        // Set position
        public void SetPosition(float x, float y)
        {
            localTransform.SetTranslation(x, y);
            UpdateTransform();
        }

        // Set rotation
        public void SetRotate(float radians)
        {
            localTransform.SetRotateZ(radians);
            UpdateTransform();
        }

        // Rotate scene object
        public void Rotate(float radians)
        {
            localTransform.RotateZ(radians);
            UpdateTransform();
        }

        // Set scale
        public void SetScale(float width, float height)
        {
            localTransform.SetScaled(width, height, 1);
            UpdateTransform();
        }
        

        // Scale scene object
        public virtual void Scale(float width, float height)
        {
            localTransform.Scale(width, height, 1);
            UpdateTransform();
        }

        public Vector3 GetLocalScale()
        {
            return localTransform.GetScale();
        }
        public Vector3 GetGlobalScale()
        {
            return globalTransform.GetScale();
        }

        // Translate scene object
        public void Translate(float x, float y, bool overrideCollision = false)
        {

            if (overrideCollision)
            {
                localTransform.Translate(x, y);
                return;
            }
            // Split the collision check between both the x and y axis 
            // This is done so that if one axes is colliding and the other is not, the object will still move along the axis that is not colliding
            // E.g. if the x-axis is currently colliding but the y-axis isn't, the tank should still be able to translate along that y-axis
            // This may impact performace, however it is untested.

            // Check collision on x axis change
            if (!CheckCollision(x, 0))
            {
                localTransform.Translate(x, 0);
            
            } 
            
            // Check collision on y axis change
            if (!CheckCollision(0, y))
            {
                localTransform.Translate(0, y);
                
            }
            UpdateTransform();
        }

        // Check if this object has hit the world boundry after moving by x and y
        public string HasHitWorldBoundry(float x = 0, float y = 0)
        {
            
            // Right wall
            if (globalTransform.m20 + x >= Game.WorldBoundries[0].x)
            {
                return "Right";
            }
            // Left wall
            else if (globalTransform.m20 + x <= Game.WorldBoundries[1].x)
            {
                return "Left";
            }
            // Bottom wall
            else if (globalTransform.m21 + y >= Game.WorldBoundries[0].y)
            {
                return "Bottom";
            }
            // Top wall
            else if (globalTransform.m21 + y <= Game.WorldBoundries[1].y)
            {
                return "Top";
            }

            return "";
        }

        // Check if this object is currently colliding with object
        public bool IsCollidingWithObject(SceneObject obj)
        {
            // Check if collision distance is met
            return collisionBoundary.Overlaps(obj.collisionBoundary);
        }
        
        // Check if this object is currently colliding with tag
        public bool IsCollidingWithTag(string checkTag)
        {
            // Check collision with every scene object
            foreach (SceneObject obj in Game.sceneObjects)
            {
                // Skip iteration if any of these are met
                if (!obj.hasCollision || obj == this || obj.tag != checkTag)
                {
                    continue;
                }
                return IsCollidingWithObject(obj);
            }
            return false;
        }

        public void SeperateIntersectingObject(List<string> checkTag, float normx = 1, float normy = 1)
        {
            foreach (SceneObject obj in Game.sceneObjects)
            {
                // Skip iteration if any of these are met
                if (!obj.hasCollision || obj == this || !checkTag.Contains(obj.tag))
                {
                    continue;
                }
                while (true)
                {
                    if (IsCollidingWithObject(obj))
                    {
                        obj.Translate(normx, normy);
                        break;
                    }
                }
            }
        }
        public void SeperateIntersectingObject(SceneObject obj, float normx = 1, float normy = 1)
        {
                // Skip iteration if any of these are met
                if (!obj.hasCollision || obj == this)
                {
                    return;
                }
                while (true)
                {
                    if (IsCollidingWithObject(obj))
                    {
                    obj.Translate(normx, normy);
                    break;
                    }
                }
            }


            // General check collision for this object
            public bool CheckCollision(float x, float y)
        {
            // Return if there is no collision on this object
            if (!hasCollision)
            {
                return false;
            }
         
            List<SceneObject> objects = new List<SceneObject>(Game.sceneObjects);
            // Check collision with every scene object
            foreach (SceneObject obj in objects)
            {
                // Has collision, not itself, and aren't both bullets
                if (obj.hasCollision && obj != this)
                {
                    // Get the side that will be colliding
                    Vector3 norm = collisionBoundary.CalculateNorm(obj.collisionBoundary, x, y);
                    if (norm.IsEmpty())
                    {
                        norm = new Vector3(x, y, 0);
                        norm.Normalize();
                    }

                    // Check if that location is colliding
                    if (collisionBoundary.Overlaps(obj.collisionBoundary, x, y))
                    {
                        if((tag == "Bullet" && obj.tag == "CollideAll"))
                        {
                            // Save the bullets last collide in order to implement "invunerable" period for collisions
                            lastCollide = obj.id;

                            ((BulletObject)this).CollideEvent(norm);
                                
                            return true;
                        }

                        else if((tag == "Bullet" && obj.tag == "Bullet")){
                            // If bullets don't have the same target
                            if(((BulletObject)this).bulletTarget != ((BulletObject)obj).bulletTarget)
                            {
                                obj.isWaitingDestroy = true;
                                isWaitingDestroy = true;
                                return true;
                            }
                        }

                        // Collide player and enemy tanks with eachother
                        else if((tag == "Player" && obj.tag == "Enemy") || (obj.tag == "Player" && tag == "Enemy"))
                        {
                            return true;
                        }

                        // Check for collison between movable objects
                        else if ( (obj.movable && (tag == "Player" || tag == "Enemy")) || (movable && obj.movable))
                        {
                            // Find the angle between current object and other object in order to 
                            Vector3 objvec = new Vector3(obj.globalTransform.m20 - globalTransform.m20, obj.globalTransform.m21 - globalTransform.m21, 0);
                            Vector3 facing = new Vector3(globalTransform.m00, globalTransform.m01, 1);
                            facing.Normalize();
                            objvec.Normalize();
                            double angle = Math.Acos(objvec.Dot(facing)) * RAD2DEG;

                            if(angle-45 < 25 || (movable && obj.movable) && lastCollide != obj.id)
                            {

                                // Store this object inside the collided object to avoid this object being collided again while translating the collided object 
                                obj.lastCollide = id;

                                // Offset hit object by the amount being forced onto it
                                obj.Translate(-norm.x, -norm.y);
                                
                                // Reset the objects last collide
                                obj.lastCollide = -1;
                                //lastCollide = null;
                            }
                            return true;
                        }

                        else if (obj.tag == "CollideAll")
                        {
                            return true;
                        }

                        // If bullet object has hit it's target
                        else if (tag == "Bullet" && (obj.tag == ((BulletObject)this).bulletTarget))
                        {
                            obj.isWaitingDestroy = true;
                            isWaitingDestroy = true;

                            // Bullet hits enemy give points to player
                            if (obj.tag == "Enemy")
                            {
                                Game.playerTank.AddDestroyedTankPoints();
                            }

                            return true;
                        }
                    }
                }
            }

            // Check edge of window collisions
            string boundryHit = HasHitWorldBoundry(x, y);

            if (boundryHit != "")
            {
                // Right wall
                if (boundryHit == "Right")
                {
                    CollideEvent(new Vector3(-1, 0, 0));
                }
                // Left wall
                else if (boundryHit == "Left")
                {
                    CollideEvent(new Vector3(1, 0, 0));
                }
                // Bottom wall
                else if (boundryHit == "Bottom")
                {
                    CollideEvent(new Vector3(0, 1, 0));
                }
                // Top wall
                else if (boundryHit == "Top")
                {
                    CollideEvent(new Vector3(0, -1, 0));
                }
                return true;
            }
            return false;
        }

        public virtual void CollideEvent(Vector3 Normal)
        {

        }



        // Return the amount of children
        public int GetChildCount()
        {
            return children.Count;
        }

        // Get a child of a certain index
        public SceneObject GetChild(int index)
        {
            return children[index];
        }

        // Add child to this sceneObject
        public void AddChild(SceneObject child)
        {
            // Check and make sure the object that's being added doesn't already have a parent
            //Debug.Assert(child.parent == null);
            // Make this sceneObject the parent of the child 
            child.parent = this;
            // Add the child to children list
            children.Add(child);
        }

        // Remove child from this sceneObject
        public void RemoveChild(SceneObject child)
        {
            // If removal is succesful, remove the childs parent
            if (children.Remove(child) == true)
            {
                child.parent = null;
            }
        }
    }
}
