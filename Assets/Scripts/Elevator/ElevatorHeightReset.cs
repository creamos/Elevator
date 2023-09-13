using System.Collections;
using UnityEngine;

public class ElevatorHeightReset : MonoBehaviour
{
    [SerializeField] private KeyCode resetKey = KeyCode.Space;
    [SerializeField] private bool teleportReset;
    private Coroutine resetRoutine;

    [SerializeField] private float resetHeight;

    private void Update()
    {
        if (Input.GetKeyDown(resetKey) && resetRoutine == null)
        {
            if (teleportReset)
            {
                var elevatorTransform = transform;
                var elevatorPos = elevatorTransform.position;
                elevatorPos = new Vector3(elevatorPos.x, resetHeight, elevatorPos.z);
                elevatorTransform.position = elevatorPos;
            }
            else
            {
                resetRoutine = StartCoroutine(ResyncHeightCrt());

                IEnumerator ResyncHeightCrt()
                {
                    float startTime = Time.time;
                    float duration = 0.25f;
                    float progress = 0;

                    Vector3 initialPos = transform.position;
                    Vector3 endPos = new Vector3(initialPos.x, resetHeight, initialPos.z);

                    do
                    {
                        progress = Mathf.Clamp01((Time.time - startTime) / duration);
                        transform.position = Vector3.Lerp(initialPos, endPos, progress);
                        yield return null;
            
                    } while (progress < 1);

                    resetRoutine = null;
                }
            }
        }
    }
}
