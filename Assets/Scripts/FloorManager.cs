using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public int MaxPawnPerFloor;
    
    public List<Floor> Floors;

    private void Start()
    {
        for(int i = 0; i < Floors.Count; ++i)
        {
            Floors[i].Init(i, Floors.Count, MaxPawnPerFloor);
        }
    }
}