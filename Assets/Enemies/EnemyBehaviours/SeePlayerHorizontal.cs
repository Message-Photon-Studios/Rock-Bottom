using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class SeePlayerHorizontal : Node
{
    EnemyStats stats;
    PlayerStats player;
    float eyePosY;
    float viewRange;
    /// <summary>
    /// Checks wether the player can be seen in an horizontal line from the enemy
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="player"></param>
    /// <param name="eyePosY"></param>
    /// <param name="viewRange"></param>
    public SeePlayerHorizontal(EnemyStats stats, PlayerStats player, float eyePosY, float viewRange)
    {
        this.stats = stats;
        this.player = player;
        this.eyePosY = eyePosY;
        this.viewRange = viewRange;
    }
    
    private float playerDisapearCooldown = 0;
    public override NodeState Evaluate()
    {

        RaycastHit2D hitLeft = Physics2D.Raycast(stats.GetPosition()+Vector2.up*eyePosY, Vector2.left, viewRange, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlayer);
        RaycastHit2D hitRight = Physics2D.Raycast(stats.GetPosition()+Vector2.up*eyePosY, Vector2.right, viewRange, GameManager.instance.maskLibrary.onlyGround | GameManager.instance.maskLibrary.onlyPlayer);

        if((hitLeft.collider != null && hitLeft.collider.CompareTag("Player")) || (hitRight.collider != null && hitRight.collider.CompareTag("Player")))
        {
            state = NodeState.SUCCESS;
        } else
            state = NodeState.FAILURE;
        
        return state;
    }
}
