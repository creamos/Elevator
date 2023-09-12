using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Elevator : MonoBehaviour
{
    [SerializeField] private JoyconHandler joyconHandler;
    [SerializeField] private RotaryHandler rotaryHandler;

    [SerializeField] private float speedMod = 1;

    private void Update()
    {
        transform.position += Vector3.up * (Time.deltaTime * rotaryHandler.CurrentAcceleration);
    }
}
