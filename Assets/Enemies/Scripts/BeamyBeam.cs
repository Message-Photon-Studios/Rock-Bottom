using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BeamyBeam : MonoBehaviour
{
    [SerializeField] Beamy parent;
    [SerializeField] ParticleSystem particles;
    [SerializeField] ParticleSystem rootParticles;
    [SerializeField] Color defaultColor;
    [SerializeField] float rotationSpeed;
    private PlayerStats player;
    private EnemyStats enemyStats;

    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        enemyStats = parent.GetComponent<EnemyStats>();
        enemyStats.onColorChanged += ChangeColor;
        ChangeColor(enemyStats.GetColor());
    }

    private void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime*rotationSpeed);
        transform.rotation = newDir;

        if (enemyStats.IsAsleep())
        {
            particles.Stop();
            rootParticles.Stop();
        }
    }

    private void OnParticleCollision(GameObject other) { 
        if(other.CompareTag("Player"))
            parent.DamagePlayer();
    }

    private void ChangeColor(GameColor color)
    {
        ParticleSystem.MainModule main = particles.main;
        ParticleSystem.MainModule rootMain = rootParticles.main;
        if (color)
        {
            main.startColor = color.plainColor;
            rootMain.startColor = color.plainColor;
        }
        else
        {
            main.startColor = defaultColor;
            rootMain.startColor = defaultColor;
        }
    }
    public void TargetSet()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
}
