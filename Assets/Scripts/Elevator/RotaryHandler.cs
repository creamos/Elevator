using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaryHandler : MonoBehaviour
{
    [SerializeField] private SerialCommunication serialCommunication;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float sharpness = 1f;
    [SerializeField] private float upBias = 1f;
    [SerializeField] private float downBias = 1f;
    [SerializeField] private bool debugMode = false;
    
    private float currentAcceleration = 0f;
    private float targetAcceleration = 0f;
    
    private int previousRotatorValue = 0;

    public float CurrentAcceleration => currentAcceleration;

    void Update()
    {
        
        float rotatorDelta = serialCommunication.RotatorValue - previousRotatorValue;
        
        if(rotatorDelta > 0)
            rotatorDelta *= upBias;
        else if(rotatorDelta < 0)
            rotatorDelta *= downBias;
        
        targetAcceleration = rotatorDelta * sensitivity;
        currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * sharpness);
        
        previousRotatorValue = serialCommunication.RotatorValue;

        if (!debugMode)
            return;
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentAcceleration = 10;
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentAcceleration = -10;
        }

        else
        {
            currentAcceleration = 0;
        }

    }
}
