using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] InputActionReference interactAction;
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] [TextArea(10,40)] string[] tutorialTexts;
    int textNr = 0;

    [SerializeField] GameObject dummyTemplate;
    [SerializeField] GameObject[] dummys;

    Vector3[] spawnPoints = new Vector3[0];

    void Start()
    {
        spawnPoints = new Vector3[dummys.Length];
        for (int i = 0; i < dummys.Length; i++)
        {
            spawnPoints[i] = dummys[i].transform.position;
        }

        tutorialText.text = tutorialTexts[0];
    }

    void OnEnable()
    {
        interactAction.action.performed += NextTutorial;
    }

    void OnDisable()
    {
        interactAction.action.performed -= NextTutorial;
    }

    void Update()
    {
        for (int i = 0; i < dummys.Length; i++)
        {
            if(!dummys[i])
            {
                GameObject newDummy = GameObject.Instantiate(dummyTemplate,spawnPoints[i], dummyTemplate.transform.rotation) as GameObject;
                newDummy.GetComponent<EnemyStats>().SetColor(GetComponent<EnemyManager>().GetRandomEnemyColor());
                dummys[i] = newDummy;
            }
        }
    }

    void NextTutorial(InputAction.CallbackContext ctx)
    {
        textNr ++;
        if(tutorialTexts.Length <= textNr)
        {
            tutorialCanvas.SetActive(false);
            return;
        }

        tutorialText.text = tutorialTexts[textNr];
    }
}