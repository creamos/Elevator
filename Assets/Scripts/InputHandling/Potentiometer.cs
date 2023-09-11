using UnityEngine;

public class Potentiometer : MonoBehaviour
{
    public float Value;

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Value = 1;
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Value = -1;
        }

        else
        {
            Value = 0;
        }
    }
}