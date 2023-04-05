using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    /// <summary>
    /// A behiour node in a behaviour tree
    /// </summary>
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        /// <summary>
        /// The current state of the node
        /// </summary>
        protected NodeState state;

        /// <summary>
        /// The parent of this node
        /// </summary>
        public Node parent;

        /// <summary>
        /// The children of this node
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        /// <returns></returns>
        protected List<Node> children {get; private set;} = new List<Node>();

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node (List<Node> children)
        {
            foreach(Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node) //Attaches the node correctly
        {
            node.parent = this;
            children.Add(node);
        }

        /// <summary>
        /// This method evaluates the node and returns the nodes state afterwards
        /// </summary>
        /// <returns></returns>
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        /// <summary>
        /// Sets the data for this node
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        /// <summary>
        /// Gets data from this or parent nodes if possibe
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            object value = null;
            if(dataContext.TryGetValue(key, out value))
                return value;
            
            Node node = parent;
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null)
                {
                    return value;
                }

                node = node.parent;
            }
            return null;
        }

        /// <summary>
        /// Cleares the data from this or parent nodes
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearData(string key)
        {
            if(dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if(cleared)
                {
                    return true;
                }
                node = node.parent;
            }
            return false;
        }
    }
}
