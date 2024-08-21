using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// This class controls the cameras movement
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed; //The horizontal speed of the camera
    [SerializeField] float verticalSpeedUp; //The vertical speed of the camera going up
    [SerializeField] float verticalSpeedDown; //The vertical speed of the camera going dow
    [SerializeField] Transform focusPoint; //The point that the camera will try and follow
    [SerializeField] Vector2 deadZone;

    [SerializeField] PlayerStats player; // Sylvia, Needed to change the vingette effect when health is low

    [Range(0, 1)]
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] AnimationCurve beatCurve;

    [Space(10)]
    [SerializeField] Color vignetteNormalColor; // Normal vingette
    [SerializeField] float vignetteNormalIntensity;
    [SerializeField] float vignetteNormalSmoothness;
    [Space(10)]
    [SerializeField] Color vignetteCritColor; // Intense vingette
    [SerializeField] float vignetteCritIntensity;
    [SerializeField] float vignetteCritSmoothness;

    private bool isSuffering; // Used to check if the player is suffering from low health
    private Vignette vignette;
    private float timer = 0;

    private Vector3 startPos;
    private Vector3 focusStartPos;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        startPos = transform.position;
        focusStartPos = focusPoint.position;
        var ratio = Screen.height / 1080.0;
        if (Math.Floor(ratio) > 0)
            ratio /= Math.Floor(ratio);
        ratio *= 100;
        
        var pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        pixelPerfectCamera.assetsPPU = (int)Math.Floor(ratio);
        vignette = GetComponentInChildren<Volume>().profile.components[1] as Vignette;
        setIntenseVingette(false);
    }

    /// <summary>
    /// Is called when a new level starts
    /// </summary>
    public void SetStartLevel()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "PlayerColorsCombatTest" || SceneManager.GetActiveScene().name == "TutorialCaves") return;
        focusPoint.position = focusStartPos;
        transform.position = startPos;
        focusPoint.GetComponent<CameraFocus>().SetStartLevel();
    }

    private void FixedUpdate() {

        Vector3 movePos = transform.position;
        Vector3 moveVerticalPos = transform.position;
        if(focusPoint.position.x > deadZone.x+transform.position.x || focusPoint.position.x < -deadZone.y+transform.position.x)
            movePos = Vector3.Slerp(transform.position, focusPoint.position, speed*Time.fixedDeltaTime);
        if(focusPoint.position.y > deadZone.y+transform.position.y || focusPoint.position.y < -deadZone.y+transform.position.y)
            moveVerticalPos = Vector3.Slerp(transform.position, focusPoint.position, ((focusPoint.position.y > transform.position.y)? verticalSpeedUp:verticalSpeedDown)*Time.fixedDeltaTime);

        transform.position = new Vector3(movePos.x, moveVerticalPos.y, transform.position.z);

        // Get health from the player
        float health = player.GetHealth();
        if (isSuffering && !(player.GetMaxHealth() * lowHealthThreshold > health))
        {
            setIntenseVingette(false);
        }
        else if (!isSuffering && player.GetMaxHealth() * lowHealthThreshold > health)
        {
            setIntenseVingette(true);
        }

        if (isSuffering)
        {
            timer += Time.fixedDeltaTime;
            vignette.smoothness.value = vignetteCritSmoothness + beatCurve.Evaluate(timer) * 0.2f;
            if (timer >= 1) 
                timer = 0;
        }
    }

    private void setIntenseVingette(bool set)
    {
        // Get vingette effect from global volume in children
        if (vignette != null)
        {
            if (set)
            {
                timer = 0;
                vignette.color.value = vignetteCritColor;
                vignette.intensity.value = vignetteCritIntensity;
                vignette.smoothness.value = vignetteCritSmoothness;
                isSuffering = true;
            }
            else
            {
                vignette.color.value = vignetteNormalColor;
                vignette.intensity.value = vignetteNormalIntensity;
                vignette.smoothness.value = vignetteNormalSmoothness;
                isSuffering = false;
            }
        }
    }
}
