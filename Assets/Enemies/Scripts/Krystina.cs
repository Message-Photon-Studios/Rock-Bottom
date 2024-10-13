using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;

public class Krystina : Enemy
{
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollOffset;
    [SerializeField] float patrollIdleTime;

    protected override Node SetupTree()
    {
        
        Node root = new RandomPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .4f, "attack", "walk", patrollOffset);
        
        root.SetData("attack", false);
        return root;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition()+Vector2.right*patrollOffset + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance+ Vector2.right*patrollOffset);
    }
#endif
}
