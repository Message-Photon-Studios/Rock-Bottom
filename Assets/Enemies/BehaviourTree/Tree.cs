using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// The base class for a behaviour tree
    /// </summary>
    public abstract class Tree : MonoBehaviour
    {
        /// <summary>
        /// The root node of the behaviour tree
        /// </summary>
        protected Node root = null;
        protected void Start()
        {
            root = SetupTree();
        }

        protected virtual void FixedUpdate() //Evaluates the tree
        {
            if(root != null)
                root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
