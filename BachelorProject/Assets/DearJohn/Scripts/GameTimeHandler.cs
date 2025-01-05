using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeHandler : MonoBehaviour
{
    public static GameTimeHandler Instance { get; private set; }
    public float currentGameTime = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        currentGameTime += Time.deltaTime;
    }
}
