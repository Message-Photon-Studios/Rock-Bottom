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
    /// Sets a specific animation bool on the animator
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
