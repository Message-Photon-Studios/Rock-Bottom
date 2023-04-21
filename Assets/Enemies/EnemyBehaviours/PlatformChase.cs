using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class PlatformChase : Node
{
    EnemyStats stats;
    Transform player;
    Rigidbody2D body;
    Animator animator;
    float chaseSpeedFactor;
    string stopBool;
    string walkAnimation;
    float startPoint;
    float chasePos;

    /// <summary>
    /// Chase the player if possible. If the enemy reaches the edge of a platform it will stop chasing
    /// </summary>
    /// <param name="stats">The enemy stats</param>
    /// <param name="player">The player transform</param>
    /// <param name="body"></param>
    /// <param name="animator"></param>
    /// <param name="chaseSpeedFactor">This float will be multiplied with the movement speed</param>
    /// <param name="legPos">The distance to the legs from the middle</param>
    /// <param name="stopBool">The player will stop when this bool is true</param>
    /// <param name="walkAnimationBool">The name of the walk animation bool</param>
    /// <returns></returns>
    public PlatformChase(EnemyStats stats, Transform player, Rigidbody2D body, Animator animator, float chaseSpeedFactor, float legPos, string stopBool, string walkAnimationBool) :
        base(new List<Node>{new CheckPlatformEdgePartly(stats, legPos), new CheckPlatformEdgePartly(stats, -legPos)})
    {
        this.stats = stats;
        this.player = player;
        this.body = body;
        this.chaseSpeedFactor = chaseSpeedFactor;
        this.animator = animator;
        this.stopBool = stopBool;
        this.walkAnimation = walkAnimationBool;
        startPoint = stats.GetPosition().x;
    }

    public override NodeState Evaluate()
    {
        var stopped = GetData(stopBool);
        if(stats.IsAsleep() || (stopped != null && (bool)stopped))
        {
            state = NodeState.FAILURE;
            return state;
        }

        bool atEdgeRight = (children[0].Evaluate() == NodeState.SUCCESS);
        bool atEdgeLeft = (children[1].Evaluate() == NodeState.SUCCESS);
        if((atEdgeRight && player.position.x > stats.GetPosition().x) || (atEdgeLeft && player.position.x < stats.GetPosition().x))
        {
            animator.SetBool(walkAnimation, false);
        } else
        {
            animator.SetBool(walkAnimation, true);
            chasePos = player.position.x;
            body.AddForce(new Vector2(((chasePos < stats.GetPosition().x)?-1:1)*stats.GetSpeed()*chaseSpeedFactor, 0)*Time.fixedDeltaTime);
        }
        
        state = NodeState.RUNNING;
        return state;
    }
}
