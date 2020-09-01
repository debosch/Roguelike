using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject gameBehavior;
    
    private void Awake()
    {
        if (GameBehavior.Instance == null)
            Instantiate(gameBehavior);
    }
}
