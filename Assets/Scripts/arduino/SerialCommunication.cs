using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class SerialCommunication : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    
    private int rotatorValue;
    public int RotatorValue => rotatorValue;

    public void OnSerialValues(string[] args)
    {
        rotatorValue = int.Parse(args[0]);
        if(debugMode)
            Debug.Log(args[0]);
    }
}
