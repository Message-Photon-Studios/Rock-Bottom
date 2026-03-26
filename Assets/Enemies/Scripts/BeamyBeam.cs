using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BeamyBeam : MonoBehaviour
{
    [SerializeField] Enemy parent;
    [SerializeField] ParticleSystem particles;
    [SerializeField] ParticleSystem rootParticles;
    [SerializeField] Color defaultColor;
    [SerializeField] float aimRotationSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float laserSpeed;
    [SerializeField] float laserMaxLenght;
    [SerializeField] SpriteRenderer beamRenderer;
    [SerializeField] SpriteRenderer aimRenderer;
    public LayerMask layersToHit;
    private PlayerStats player;
    private EnemyStats enemyStats;
    private float currentDistance;
    private bool aim = false;
    

    
    private void Start()
    {
        player = Player.instance.playerStats;
        enemyStats = parent.GetComponent<EnemyStats>();
        enemyStats.onColorChanged += ChangeColor;
        ChangeColor(enemyStats.GetColor());
    }

    private void Update()
    {
        

        float rotSpeed = aimRotationSpeed;
        
        if (particles.isEmitting){
            beamRenderer.enabled = true;
            DisableAim();
            rotSpeed = rotationSpeed;
        } else {
            beamRenderer.enabled = false;
            currentDistance = 0;
        }
        aimRenderer.enabled = aim;

        Vector3 direction = player.transform.position - transform.position;
        float playerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * rotSpeed);
        transform.rotation = newDir;

        ParticleSystem.ShapeModule hitterShape = particles.shape;

        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, laserMaxLenght, layersToHit);
        float hitdistance = hit? hit.distance : laserMaxLenght;

        if (currentDistance < hitdistance)
        {
            currentDistance += laserSpeed * Time.deltaTime;
        } else
        {
            currentDistance = hitdistance;
        }

        beamRenderer.size = new Vector3(currentDistance, (float)1.18);
        aimRenderer.size = new Vector3(hitdistance, (float)1.18);

        hitterShape.position = new Vector2(currentDistance - 0.01f, 0);

        if (enemyStats.IsAsleep() || enemyStats.IsDead())
        {
            particles.Stop();
            rootParticles.Stop();
            DisableAim();
        }
    }

    private void OnParticleCollision(GameObject other) {
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

    public void EnableAim()
    {
        aim = true;
    }

    public void DisableAim()
    {
        aim = false;
    }
}
