using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

/// <summary>
/// Enables or disables collision between two colliders
/// </summary>
public class ChangeCollisionDetection : Node
{
    Collider2D collider1;
    Collider2D collider2;
    bool setCollisionActive;
    public ChangeCollisionDetection(Collider2D collider1, Collider2D collider2, bool setCollisionActive)
    {
        this.collider1 = collider1;
        this.collider2 = collider2;
        this.setCollisionActive = setCollisionActive;
    }

    public override NodeState Evaluate()
    {
        Physics2D.IgnoreCollision(collider1, collider2, !setCollisionActive);
        state = NodeState.SUCCESS;
        return state;
    }
}
