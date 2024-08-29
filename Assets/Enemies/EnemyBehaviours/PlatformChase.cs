using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class PlatformChase : Node
{
    EnemyStats stats;
    PlayerStats player;
    Rigidbody2D body;
    Animator animator;
    float chaseSpeedFactor;
    string stopBool;
    string walkAnimation;
    float startPoint;
    float viewRange;
    float eyePosY;
    float chasePos;

    /// <summary>
    /// Chase the player if possible. If the enemy reaches the edge of a platform it will stop chasing
    /// </summary>
    /// <param name="stats">The enemy stats</param>
    /// <param name="player">The player transform</param>
    /// <param name="body"></param>
    /// <param name="animator"></param>
    /// <param name="chaseSpeedFactor">This float will be multiplied with the movement speed</param>
    /// <param name="viewRange">This float determines how far the player can see</param>
    /// <param name="eyePosY">This float determines where the eye level is in relation to the pivot point</param>
    /// <param name="legPos">The distance to the legs from the middle</param>
    /// <param name="stopBool">The player will stop when this bool is true</param>
    /// <param name="walkAnimationBool">The name of the walk animation bool</param>
    /// <returns></returns>
    public PlatformChase(EnemyStats stats, PlayerStats player, Rigidbody2D body, Animator animator, float chaseSpeedFactor, float viewRange, float eyePosY,  float legPos, string stopBool, string walkAnimationBool) :
        base(new List<Node>{new CheckPlatformEdgePartly(stats, legPos), new CheckPlatformEdgePartly(stats, -legPos), new LookAtPlayer(stats, player)})
    {
        this.stats = stats;
        this.player = player;
        this.body = body;
        this.chaseSpeedFactor = chaseSpeedFactor;
        this.animator = animator;
        this.stopBool = stopBool;
        this.viewRange = viewRange;
        this.walkAnimation = walkAnimationBool;
        this.eyePosY = eyePosY;
        startPoint = stats.GetPosition().x;
    }
    
    private float playerDisapearCooldown = 0;
    public override NodeState Evaluate()
    {
        var stopped = GetData(stopBool);
        if(stats.IsAsleep() || (stopped != null && (bool)stopped))
        {
            state = NodeState.FAILURE;
            animator.SetBool(walkAnimation, false);
            return state;
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(stats.GetPosition()+Vector2.up*eyePosY, Vector2.left, viewRange, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlayer);
        RaycastHit2D hitRight = Physics2D.Raycast(stats.GetPosition()+Vector2.up*eyePosY, Vector2.right, viewRange, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlayer);

        if((hitLeft.collider == null || !hitLeft.collider.CompareTag("Player")) && (hitRight.collider == null || !hitRight.collider.CompareTag("Player")))
        {
            playerDisapearCooldown += Time.fixedDeltaTime;
            if(playerDisapearCooldown > 1f)
            {
                state = NodeState.FAILURE;
                animator.SetBool(walkAnimation, false);
                return state;
            }
        } else
            playerDisapearCooldown = 0;


        bool atEdgeRight = (children[0].Evaluate() == NodeState.SUCCESS);
        bool atEdgeLeft = (children[1].Evaluate() == NodeState.SUCCESS);
        if((atEdgeRight && player.transform.position.x > stats.GetPosition().x) || (atEdgeLeft && player.transform.position.x < stats.GetPosition().x))
        {
            animator.SetBool(walkAnimation, false);
        } else
        {
            animator.SetBool(walkAnimation, true);
            chasePos = player.transform.position.x;
            
            children[2].Evaluate();
            body.AddForce(new Vector2(((chasePos < stats.GetPosition().x)?-1:1)*stats.GetSpeed()*chaseSpeedFactor, 0)*Time.fixedDeltaTime);
        }
        
        state = NodeState.RUNNING;
        return state;
    }
}
