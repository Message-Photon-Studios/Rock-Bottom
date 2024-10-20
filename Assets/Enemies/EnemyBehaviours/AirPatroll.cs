using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AirPatroll : Node
{
    EnemyStats stats;
    float patrollDistance;
    float patrollSpeedFactor;
    float maxIdleTime;
    float idleTimer;
    float patrollStart;
    Rigidbody2D body;
    Animator animator;
    string moveAnimation;
    string stopBool;

    float patrollPoint;

    /// <summary>
    /// Makes the enemy move to random positions within the patroll distance. When the enemy have moved to a position it will idle for a random time and then move to a new
    /// random position. If the enemy moves to an edge, into a wall or into another enemy the enemy will return to its starting position. 
    /// It will return running while the it patrolls, if the patroll is blocked for whatever reason then it will return faliure.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="body"></param>
    /// <param name="animator"></param>
    /// <param name="patrollDistance">The maximum distance from its starting position that the enemy can move to.</param>
    /// <param name="patrollSpeedFactor">This factor will change the speed of the patroll. It scales with the enemys movemet speed.</param>
    /// <param name="maxIdleTime">The maximum time that the enemy can idle</param>
    /// <param name="legPos">The distance from the middle to the enemys leg check</param>
    /// <param name="stopBool">If this bool is true the enemy will stop moving.</param>
    /// <param name="moveAnimationBool">This animation bool will be set to true when the enemy is moving</param>
    /// <returns></returns>
    public AirPatroll(EnemyStats stats, Rigidbody2D body, Animator animator, float patrollDistance, float patrollSpeedFactor, float maxIdleTime, float wallLookDist, string stopBool, string moveAnimationBool) : 
    base(new List<Node>{new CheckWall(stats, Vector2.right, wallLookDist,0f)})
    {
        this.stats = stats;
        this.patrollDistance = patrollDistance;
        this.body = body;
        this.patrollSpeedFactor = patrollSpeedFactor;
        this.animator = animator;
        this.maxIdleTime = maxIdleTime;
        this.stopBool = stopBool;
        this.moveAnimation = moveAnimationBool;
        patrollStart = stats.GetPosition().x;
        patrollPoint = stats.GetPosition().x;
        idleTimer = 0.5f;
    }

    public override NodeState Evaluate()
    {
        var stopped = GetData(stopBool);
        if(stats.IsAsleep() || (stopped != null && (bool)stopped)) 
        {
            state = NodeState.FAILURE;
            animator.SetBool(moveAnimation, false);
            return state;
        }

        if(idleTimer > 0)
        {
            animator.SetBool(moveAnimation, false);
            idleTimer -= Time.deltaTime;
            if(idleTimer < 0)
            {
                idleTimer = 0;
            }
            state = NodeState.RUNNING;
            return state;
        }

        bool atEdge = (children[0].Evaluate() == NodeState.SUCCESS);
        if(Mathf.Pow(stats.GetPosition().x-patrollPoint, 2) < 1f && !atEdge)
        {
            patrollPoint = GetRandomPoint();
            idleTimer = UnityEngine.Random.Range(0, maxIdleTime);
            animator.SetBool(moveAnimation, false);
            state = NodeState.RUNNING;
            return state;
        }
        else if(atEdge)
            patrollPoint = patrollStart;
        else if(Mathf.Abs(stats.GetPosition().x - patrollStart) > patrollDistance)
            patrollPoint = patrollStart;

        animator.SetBool(moveAnimation, true);
        if((patrollPoint - stats.GetPosition().x)*stats.lookDir<0) stats.ChangeDirection();
        body.AddForce(new Vector2(((patrollPoint < stats.GetPosition().x)?-1:1)*stats.GetSpeed()*patrollSpeedFactor, 0)*Time.deltaTime);
        state = NodeState.RUNNING;
        return state;
    }

    private float GetRandomPoint()
    {
        int randomDir = UnityEngine.Random.Range(-1, 2);
        float ret = patrollStart + randomDir * patrollDistance;
        return ret;
    }
}
