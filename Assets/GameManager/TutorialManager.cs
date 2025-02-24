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

    [SerializeField] bool unlockBottleRotation;

    void Start()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath += (EnemyStats _) => { StartRespawn(position, index);};
        }

        if(fillPlayerBottle > 0)
        {
            PlayerLevelMananger.instance.GetComponent<ColorInventory>().AddColor(fillColor, fillPlayerBottle);
        }

        if(unlockBottleRotation) 
        {   
            PlayerLevelMananger.instance.GetComponent<ColorInventory>().lockSwapping = false;
            PlayerLevelMananger.instance.playerUi.UnlockColorSlots();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            if(dummys[i] ==  null) continue;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath -= (EnemyStats _) => { StartRespawn(position, index);};
        }
    }

    private void StartRespawn(Vector3 position, int index)
    {
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath -= (EnemyStats _) => { StartRespawn(position, index);};
        StartCoroutine(Respawn(position, index));
    }

    IEnumerator Respawn(Vector3 position, int index)
    {
        yield return new WaitForSeconds(respawnTime);
        GameObject newDummy = GameObject.Instantiate(dummyTemplate,position, dummyTemplate.transform.rotation) as GameObject;
        newDummy.GetComponent<EnemyStats>().SetColor(GetComponent<EnemyManager>().GetRandomEnemyColor());
        dummys[index] = newDummy;
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath += (EnemyStats _) => { StartRespawn(position, index);};
        yield return null;

    }
}
