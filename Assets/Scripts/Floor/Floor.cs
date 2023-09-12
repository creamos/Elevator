using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int Index;
    public List<Pawn> WaitingPawns;
    public Transform GroundHeightTarget, ExitElevatorTarget, ExitFloorTarget;

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
        pawnCount--;
        
        ShiftQueueContent();
        
        //MovePawns();

        if (WaitingPawns[0] != null) WaitingPawns[0].ShowDestinationBubble();

        return pawn;
    }

    private void ShiftQueueContent()
    {
        for (int i = 1; i < WaitingPawns.Count; ++i)
        {
            if (WaitingPawns[i] == null) break;

            WaitingPawns[i - 1] = WaitingPawns[i];
            WaitingPawns[i] = null;
        }
    }

    public void SpawnPawn()
    {
        if (pawnCount == WaitingPawns.Count)
            GameOver();

        else
        {
            var pawn = Instantiate(pawnPrefab, spawnPos.position + offset * pawnCount * Vector3.left, Quaternion.identity);

            int destination = Random.Range(0, floorCount-1);
            if (destination >= Index) destination++;
            pawn.Init(destination);

            if (pawnCount == 0) pawn.ShowDestinationBubble();
            WaitingPawns[pawnCount] = pawn;
            pawnCount += 1;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}