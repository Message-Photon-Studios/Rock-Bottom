using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntrence : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    bool collapsed = false;

    void OnEnable()
    {
        if(!collapsed)
            StartCoroutine(CollapseEntrance());
        else
            animator.SetBool("Collapsed", true);
    }
    IEnumerator CollapseEntrance()
    {
        yield return new WaitForSeconds(4f);
        animator.SetBool("Collapsed", true);
        audioSource.Play();
        collapsed = true;
        yield return null;
    }
}
