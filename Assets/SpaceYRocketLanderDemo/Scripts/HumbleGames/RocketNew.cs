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

        [Header("Engines")]

        [SerializeField]
        private RocketEngine mainBottomEngine;

        [SerializeField]
        private RocketEngine stopTopEngine;
        
        [SerializeField]
        private RocketEngine bottomLeftEngine;
        
        [SerializeField]
        private RocketEngine bottomRightEngine;
        
        [SerializeField]
        private RocketEngine topLeftEngine;
        
        [SerializeField]
        private RocketEngine topRightEngine;



        [Header("Visualisation")]

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

        [SerializeField]
        private float engineBurstParticlesPlayDuration = 0.3f;

        private Vector3 initialRotation;

        // ------------------------------------------------------------------------------------------------------------
        //                                         Unity Lifecycle
        // ------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            initialRotation = transform.localEulerAngles;
        }

        // ------------------------------------------------------------------------------------------------------------
        //                                             Public API
        // ------------------------------------------------------------------------------------------------------------

        public void ResetRocket()
        {
            transform.localEulerAngles = initialRotation;
            rocketRb.velocity = Vector3.zero;
            rocketRb.angularVelocity = Vector3.zero;
        }

        public void ActivateMainBottomEngine()
        {
            ActivateEngineParticles(particlesMainBottomEngine);
            mainBottomEngine.ApplyEngineForce(rocketRb, powerMainEngine);
        }

        public void ActivateStopTopEngine()
        {
            ActivateEngineParticles(particlesStopTopEngine);
            stopTopEngine.ApplyEngineForce(rocketRb, powerStopEngine);
        }

        public void ActivateAuxiliaryTopLeftEngine()
        {
            ActivateEngineParticles(particlesTopLeftEngine);
            topLeftEngine.ApplyEngineForce(rocketRb, powerSideEngines);
        }

        public void ActivateAuxiliaryTopRightEngine()
        {
            ActivateEngineParticles(particlesTopRightEngine);
            topRightEngine.ApplyEngineForce(rocketRb, powerSideEngines);
        }

        public void ActivateAuxiliaryBottomLeftEngine()
        {
            ActivateEngineParticles(particlesBottomLeftEngine);
            bottomLeftEngine.ApplyEngineForce(rocketRb, powerSideEngines);
        }

        public void ActivateAuxiliaryBottomRightEngine()
        {
            ActivateEngineParticles(particlesBottomRightEngine);
            bottomRightEngine.ApplyEngineForce(rocketRb, powerSideEngines);
        }

        private void ActivateEngineParticles(ParticleSystem particleSystem)
        {
            if (enableParticles)
            {
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