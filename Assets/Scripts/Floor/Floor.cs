using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public int Index;
    public List<Pawn> WaitingPawns;
    public Transform GroundHeightTarget, ExitElevatorTarget, ExitFloorTarget;

    [SerializeField] private Transform waitingPos, spawnPos;
    [SerializeField] private float offset;

    private int pawnCount;
    private int floorCount;
    private Pawn pawnPrefab;

    public Vector3 GetWaitingPos(int positionInQueue) => offset * positionInQueue * Vector3.left + waitingPos.position;

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

    public Pawn TryPickup()
    {
        if (WaitingPawns[0] == null || !WaitingPawns[0].MovementInQueueBehaviour.WaitingPosReached)
            return null;
        
        var pawn = WaitingPawns[0];
        WaitingPawns[0] = null;
        pawnCount--;
        
        ShiftQueueContent();
        
        MovePawns();

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
            var pawn = Instantiate(pawnPrefab, spawnPos.position, Quaternion.identity);

            int destination = Random.Range(0, floorCount-1);
            if (destination >= Index) destination++;
            pawn.Init(destination);
            
            WaitingPawns[pawnCount] = pawn;
            pawn.MovementInQueueBehaviour.SetWaitingSlot(GetWaitingPos(pawnCount), pawnCount);
            
            pawnCount += 1;
            
        }
    }

    private void MovePawns()
    {
        for (int pawnIndex = 0; pawnIndex < pawnCount; pawnIndex++)
        {
            var pawn = WaitingPawns[pawnIndex];
            pawn.MovementInQueueBehaviour.SetWaitingSlot(GetWaitingPos(pawnIndex), pawnIndex);
            
            
        }
    }
    
    private void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}