using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RaylibStarterCS
{
    public class SceneObject
    {

        public string tag = "";

        protected SceneObject parent = null;
        protected List<SceneObject> children = new List<SceneObject>();

        protected Matrix3 localTransform = new Matrix3(1);
        protected Matrix3 globalTransform = new Matrix3(1);

        // Right top and left bottom
        protected Vector3[] WorldBoundries = new Vector3[2];
        //protected float HitRadius = 5f;

        public bool waitingDestroy = false;

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
        public void RemoveSelfFromSceneObjects()
        {
            Game.sceneObjects.Remove(this);
        }

        // Called on every update
        public virtual void OnUpdate(float deltaTime) 
        {
         
        } 
        
        // Called on every draw
        public virtual void OnDraw() 
        { 
 
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

        // Update the transform of this sceneObject. This is called everytime the sceneObjects transformation is changed
        public void UpdateTransform()
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

        // Set scale
        public void SetScale(float width, float height)
        {
            localTransform.SetScaled(width, height, 1);
            UpdateTransform();
        }

        // Translate scene object
        public void Translate(float x, float y)
        {
            if (!CheckCollision(x, y))
            {
                localTransform.Translate(x, y);
                UpdateTransform();
            }
        }

        public bool CheckCollision(float x, float y)
        {
            Vector3[] WorldBoundries = { new Vector3(GetScreenWidth(), GetScreenHeight(), 0), new Vector3(0, 0, 0) };
            // Check edge of window collisions
            if ((globalTransform.m20 + x >= WorldBoundries[0].x || globalTransform.m20 + x <= WorldBoundries[1].x || globalTransform.m21 + y >= WorldBoundries[0].y || globalTransform.m21 + y <= WorldBoundries[1].y))
            {
                // Right wall
                if (globalTransform.m20 + x >= WorldBoundries[0].x)
                {
                    CollideEvent(new Vector3(-1, 0, 0));
                }
                // Left wall
                else if (globalTransform.m20 + x <= WorldBoundries[1].x)
                {
                    CollideEvent(new Vector3(1, 0, 0));
                }
                // Bottom wall
                else if (globalTransform.m21 + y >= WorldBoundries[0].y)
                {
                    CollideEvent(new Vector3(0, 1, 0));
                }
                // Top wall
                else if (globalTransform.m21 + y <= WorldBoundries[1].y)
                {
                    CollideEvent(new Vector3(0, -1, 0));
                }
                return true;
            }

            List<SceneObject> removeObjs = new List<SceneObject>();
            foreach(SceneObject obj in Game.sceneObjects)
            {
                if(obj != this && !(tag == "Bullet" && obj.tag == "Bullet"))
                {
                    Vector3 sceneObjectPos = new Vector3(obj.GlobalTransform.m20, obj.GlobalTransform.m21, 0);
                    Vector3 thisObjectPos = new Vector3(GlobalTransform.m20, GlobalTransform.m21, 0);
                    float dist = MathF.Sqrt(Math.Abs(sceneObjectPos.x - thisObjectPos.x) + Math.Abs(sceneObjectPos.y - thisObjectPos.y));

                    
                    if (dist < 7.5f)
                    {
                        if (tag == "Bullet" && obj.tag == ((BulletObject)this).bulletTarget)
                        {
                            obj.waitingDestroy = true;
                            waitingDestroy = true;
                        }
                        Console.WriteLine(dist);
                    }
                }
            }
            // Remove wanted sceneobject
            foreach(SceneObject obj in removeObjs)
            {
                obj.RemoveSelfFromSceneObjects();
            }
            


            return false;


          
        }

        public virtual void CollideEvent(Vector3 Normal)
        {

        }

        // Rotate scene object
        public void Rotate(float radians)
        {
            localTransform.RotateZ(radians);
            UpdateTransform();
        }

        // Scale scene object
        public void Scale(float width, float height)
        {
            localTransform.Scale(width, height, 1);
            UpdateTransform();
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
            Debug.Assert(child.parent == null);
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
