using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class RandomPatroll : Node
{
    EnemyStats stats;
    float patrollDistance;
    float patrollSpeedFactor;
    float maxIdleTime;
    float idleTimer;
    float patrollStart;
    Rigidbody2D body;
    Animator animator;
    string walkAnimation;
    string attackBool;

    float patrollPoint;
    public RandomPatroll(EnemyStats stats, Rigidbody2D body, Animator animator, float patrollDistance, float patrollSpeedFactor, float maxIdleTime, float legPos, string attackBool, string walkAnimationBool) :
        base(new List<Node>{new CheckPlatformEdge(stats, legPos)})
    {
        this.stats = stats;
        this.patrollDistance = patrollDistance;
        this.body = body;
        this.patrollSpeedFactor = patrollSpeedFactor;
        this.animator = animator;
        this.maxIdleTime = maxIdleTime;
        this.attackBool = attackBool;
        this.walkAnimation = walkAnimationBool;
        patrollStart = stats.GetPosition().x;
        patrollPoint = stats.GetPosition().x;
        idleTimer = 0.5f;
    }

    public override NodeState Evaluate()
    {
        var attacking = GetData(attackBool);
        if(stats.IsAsleep() || (attacking != null && (bool)attacking)) return NodeState.FAILURE;

        if(idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            if(idleTimer < 0)
            {
                idleTimer = 0;
                animator.SetBool(walkAnimation, true);
            }
            return NodeState.RUNNING;
        }

        bool atEdge = (children[0].Evaluate() == NodeState.SUCCESS);
        if(Mathf.Pow(stats.GetPosition().x-patrollPoint, 2) < 1f && !atEdge)
        {
            patrollPoint = GetRandomPoint();
            idleTimer = UnityEngine.Random.Range(0, maxIdleTime);
            animator.SetBool(walkAnimation, false);
            return NodeState.RUNNING;
        }
        else if(atEdge)
            patrollPoint = patrollStart;

        body.AddForce(new Vector2(((patrollPoint < stats.GetPosition().x)?-1:1)*stats.GetSpeed()*patrollSpeedFactor, 0)*Time.deltaTime);
        return NodeState.RUNNING;
    }

    private float GetRandomPoint()
    {
        int randomDir = UnityEngine.Random.Range(-1, 2);
        float ret = patrollStart + randomDir * patrollDistance;
        return ret;
    }
}
