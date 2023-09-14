using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class ElevatorGameStartBehaviour : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent GameStarted;

    [SerializeField]
    private Elevator elevator;
    
    private void OnEnable()
    {
        if (GameStarted)
        {
            GameStarted.OnTriggered -= OnGameStarted;
            GameStarted.OnTriggered += OnGameStarted;
        }
    }

    private void OnDestroy()
    {
        if (GameStarted) GameStarted.OnTriggered -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        elevator.EnablePlayerInputs(true);
    }
}
