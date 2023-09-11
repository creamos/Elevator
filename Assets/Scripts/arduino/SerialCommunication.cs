using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class SerialCommunication : MonoBehaviour
{
    public void OnSerialValues(string[] args)
    {
        foreach (var arg in args)
        {
            Debug.Log(arg);
        }
    }
}
