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
            if (playSoundEvent)
            {
                playSoundEvent.OnTriggered += PlaySound;
            }
        }

        public void PlaySound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(soundRef);
        }
    }
}