using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] float respawnTime;

    [SerializeField] GameObject dummyTemplate;
    [SerializeField] GameObject[] dummys;
    [SerializeField] int fillPlayerBottle;
    [SerializeField] GameColor fillColor;

    GameColor[] dummyColors;

    [SerializeField] bool unlockBottleRotation;

    void Start()
    {
        dummyColors = new GameColor[dummys.Length];
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath += (EnemyStats _) => { StartRespawn(position, _.GetColor(), _.lookDir, index); };
            dummyColors[i] = dummys[i].GetComponent<EnemyStats>().GetColor();
        }

        if(fillPlayerBottle > 0)
        {
            Player.instance.GetComponent<ColorInventory>().AddColor(fillColor, fillPlayerBottle, 0);
        }

        if(unlockBottleRotation) 
        {   
            Player.instance.GetComponent<ColorInventory>().lockSwapping = false;
            Player.instance.playerUi.UnlockColorSlots();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            if(dummys[i] ==  null) continue;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath -= (EnemyStats _) => { StartRespawn(position, _.GetColor(), _.lookDir, index);};
        }
    }

    private void StartRespawn(Vector3 position, GameColor color, float lookDir, int index)
    {
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath -= (EnemyStats _) => { StartRespawn(position, _.GetColor(), _.lookDir,  index);};
        StartCoroutine(Respawn(position, dummyColors[index], lookDir, index));
    }

    IEnumerator Respawn(Vector3 position, GameColor color, float lookDir,  int index)
    {
        yield return new WaitForSeconds(respawnTime);
        GameObject newDummy = GameObject.Instantiate(dummyTemplate, position, dummyTemplate.transform.rotation) as GameObject;
        newDummy.GetComponent<Enemy>().SetupEnemy();
        newDummy.GetComponent<EnemyStats>().SetColorByHand(color);
        if (newDummy.GetComponent<EnemyStats>().lookDir != lookDir) newDummy.GetComponent<EnemyStats>().ChangeDirection();
        dummys[index] = newDummy;
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath += (EnemyStats _) => { StartRespawn(position, color, lookDir, index);};
        yield return null;

    }
}
