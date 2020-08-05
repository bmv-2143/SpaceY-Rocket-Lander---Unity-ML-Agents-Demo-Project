using System.Collections;
using UnityEngine;

namespace HumbleGames
{
    public class RocketNew : MonoBehaviour
    {
        // TODO: move into a separate RocketConfig Scriptable Object

        [SerializeField]
        private bool enableParticles = true;

        [SerializeField]
        private bool useGravity = true;

        [SerializeField]
        private float powerMainEngine = 14f;

        [SerializeField]
        private float powerStopEngine = 7f;

        [SerializeField]
        private float powerSideEngines = 0.2f;

        [SerializeField]
        private Rigidbody rocketRb;

        [SerializeField]
        private ParticleSystem particlesMainBottomEngine;

        [SerializeField]
        private ParticleSystem particlesStopTopEngine;

        [SerializeField]
        private ParticleSystem particlesBottomLeftEngine;

        [SerializeField]
        private ParticleSystem particlesBottomRightEngine;

        [SerializeField]
        private ParticleSystem particlesTopLeftEngine;

        [SerializeField]
        private ParticleSystem particlesTopRightEngine;

        [Header("Visualisation")]

        [SerializeField]
        private float engineBurstParticlesPlayDuration = 0.3f;




        private Coroutine engineParticlesEmissionCoroutine;

        // ------------------------------------------------------------------------------------------------------------
        //                                             Public API
        // ------------------------------------------------------------------------------------------------------------

        public void Initialize()
        {

        }

        public void ResetRocket()
        {
        
        }

        public void ActivateMainBottomEngine()
        {
            ActivateEngineParticles(particlesMainBottomEngine);
        }

        public void ActivateStopTopEngine()
        {
            ActivateEngineParticles(particlesStopTopEngine);
        }

        public void ActivateAuxiliaryTopLeftEngine()
        {
            ActivateEngineParticles(particlesTopLeftEngine);
        }

        public void ActivateAuxiliaryTopRightEngine()
        {
            ActivateEngineParticles(particlesTopRightEngine);
        }

        public void ActivateAuxiliaryBottomLeftEngine()
        {
            ActivateEngineParticles(particlesBottomLeftEngine);
        }

        public void ActivateAuxiliaryBottomRightEngine()
        {
            ActivateEngineParticles(particlesBottomRightEngine);
        }

        private void ActivateEngineParticles(ParticleSystem particleSystem)
        {
            if (enableParticles)
            {
                if (engineParticlesEmissionCoroutine != null)
                {
                    StopCoroutine(engineParticlesEmissionCoroutine);
                }

                particleSystem.Play();
                StartCoroutine(PlayParticlesForDuration(particleSystem, engineBurstParticlesPlayDuration));
            }
        }

        private IEnumerator PlayParticlesForDuration(ParticleSystem particlesToStop, float playDuration)
        {
            yield return new WaitForSeconds(playDuration);
            particlesToStop.Stop();
        }
    }
}