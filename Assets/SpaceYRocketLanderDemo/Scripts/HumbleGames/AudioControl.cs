using UnityEngine;

namespace HumbleGames
{
    public class AudioControl : MonoBehaviour
    {

        public AudioSource soundSource;

        public AudioClip mainEngineSfx;
        public AudioClip sideEngineSfx;
        public AudioClip accidentSfx;

        private const int FRAMES_TO_WAIT = 15;
        private int mainEngineClipPlayedFrame;

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

            if (currentFrame - mainEngineClipPlayedFrame > FRAMES_TO_WAIT)
            {
                soundSource.PlayOneShot(clip, volume);
                mainEngineClipPlayedFrame = currentFrame;
            }
        }
    }
}