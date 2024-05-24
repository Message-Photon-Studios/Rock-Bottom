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
    public LayerMask layersToHit;
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
        float playerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * rotationSpeed);
        transform.rotation = newDir;

        if (particles.isEmitting){
            GetComponent<SpriteRenderer>().enabled = true;
        } else {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        ParticleSystem.ShapeModule hitterShape = particles.shape;

        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 50f, layersToHit);

        if (hit.collider == null){
            GetComponent<SpriteRenderer>().size = new Vector3(50f, (float)1.28);
        }
        else {
            GetComponent<SpriteRenderer>().size = new Vector3(hit.distance, (float)1.18);
        }

        hitterShape.position = new Vector2(hit.distance, 0);

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
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
            GetComponent<SpriteRenderer>().color = color.plainColor;
        }
        else
        {
            main.startColor = defaultColor;
            rootMain.startColor = defaultColor;
            GetComponent<SpriteRenderer>().color = defaultColor;
        }
    }
    public void TargetSet()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
