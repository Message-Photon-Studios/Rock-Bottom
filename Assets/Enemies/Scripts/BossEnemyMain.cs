using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;

public class BossEnemyMain : Enemy
{
    [Header("Minion spawning")]
    [SerializeField] int minionMax;
    [SerializeField] float wispTimer;
    [SerializeField] float minionWaveTimer;
    [SerializeField] GameObject wispTmp;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] Vector2 spawnForce;

    [Header("Beam attack")]
    [SerializeField] int beamDamage = 0;
    [SerializeField] float laserDecaySpeed = .5f;
    [SerializeField] Trigger beamTrigger;
    [SerializeField] float beamTimer;
    [SerializeField] ParticleSystem attackOrb;
    [SerializeField] ParticleSystem attackBeam;
    [SerializeField] ParticleSystem attackAim;

    [Header("Bomb attack")]
    [SerializeField] float bombTimer;
    [SerializeField] Animator handAnimator;

    [Header("Patrolling")]
    [SerializeField] float patrollDistance;
    [SerializeField] float patrollIdleTime;

    UnityEvent<GameObject> onWispSpawned = new UnityEvent<GameObject>();
    UnityEvent onNewMinionWave = new UnityEvent();

    [SerializeField] List<GameObject> spawned = new List<GameObject>();

    protected override Node SetupTree()
    {
        
        Node root = new Sequence(new List<Node>{

            new KeepHeight(stats, transform.position.y, 1f),
            new Selector(new List<Node>{

                new Sequence(new List<Node>{
                    new Wait(minionWaveTimer),
                    new ActivateAction(onNewMinionWave)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("sleeping", false),
                    new Wait(bombTimer),
                    new AnimationTrigger(handAnimator, "bombSnap")
                }),
                
                new Sequence(new List<Node>{
                    new CheckBool("sleeping", false),
                    new CheckBool("attack", false),
                    new CheckPlayerArea(stats, player, beamTrigger),
                    new Wait(beamTimer, .5f),
                    new LookAtPlayer(stats, player),
                    new SetParentVariable("attack", true, 3),
                    new ParticlesPlay(attackAim, true),
                    new ParticlesPlay(attackOrb, true),
                    new ParticlesPlay(attackBeam, true),
                    }),

                new Sequence(new List<Node> {
                    new CheckBool("attack", true),
                    new Inverter(new CheckPlayerArea(stats, player, beamTrigger)),
                    new Wait(laserDecaySpeed, 0.1f),
                    new ParticlesPlay(attackOrb, false),
                    new ParticlesPlay(attackBeam, false),
                    new ParticlesPlay(attackAim, false),
                    new SetParentVariable("attack", false, 3)
                }),

                new Sequence(new List<Node>{
                    new CheckBool("canSpawn", true),
                    new CheckBool("sleeping", false),
                    new Wait(wispTimer),
                    new EnemyObjectSpawner(stats, wispTmp, spawnOffset, spawnForce, true, "spawnedWisp"),
                    new ActivateAction<GameObject>(onWispSpawned, "spawnedWisp")
                }),

                new AirPatroll(stats, body, animator, patrollDistance, 1, patrollIdleTime, .7f, "attack", "move")
            })
        });
        
        onWispSpawned.AddListener(WispSpawned);
        onNewMinionWave.AddListener(NewMinionWave);
        triggersToFlip.Add(beamTrigger);
        root.SetData("activateBeam", false);
        root.SetData("attack", false);
        root.SetData("sleeping", false);
        root.SetData("canSpawn", true);
        return root;
    }

    public void IncreaseMinionAmount(int addMax)
    {
        minionMax+=addMax;
    }

    void NewMinionWave()
    {
        KillAllMinions();
        root.SetData("canSpawn", true);
    }

    void WispSpawned(GameObject spawnedWisp)
    {
        spawned.Add(spawnedWisp);
        if(spawned.Count >= minionMax) root.SetData("canSpawn", false);

        spawnedWisp.GetComponent<EnemyStats>().onEnemyDeath += SpawnDied;
        spawnedWisp.GetComponent<Wisp>().onObjectSpawned.AddListener(SpawnedMinion);
    }

    void SpawnedMinion(GameObject minion)
    {
        spawned.Add(minion);
        minion.GetComponent<EnemyStats>().onEnemyDeath += SpawnDied;

        if(spawned.Count >= minionMax) root.SetData("canSpawn", false);
    }
    void SpawnDied(EnemyStats deadWisp)
    {
        Debug.Log("test");
        deadWisp.onEnemyDeath -= SpawnDied;
        for (int i = 0; i < spawned.Count; i++)
        {
            if(spawned[i] == null || spawned[i] == deadWisp.gameObject)
            {
                spawned.RemoveAt(i);
                i--;
            }
        }
    }

    public void KillAllMinions()
    {
        while(spawned.Count > 0)
        {
            spawned[0].GetComponent<EnemyStats>().KillEnemy();
        }
    }

    public override void DamagePlayer()
    {
        player.DamagePlayer(stats.GetScaledDamage(beamDamage), stats);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onWispSpawned.RemoveAllListeners();
        onNewMinionWave.RemoveAllListeners();
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        beamTrigger.DrawTrigger(stats.GetPosition());
        Handles.color = Color.yellow;
        Handles.DrawLine(stats.GetPosition() + Vector2.left* patrollDistance, stats.GetPosition() + Vector2.right* patrollDistance);
    }
#endif
}
