using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Pawn : MonoBehaviour
{
    public int Destination;
    public bool IsInElevator;

    public void Init(int destination)
    {
        Destination = destination;
    }

    /// <summary>
    /// Called when the pawn is added to the elevator
    /// </summary>
    public void GetInElevator()
    {
        IsInElevator = true;
        StartCoroutine(JumpInElevator());
    }

    /// <summary>
    /// Called when the pawn in the elevator reaches the desired floor
    /// </summary>
    public void Release()
    {
        IsInElevator = false;
        //StartCoroutine(JumpOutElevator());
    }

    private IEnumerator JumpInElevator()
    {
        yield return JumpTo(ElevatorInfo.Instance.SeatTarget, 1f, 0.5f);
        transform.parent = ElevatorInfo.Instance.transform;
    }
    
    private IEnumerator JumpTo(Transform target, float height, float duration)
    {
        Vector3 initialPos = transform.position;

        float startTime = Time.time;
        float progress = 0;

        do
        {
            progress = (Time.time - startTime) / duration;
            progress = Mathf.Clamp01(progress);

            transform.position = Vector3.Lerp(initialPos, target.position, progress);
            if (progress <= 0.5f)
                transform.position += Vector3.up * Mathf.SmoothStep(0, 1, progress * 2f);
            else
                transform.position += Vector3.up * Mathf.SmoothStep(1, 0, (progress - 0.5f) * 2f);

            yield return null;
            
        } while (progress < 1);
    }
}