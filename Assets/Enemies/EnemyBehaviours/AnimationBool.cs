using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AnimationBool : Node
{
    Animator animator;
    string triggerName;
    bool setTo;

    /// <summary>
    /// Activates the specified animation trigger on the animator and then returns success.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="triggerName"></param>
    /// <param name="setTo"></param>
    public AnimationBool(Animator animator, string triggerName, bool setTo)
    {
        this.animator = animator;
        this.triggerName = triggerName;
        this.setTo = setTo;
    }

    public override NodeState Evaluate()
    {
        animator.SetBool(triggerName, setTo);
        state = NodeState.SUCCESS;
        return state;
    }
}
