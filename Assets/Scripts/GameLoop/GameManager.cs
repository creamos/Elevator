using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;

    private void Start()
    {
        spawnManager.StartSpawnLoop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(spawnManager.IsRunning) spawnManager.StopSpawnLoop();
            else spawnManager.StartSpawnLoop();
        }
    }
}