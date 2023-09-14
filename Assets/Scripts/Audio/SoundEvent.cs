using FMOD.Studio;
using FMODUnity;
using ScriptableEvents;
using UnityEngine;

namespace Audio
{
    public class SoundEvent : MonoBehaviour
    {
        [SerializeField] private EventReference soundRef;
        [SerializeField] private GameEvent playSoundEvent;

        private bool playOnStart = false;
        
        private EventInstance soundInstance;

        private void Start()
        {
            if (playSoundEvent != null)
            {
                playSoundEvent.OnTriggered += PlaySound;
            }
        }

        public void PlaySound()
        {
            Debug.Log("Play sound " + gameObject.name);
            FMODUnity.RuntimeManager.PlayOneShot(soundRef);
        }
    }
}