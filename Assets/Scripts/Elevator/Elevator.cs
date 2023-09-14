using NaughtyAttributes;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private JoyconHandler joyconHandler;
    [SerializeField] private RotaryHandler rotaryHandler;

    [ShowNonSerializedField] private bool isReadingInput;
    [field: SerializeField] public Transform PivotPoint { get; private set; }

    public void EnablePlayerInputs(bool state) => isReadingInput = state;

    private void Update()
    {
        if (!isReadingInput) return;
        
        transform.position += Vector3.up * (Time.deltaTime * rotaryHandler.CurrentAcceleration);
        
        //transform.rotation = Quaternion.Euler(joyconHandler.JoyconRotation);
        PivotPoint.rotation = joyconHandler.JoyconRotation;
    }
}
