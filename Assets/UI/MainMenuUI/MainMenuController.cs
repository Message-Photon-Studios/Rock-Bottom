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
    [SerializeField] Button settingsButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    [SerializeField] Image fadeToBlackImg;
    [SerializeField] Image sylviaLoading;
    [SerializeField] Image mainMenuTitle;

    [SerializeField] AnimationCurve titleCurve;

    [SerializeField] IndicatorController startController;
    [SerializeField] IndicatorController settingstController;
    [SerializeField] IndicatorController creditsController;
    [SerializeField] IndicatorController exitController;

    [SerializeField] AnimationCurve indicatorCurve;

    [SerializeField] Camera cam;

    [SerializeField] Sprite[] LoadingSprites;

    [SerializeField] GameObject credits;

    [HideInInspector] public bool areCreditsOpen = false;

    private void OnEnable()
    {
        sylviaLoading.gameObject.SetActive(false);
        StartCoroutine(FadeOutCoroutine(true));
        StartCoroutine(tiltCamera());
        startButton.GetComponent<IndicatorController>().ShowIndicator(true);

        DontDestroy bgMusic = GameObject.FindObjectOfType<DontDestroy>();
        bgMusic.enableChildren();
    }

    //Load scene "Gem" when pressed.
    public void LoadScene()
    {
        StartCoroutine(FadeOutCoroutine(false));
    }

    //Exit application when pressed.
    public void ExitGame()
    {
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
        {
            StartCoroutine(initTitle());
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(startController.appear(indicatorCurve));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(settingstController.appear(indicatorCurve));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(creditsController.appear(indicatorCurve));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(exitController.appear(indicatorCurve));
            yield break;
        }

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

    private IEnumerator initTitle()
    {
        for (float i = 0; i < 1; i += 0.013f)
        {
            mainMenuTitle.rectTransform.anchoredPosition = new Vector2(
                mainMenuTitle.rectTransform.anchoredPosition.x, 
                Mathf.Lerp(215, 856, titleCurve.Evaluate(i))
            );
            
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator tiltCamera()
    {
        // Tilt the worldUp of the camera in a sinewave pattern
        var centerPos = cam.transform.position.x;
        while (true)
        {
            var sinVal = (Mathf.Sin(Time.time / 3) - 0.5f) * 0.15f;
            cam.transform.eulerAngles = new Vector3(0, 0, sinVal);
            cam.transform.position = new Vector3(centerPos - sinVal, cam.transform.position.y, cam.transform.position.z);
            yield return new WaitForSeconds(0.03f);
        }   
    }

    public void showCredits()
    {
        startButton.interactable = false;
        settingsButton.interactable = false;
        creditsButton.interactable = false;
        exitButton.interactable = false;

        areCreditsOpen = true;
        credits.SetActive(true);
    }

    public void hideCredits()
    {
        startButton.interactable = true;
        settingsButton.interactable = true;
        creditsButton.interactable = true;
        exitButton.interactable = true;
        
        creditsButton.Select();

        areCreditsOpen = false;
        credits.SetActive(false);
    }
}
