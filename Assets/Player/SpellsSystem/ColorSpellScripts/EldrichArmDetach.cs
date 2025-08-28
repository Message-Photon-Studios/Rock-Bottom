using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldrichArmDetach : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) GetComponentInParent<EldrichArmScript>().DetachEnemy(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) GetComponentInParent<EldrichArmScript>().ReAttachEnemy(collision);
    }
}
