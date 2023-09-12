using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int Index;
    public List<Pawn> WaitingPawns;
    public Transform GroundHeightTarget;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private float offset;

    private int pawnCount;
    private int floorCount;
    private Pawn pawnPrefab;

    [SerializeField] private KeyCode spawnPawnKey;


    public void Init(int index, int floorCount, int maxPawns, Pawn pawnPrefab)
    {
        Index = index;
        this.floorCount = floorCount;
        this.pawnPrefab = pawnPrefab;
        
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

    public Pawn TryPickup()
    {
        if (WaitingPawns[0] == null)
            return null;
        
        var pawn = WaitingPawns[0];
        WaitingPawns[0] = null;
        
        for (int i = 1; i < WaitingPawns.Count; ++i)
        {
            if (WaitingPawns[i] == null) break;
            
            WaitingPawns[i - 1] = WaitingPawns[i];
            WaitingPawns[i] = null;
        }

        return pawn;
    }
    
    private void SpawnPawn()
    {
        Debug.Log("Pawn Spawned on floor " + Index);
        if (pawnCount == WaitingPawns.Count)
            GameOver();

        else
        {
            var pawn = Instantiate(pawnPrefab, spawnPos.position + Vector3.left * offset * pawnCount, Quaternion.identity);
            pawn.Init(Random.Range(0, floorCount));
            
            WaitingPawns[pawnCount] = pawn;
            pawnCount += 1;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}