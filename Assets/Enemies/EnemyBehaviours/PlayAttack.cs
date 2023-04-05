using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class PlayAttack : Node
{
    Animator animator;
    string triggerName;

    public PlayAttack(Animator animator, string triggerName)
    {
        this.animator = animator;
        this.triggerName = triggerName;
    }

    public override NodeState Evaluate()
    {
        animator.SetTrigger(triggerName);
        return NodeState.SUCCESS;
    }
}
