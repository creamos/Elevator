using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance { get; private set; }
    
    public int MaxPawnPerFloor;
    public List<Floor> Floors;

    [SerializeField] private Pawn pawnPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < Floors.Count; ++i)
        {
            Floors[i].Init(i, Floors.Count, MaxPawnPerFloor, pawnPrefab);
        }
    }
}