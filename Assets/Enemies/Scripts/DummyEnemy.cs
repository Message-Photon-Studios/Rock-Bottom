using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class DummyEnemy : Enemy
{
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>{});
        return root;
    }
}
