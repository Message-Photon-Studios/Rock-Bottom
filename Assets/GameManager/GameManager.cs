using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MaskLibrary maskLibrary;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }
    
    [System.Serializable]
    public struct MaskLibrary {
        public LayerMask onlyGround;
        public LayerMask onlyEnemy;
        public LayerMask onlyPlayer;
    }
}
