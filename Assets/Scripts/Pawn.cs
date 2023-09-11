using System;
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
}