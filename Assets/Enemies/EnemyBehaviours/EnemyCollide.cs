using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyCollide : Node
{
    ColliderCheck collider;
    string tag;
    public EnemyCollide (ColliderCheck collider, string tag)
    {
        this.collider = collider;
        this.tag = tag;
        
        state = NodeState.FAILURE;
        collider.onCollision += OnCollison;
    }

    private void OnCollison(Collision2D other)
    {
        if(tag == "") state = NodeState.SUCCESS;
        else if(other.collider.CompareTag(tag)) state = NodeState.SUCCESS;
    }

    public override NodeState Evaluate()
    {
        return state;
    }
}
