using System;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private float speedMod = 1;

    private void Update()
    {
        float accel = inputHandler.Acceleration;
        transform.position += Vector3.up * accel * Time.deltaTime * speedMod;
    }
}
