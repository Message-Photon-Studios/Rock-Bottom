using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// This node will invert the output of its child node. If the child is running then this is running
    /// </summary>
    public class Inverter : Node
    {
        public Inverter (Node node): base(new List<Node>{node}){}

        public override NodeState Evaluate()
        {
            switch (children[0].Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    break;
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    break;
                default:
                    state = NodeState.RUNNING;
                    break;
            }

            return state;
        }
    }
}
