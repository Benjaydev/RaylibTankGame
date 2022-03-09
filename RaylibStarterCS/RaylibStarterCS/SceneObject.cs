using System;
using System.Collections.Generic;
using System.Text;

namespace RaylibStarterCS
{
    class SceneObject
    {
        protected SceneObject parent = null;
        protected List<SceneObject> children = new List<SceneObject>();

        public SceneObject Parent
        {
            get { return parent; }
        }

        public SceneObject()
        {
        }

        public int GetChildCount()
        {
            return children.Count;
        }

        public SceneObject GetChild(int index)
        {
            return children[index];
        }
    }
}
