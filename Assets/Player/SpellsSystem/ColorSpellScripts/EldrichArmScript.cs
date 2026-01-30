using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldrichArmScript : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Texture armTexture;
    [SerializeField] GameObject root;
    [SerializeField] GameObject enemyAnchor;
    [SerializeField] GameObject[] points;

    [SerializeField] float defaultSpringDistance, defaultSpringFrequency, defaultSpringDampingRatio;
    [SerializeField] float SpringDistance, SpringFrequency, SpringDampingRatio;
    [SerializeField] float disabledMultiplier = 4;

    private EldrichInkScript parent;
    private GameObject targetEnemy;
    private bool disabled = false;
    // Start is called before the first frame update
    void Start()
    {
        line.positionCount = points.Length+1;
        line.material.SetTexture("_MainTex", armTexture);
        RetractArm(defaultSpringDistance, defaultSpringFrequency, defaultSpringDampingRatio);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, root.transform.position);
        for(int i = 0; i < points.Length; i++)
        {
            line.SetPosition(i+1, points[i].transform.position);
        }
        if (targetEnemy) enemyAnchor.transform.position = targetEnemy.transform.position;
        if (!targetEnemy && Vector2.Distance(root.transform.position, enemyAnchor.transform.position) < 0.5) Destroy(gameObject);
        if (!parent && !disabled) DisableArm();
    }

    public void setEnemyTarget(GameObject enemy)
    {
        targetEnemy = enemy;
    }

    public void setEldrichParent(EldrichInkScript parent)
    {
        this.parent = parent;
    }

    public void DetachEnemy(Collider2D collision)
    {
        if (collision.gameObject != targetEnemy) return;
        RetractArm(SpringDistance, SpringFrequency, SpringDampingRatio);
        parent.DetachEnemy(collision);
        targetEnemy = null;
    }

    public void ReAttachEnemy(Collider2D collision)
    {
        if (targetEnemy || disabled || !parent) return;
        if (parent.ReAttachEnemy(collision.gameObject, gameObject)) RetractArm(defaultSpringDistance, defaultSpringFrequency, defaultSpringDampingRatio);
    }

    public void RetractArm(float distance, float frequency, float dapingRatio)
    {
        foreach (SpringJoint2D spring in GetComponentsInChildren<SpringJoint2D>())
        {
            spring.distance = distance;
            spring.frequency = frequency;
            spring.dampingRatio = dapingRatio;
        }
    }

    public void DisableArm()
    {
        disabled = true;
        targetEnemy = null;
        RetractArm(SpringDistance, SpringFrequency*disabledMultiplier, 1);
        foreach (Rigidbody2D body in GetComponentsInChildren<Rigidbody2D>())
        {
            body.drag = 10;
        }
    }
}
