using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent GameOver;

    public static FloorManager Instance { get; private set; }

    public int MaxPawnPerFloor;
    public List<Floor> Floors;

    [SerializeField] private Pawn pawnPrefab;

    private void OnEnable()
    {
        if (GameOver)
        {
            GameOver.OnTriggered -= OnGameOver;
            GameOver.OnTriggered += OnGameOver;
        }
    }

    private void OnDestroy()
    {
        if (GameOver)
            GameOver.OnTriggered -= OnGameOver;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < Floors.Count; ++i)
        {
            Floors[i].Init(i, Floors.Count, MaxPawnPerFloor, pawnPrefab);
        }
    }

    private void OnGameOver()
    {
        foreach(Floor floor in Floors)
            floor.ResetFloor();
    }
}