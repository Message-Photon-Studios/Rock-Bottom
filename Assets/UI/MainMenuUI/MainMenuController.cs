using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    //Button to start the navigation at when starting the game.
    [SerializeField] Button startButton;
    [SerializeField] Image fadeToBlackImg;
    [SerializeField] Image sylviaLoading;

    [SerializeField] Sprite[] LoadingSprites;

    private void OnEnable() {
        sylviaLoading.gameObject.SetActive(false);
        StartCoroutine(FadeOutCoroutine(true));
        startButton.GetComponent<IndicatorController>().ShowIndicator(true);
    }

    //Load scene "Gem" when pressed.
    public void LoadScene()
    {
        StartCoroutine(FadeOutCoroutine(false));
    }

    //Exit application when pressed.
    public void ExitGame() {
        Application.Quit();
        Debug.Log("Exit");
    }

    IEnumerator FadeOutCoroutine(bool fadeIn)
    {
        int direction = fadeIn ? -1 : 1;
        fadeToBlackImg.color = new Color(0, 0, 0, fadeIn ? 1 : 0);
        while ((fadeToBlackImg.color.a < 1 && !fadeIn) || (fadeToBlackImg.color.a > 0 && fadeIn))
        {
            fadeToBlackImg.color = new Color(0, 0, 0, fadeToBlackImg.color.a + Time.deltaTime * direction);
            yield return new WaitForEndOfFrame();
        }

        if (fadeIn)
            yield break;

        SceneManager.LoadSceneAsync("Tutorial");

        int count = 0;
        sylviaLoading.gameObject.SetActive(true);
        while (true)
        {
            sylviaLoading.sprite = LoadingSprites[count];
            count = (count + 1) % LoadingSprites.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
