using System.Collections;
using System.Collections.Generic;
namespace BehaviourTree
{
    /// <summary>
    /// The selector will retun success after it has found a successfull child. The selector will evaluate the success of the children in order.
    /// The selector will return failure if no successful childs are found. 
    /// </summary>
    public class Selector : Node
    {
        public Selector () : base() {}
        public Selector (List<Node> children) : base (children) {}
        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        continue;
                    case NodeState.SUCCESS:
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return  state;
        }
    }
}
