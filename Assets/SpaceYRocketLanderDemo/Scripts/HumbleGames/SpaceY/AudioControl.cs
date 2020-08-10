using UnityEngine;

namespace HumbleGames.SpaceY
{
    public class AudioControl : MonoBehaviour
    {
        [SerializeField]
        private AudioSource soundSource;
        
        [SerializeField]
        private AudioClip mainEngineSfx;
        
        [SerializeField]
        private AudioClip sideEngineSfx;
        
        [SerializeField]
        private AudioClip accidentSfx;

        [Tooltip("Don't play rocket engine Sfx more often than (fps)")]
        [SerializeField]
        private int engineSfxMinPlayDelayFps = 15;

        private int engineSfxLastPlayedFrame;

        private void OnEnable()
        {
            EventManager.OnRocketMainEngine += PlaySfxMainEngine;
            EventManager.OnRockeAuxEngine += PlaySfxSideEngine;
            EventManager.OnRockeAccident += PlaySfxAccident;
        }

        private void OnDisable()
        {
            EventManager.OnRocketMainEngine -= PlaySfxMainEngine;
            EventManager.OnRockeAuxEngine -= PlaySfxSideEngine;
            EventManager.OnRockeAccident -= PlaySfxAccident;
        }

        // ------------------------------------- Public API -------------------------------------------

        public void PlaySfxMainEngine()
        {
            PlayAudioClipNotOftenThan(mainEngineSfx, 1f);
        }

        public void PlaySfxSideEngine()
        {
            PlayAudioClipNotOftenThan(sideEngineSfx, 0.375f);
        }

        public void PlaySfxAccident()
        {
            // always play accident sound
            soundSource.PlayOneShot(accidentSfx, 1f);
        }

        // ----------------------------------- Private Helpers ----------------------------------------

        private void PlayAudioClipNotOftenThan(AudioClip clip, float volume)
        {
            int currentFrame = Time.frameCount;

            if (currentFrame - engineSfxLastPlayedFrame > engineSfxMinPlayDelayFps)
            {
                soundSource.PlayOneShot(clip, volume);
                engineSfxLastPlayedFrame = currentFrame;
            }
        }
    }
}