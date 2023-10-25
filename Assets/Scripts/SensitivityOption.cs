using UnityEngine;


public class SensitivityOption : MonoBehaviour
{
     [SerializeField] private RotaryHandler rotaryHandler;
     
     public void AddSensitivity(float value)
     {
          rotaryHandler.Sensitivity += value;
     }
}
