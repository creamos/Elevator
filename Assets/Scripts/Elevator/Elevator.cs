using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class Elevator : MonoBehaviour
{
    [SerializeField] private JoyconHandler joyconHandler;
    [SerializeField] private RotaryHandler rotaryHandler;

    [ShowNonSerializedField] private bool isReadingInput;

    public void EnablePlayerInputs(bool state) => isReadingInput = state;

    private void Update()
    {
        if (!isReadingInput) return;
        
        transform.position += Vector3.up * (Time.deltaTime * rotaryHandler.CurrentAcceleration);
        transform.rotation = Quaternion.Euler(joyconHandler.JoyconRotation);
    }
}
