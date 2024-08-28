using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingMarker : MonoBehaviour
{

    [SerializeField] Enemy parent;
    [SerializeField] ParticleSystem attackAim;
    [SerializeField] Color defaultColor;
    private PlayerStats player;
    private EnemyStats enemyStats;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        enemyStats = parent.GetComponent<EnemyStats>();
        enemyStats.onColorChanged += ChangeColor;
        ChangeColor(enemyStats.GetColor());
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null || parent == null) return;
        Vector3 direction = player.transform.position - parent.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.position = player.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        if (enemyStats.IsAsleep() || enemyStats.IsDead())
        {
            attackAim.Stop();
        } 
    }

    private void ChangeColor(GameColor color)
    {
        ParticleSystem.MainModule main = attackAim.main;
        if (color)
        {
            main.startColor = color.plainColor;
        }
        else
        {
            main.startColor = defaultColor;
        }
    }

}
