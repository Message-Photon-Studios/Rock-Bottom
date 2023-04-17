using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    /// <summary>
    /// The sequence will evaluate each child in order. It will return succes if all children are successful. It will return faliure if it finds a failed child. 
    /// </summary>
    public class Sequence : Node
    {
        public Sequence () : base() {}
        public Sequence (List<Node> children) : base (children) {}
        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;
            foreach (Node node in children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return  state;
        }
    }
}
