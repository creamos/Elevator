using System.Collections;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class ElevatorHeightReset : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent resetElevatorHeightRequest; 
    
    [SerializeField] private KeyCode resetKey = KeyCode.Space;
    [SerializeField] private bool teleportReset;
    private Coroutine resetRoutine;

    [SerializeField] private float resetHeight;

    private void OnEnable()
    {
        if (resetElevatorHeightRequest)
        {
            resetElevatorHeightRequest.OnTriggered -= OnResetRequest;
            resetElevatorHeightRequest.OnTriggered += OnResetRequest;
        }
    }

    private void OnDestroy()
    {
        if (resetElevatorHeightRequest) 
            resetElevatorHeightRequest.OnTriggered -= OnResetRequest;
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetKey))
            OnResetRequest();
    }

    private void OnResetRequest()
    {
        if(resetRoutine == null)
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
                    
                    Quaternion initialRot = transform.rotation;
                    Quaternion endRot = Quaternion.identity;

                    do
                    {
                        progress = Mathf.Clamp01((Time.time - startTime) / duration);
                        transform.position = Vector3.Lerp(initialPos, endPos, progress);
                        transform.rotation = Quaternion.Lerp(initialRot, endRot, progress);
                        yield return null;
                    } while (progress < 1);

                    resetRoutine = null;
                }
            }
        }
    }
}
