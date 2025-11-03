using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System;

public class EnemyObjectSpawner : Node
{
    EnemyStats stats;
    GameObject spawnTemp;
    Vector2 offset;
    Vector2 force;
    float forceRandomVariation;

    string returnObjName = "";
    bool setColor;
    /// <summary>
    /// Spawns an istance of the spawnTemp object at the enemys position + the offset.
    /// The offset flips with the enemy look direction. 
    /// The object will have a force applied to it equal to the force and also flipped with the enemy look dir.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="spawnTemp"></param>
    /// <param name="offset"></param>
    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset, Vector2 force, bool setColor, float forceRandomVariation)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.force = force;
        this.forceRandomVariation = forceRandomVariation;
        this.setColor = setColor;
    }

    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset, Vector2 force, bool setColor)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.force = force;
        this.forceRandomVariation = 0f;
        this.setColor = setColor;
    }

    public EnemyObjectSpawner(EnemyStats stats, GameObject spawnTemp, Vector2 offset, Vector2 force, bool setColor, string returnObjName)
    {
        this.spawnTemp = spawnTemp;
        this.stats = stats;
        this.offset = offset;
        this.force = force;
        this.forceRandomVariation = 0f;
        this.setColor = setColor;
        this.returnObjName = returnObjName;
    }



    public override NodeState Evaluate()
    {
        if(stats.IsAsleep() || stats.IsDead())
        {
            state = NodeState.FAILURE;
            return state;
        }

        try //Adding this here because i cant figure out what is null.
        {
            Vector2 useOffset = offset * (Vector2.left*stats.lookDir + Vector2.up);
            if(stats != null && spawnTemp != null && offset!=null && force!=null && this != null)
            {
                GameObject spwn = GameObject.Instantiate(spawnTemp, stats.GetPosition()+useOffset, stats.gameObject.transform.rotation) as GameObject;
                spwn.GetComponent<Rigidbody2D>()?.AddForce(force*(Vector2.right*stats.lookDir+Vector2.up)*(UnityEngine.Random.Range(0f,forceRandomVariation)+1f));

                EnemyStats spwnStats = spwn.GetComponent<EnemyStats>();
                if(spwnStats) spwnStats.spawnPower = stats.GetDamageFactor();
                if(setColor) spwn.GetComponent<EnemyStats>()?.SetColor(stats.GetColor());
                else if(!setColor) spwn.GetComponent<EnemyStats>()?.SetColor(null);
                spwn.GetComponent<EnemyStats>()?.SetParent(stats);

                if (returnObjName != "")
                {
                    Node n = this;
                    while(n.parent != null)
                    {
                        n = n.parent;
                    }
                    n.SetData(returnObjName, spwn);
                }

            }   
            state = NodeState.SUCCESS;
            return state;
        } catch (Exception e)
        {
            Debug.LogWarning(e);
            state = NodeState.FAILURE;
            return state;
            //At least you tried
        }
    }
}
