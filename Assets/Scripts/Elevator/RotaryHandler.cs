using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaryHandler : MonoBehaviour
{
    [SerializeField] private SerialCommunication serialCommunication;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private bool debugMode = false;
    
    private float acceleration = 0f;
    private int previousRotatorValue = 0;

    public float Acceleration => acceleration;

    void Update()
    {
        
        float rotatorDelta = serialCommunication.RotatorValue - previousRotatorValue;
        acceleration = rotatorDelta * sensitivity;

        previousRotatorValue = serialCommunication.RotatorValue;

        if (!debugMode)
            return;
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            acceleration = 10;
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            acceleration = -10;
        }

        else
        {
            acceleration = 0;
        }

    }
}
