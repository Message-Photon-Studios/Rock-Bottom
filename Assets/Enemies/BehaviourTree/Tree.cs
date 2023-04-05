using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        protected Node root = null;
        protected void Start()
        {
            root = SetupTree();
        }

        protected virtual void Update()
        {
            if(root != null)
                root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
