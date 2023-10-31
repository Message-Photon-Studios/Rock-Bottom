using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class ParticlesPlay: Node
{
    ParticleSystem particleSystem;
    bool play = false;

    /// <summary>
    /// Plays or stops a particle system
    /// </summary>
    public ParticlesPlay(ParticleSystem obj, bool active)
    {
        this.particleSystem = obj;
        this.play = active;
    }

    public override NodeState Evaluate()
    {
        if(play)
            particleSystem.Play();
        else
            particleSystem.Stop();
        return NodeState.SUCCESS;
    }
}
