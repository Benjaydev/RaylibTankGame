using System;
using System.Collections.Generic;
using MathsClasses;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    public class SceneObject
    {

        public string tag = "";
        public int id = 0;

        public SceneObject parent = null;
        public List<SceneObject> children = new List<SceneObject>();

        // Transforms of object
        protected Matrix3 localTransform = new Matrix3(1);
        protected Matrix3 globalTransform = new Matrix3(1);

        // Right top and left bottom
        // Collision
        public bool hasCollision = false;
        public bool movable = false;
        public float HitWidth = 5f;
        public float HitHeight = 5f;
        public Collider2D collisionBoundary = new AABB();

        // Movement
        public float maxSpeed = 200f;
        public float moveSpeed = 100f;
        public Vector3 velocity = new Vector3();
        public Vector3 acceleration = new Vector3();
        public float deceleration = 0.99f;

        public bool isWaitingDestroy = false;

        // Keep track of the id of the object that was last collided with. Default (Empty value) is -1
        public int lastCollide = -1;


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
        }

        // Copy Constructor (Pass parent is the parent to give this object)
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

            // Copy values 
            hasCollision = copy.hasCollision;
            movable = copy.movable;
            tag = copy.tag;
            HitWidth = copy.HitWidth;
            HitHeight = copy.HitHeight;

            // Copy transform
            localTransform = new Matrix3(copy.localTransform);
            globalTransform = new Matrix3(copy.globalTransform);

            // Create new id
            Game.gameLifetimeObjectCount++;
            id = Game.gameLifetimeObjectCount;     
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


        // Remove this scene object and all children from game scene
        public virtual void RemoveSelfFromSceneObjects()
        {
            // Remove self from parent
            if (parent != null)
            {
                parent.RemoveChild(this);
            }

            // Remove each child from this sceneObject
            foreach (SceneObject so in children)
            {
                so.isWaitingDestroy = true;
            }

            Game.sceneObjects.Remove(this);
        }

        // Add this scene object and all children to game scene
        public virtual void AddSelfToSceneObjects()
        {
            // Remove each child from this sceneObject
            foreach (SceneObject so in children)
            {
                so.AddSelfToSceneObjects();
            }
            if(this.GetType() != typeof(SpriteObject))
            {
                Game.sceneObjects.Add(this);
            }
            
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
            if (Game.IsDebugActive)
            {
                collisionBoundary.DrawDebug();

                if (hasCollision)
                {
                    // Draw the forward vector
                    Matrix3 transformed = new Matrix3(globalTransform);
                    transformed.RotateZ(MathF.Atan2(globalTransform.m00, globalTransform.m01));
                    transformed.Translate(globalTransform.m00 * 40, globalTransform.m01 * 40);
                    DrawLine((int)globalTransform.m20, (int)globalTransform.m21, (int)transformed.m20, (int)transformed.m21, Raylib_cs.Color.ORANGE);
                }
                
            }
        }

        // Update scene object
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



        // Set the collision type of the collision boundry
        public void SetCollisionType(Collider2D newCollider)
        {
            collisionBoundary = newCollider;
        }

        // Update the collision boundry
        public void UpdateBoundingBox()
        {
            // If scene object is using circle collider
            if (collisionBoundary.GetType() == typeof(CircleCollider))
            {
                float radius = (float) (HitWidth/Math.PI);
                ((CircleCollider)collisionBoundary).Fit(new Vector3[2] { new Vector3(globalTransform.m20 - radius, globalTransform.m21 - radius, 0), new Vector3(globalTransform.m20 + radius, globalTransform.m21 + radius, 0) });
                return;
            }

            // If using AABB
            // Get non-transformed  corners of object
            Vector3 corner = new Vector3((HitWidth / 2) + (HitWidth * 0.05f), (HitHeight / 2) + (HitHeight * 0.05f), 0);
            ((AABB)collisionBoundary).Fit(new Vector3[2] { new Vector3(globalTransform.m20 - corner.x, globalTransform.m21 - corner.y, 0), new Vector3(globalTransform.m20 + corner.x, globalTransform.m21 + corner.y, 0) });
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
        public bool Translate(float x, float y, bool overrideCollision = false)
        {
            bool result = false;
            if (overrideCollision)
            {
                localTransform.Translate(x, y);
                return true;
            }
            // Split the collision check between both the x and y axis 
            // This is done so that if one axes is colliding and the other is not, the object will still move along the axis that is not colliding
            // E.g. if the x-axis is currently colliding but the y-axis isn't, the tank should still be able to translate along that y-axis
            // This may impact performace in some scenarios, but it has been tested to make negligable difference with the simple collisions used

            // Check collision on x axis change
            if (!CheckCollision(x, 0))
            { 
                localTransform.Translate(x, 0);
                UpdateTransform();
                result = true;
            } 
            // Check collision on y axis change
            if (!CheckCollision(0, y))
            {
                localTransform.Translate(0, y);
                UpdateTransform();
                result = true;
            }

            return result;

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

        // Check if this object is currently colliding with another scene object
        public bool IsCollidingWithObject(SceneObject obj, float xChange = 0, float yChange = 0)
        {
            if (obj.collisionBoundary.GetType() == typeof(CircleCollider))
            {
                return collisionBoundary.Overlaps((CircleCollider)obj.collisionBoundary, xChange, yChange);
            }
            return collisionBoundary.Overlaps((AABB)obj.collisionBoundary, xChange, yChange);
        }
        
        // Check if this object is currently colliding with other scene objects with specified tag
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


        // General check collision for this object
        public bool CheckCollision(float x, float y)
        {
            // Return if there is no collision on this object
            if (!hasCollision)
            {
                return false;
            }
         
            // Check collision with every scene object
            foreach (SceneObject obj in Game.sceneObjects)
            {
                // Has collision, not itself, aren't both bullets, collision boundries aren't empty, and the last collision id's aren't the same
                if (obj.hasCollision && obj != this && (!obj.collisionBoundary.IsEmpty() && !collisionBoundary.IsEmpty()) && lastCollide != obj.id && obj.lastCollide != id)
                {
                    // Check if object is colliding with this object
                    if (IsCollidingWithObject(obj, x, y))
                    {

                        Vector3 normal = new Vector3(0, 0, 0);
                        // Calculate the normal against circle collider
                        if (obj.collisionBoundary.GetType() == typeof(CircleCollider))
                        {
                            normal = collisionBoundary.CalculateNormal((CircleCollider)obj.collisionBoundary, x, y);
                        }
                        // Calculate normal against AABB
                        else
                        {
                            normal = collisionBoundary.CalculateNormal((AABB)obj.collisionBoundary, x, y);
                        }

                        // Collide bullets
                        if ((tag == "Bullet" && obj.tag == "CollideAll"))
                        {
                            ((Bullet)this).CollideEvent(normal);

                            if (obj.movable)
                            {
                                obj.Translate(normal.x, normal.y);
                            }
                            // Save the bullets last collide in order to implement "invunerable" period for collisions
                            lastCollide = obj.id;
                            return true;
                        }

                        // Collide player with player collider
                        else if((tag == "Player" && obj.tag == "CollidePlayer"))
                        {
                            return true;
                        }

                        // Check for collison between movable objects
                        else if ( (obj.movable && (tag == "Player" || tag == "Enemy")) || (movable && obj.movable))
                        {
                            // Store this objects id inside the collided object to avoid this object being collided again while translating the collided object 
                            obj.lastCollide = id;

                            // Offset hit object by the amount being forced onto it
                            obj.Translate(normal.x, normal.y);
                                
                            // Reset the objects last collide
                            obj.lastCollide = -1;
                            return true;
                        }

                        // Basic collide all call
                        else if (obj.tag == "CollideAll")
                        {
                            return true;
                        }

                        // If bullet object has hit it's target
                        else if (tag == "Bullet" && (obj.tag == ((Bullet)this).bulletTarget))
                        {
                            // Destroy both objects
                            obj.isWaitingDestroy = true;
                            isWaitingDestroy = true;

                            // If bullet hits enemy, give points to player
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

        // Collide event used in some cases such as bullet
        public virtual void CollideEvent(Vector3 Normal)
        {

        }

        // Return the amount of children that scene object has
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
                Game.sceneObjects.Remove(child);
            }

        }
    }
}
