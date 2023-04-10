using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AnimationTrigger : Node
{
    Animator animator;
    string triggerName;

    /// <summary>
    /// Activates the specified animation trigger on the animator and then returns success.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="triggerName"></param>
    public AnimationTrigger(Animator animator, string triggerName)
    {
        this.animator = animator;
        this.triggerName = triggerName;
    }

    public override NodeState Evaluate()
    {
        animator.SetTrigger(triggerName);
        state = NodeState.SUCCESS;
        return state;
    }
}
