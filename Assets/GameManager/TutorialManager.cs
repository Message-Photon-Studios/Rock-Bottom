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

    void Start()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath += () => { StartRespawn(position, index);};
        }

    }

    void OnDisable()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            int index = i;
            Vector3 position = dummys[i].transform.position;
            dummys[i].GetComponent<EnemyStats>().onEnemyDeath -= () => { StartRespawn(position, index);};
        }
    }

    private void StartRespawn(Vector3 position, int index)
    {
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath -= () => { StartRespawn(position, index);};
        StartCoroutine(Respawn(position, index));
    }

    IEnumerator Respawn(Vector3 position, int index)
    {
        yield return new WaitForSeconds(respawnTime);
        GameObject newDummy = GameObject.Instantiate(dummyTemplate,position, dummyTemplate.transform.rotation) as GameObject;
        newDummy.GetComponent<EnemyStats>().SetColor(GetComponent<EnemyManager>().GetRandomEnemyColor());
        dummys[index] = newDummy;
        dummys[index].GetComponent<EnemyStats>().onEnemyDeath += () => { StartRespawn(position, index);};
        yield return null;

    }
}
