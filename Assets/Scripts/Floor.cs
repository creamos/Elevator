using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int Index;
    public List<Pawn> WaitingPawns;

    private int pawnCount;
    private int floorCount;

    [SerializeField] private KeyCode spawnPawnKey;

    public void Init(int index, int floorCount, int maxPawns)
    {
        Index = index;
        this.floorCount = floorCount;
        pawnCount = 0;
        
        WaitingPawns = new List<Pawn>(maxPawns);
        for (int i = 0; i < maxPawns; ++i)
            WaitingPawns.Add(null);
    }

    public void Update()
    {
        if (Input.GetKeyDown(spawnPawnKey))
        {
            SpawnPawn();
        }
    }

    private void SpawnPawn()
    {
        Debug.Log("Pawn Spawned on floor " + Index);
        if (pawnCount == WaitingPawns.Count)
            GameOver();

        else
        {
            WaitingPawns[pawnCount] = new Pawn(Random.Range(0, floorCount));
            pawnCount += 1;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}