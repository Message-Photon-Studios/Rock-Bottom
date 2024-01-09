using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyEye : MonoBehaviour
{
    [SerializeField] float pivotDistance;
    [SerializeField] EnemyStats stats;

    SpriteRenderer spriteRenderer;

    Transform playerTrans; 
    Vector2 originPosition;
    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originPosition = transform.localPosition;
    }


    void Update()
    {
        Vector2 towardsPlayer = (Vector2)playerTrans.position - ((Vector2)transform.parent.position + originPosition);
        towardsPlayer.Normalize();

        transform.localPosition = originPosition+towardsPlayer*pivotDistance;

        if(stats.IsAsleep() || stats.IsDead())
        {
            spriteRenderer.enabled = false;
        } else
        {
            spriteRenderer.enabled = true;
        }
    }
}
