using HumbleGames.SpaceY.Simulation;
using HumbleGames.SpaceY.Utils;
using UnityEngine;

namespace HumbleGames.SpaceY.MLAgents
{

    public class LegLandingProbe : MonoBehaviour
    {
        public enum LandingProbeType
        {
            LeftLandingProbe,
            RightLandingProbe
        }

        [SerializeField]
        private LandingProbeType landingProbeType;

        [SerializeField]
        private SimulationState simulationState;

        [SerializeField]
        private TagHolder tagHolder;

        private void OnTriggerExit(Collider other) => HandleTriggerExit(other);

        private void OnTriggerEnter(Collider other) => HandleTriggerEnterOrStay(other);

        private void OnTriggerStay(Collider other) => HandleTriggerEnterOrStay(other);

        // TODO: refactor
        private void HandleTriggerEnterOrStay(Collider collider)
        {
            if (collider.CompareTag(tagHolder.basePlanet))
            {
                if (landingProbeType == LandingProbeType.LeftLandingProbe)
                {
                    simulationState.isLeftLegLandedOnBasePlanet = true;
                }

                if (landingProbeType == LandingProbeType.RightLandingProbe)
                {
                    simulationState.isRightLegLandedOnBasePlanet = true;
                }
            }
            
            if (collider.CompareTag(tagHolder.targetPlanet))
            {
                if (landingProbeType == LandingProbeType.LeftLandingProbe)
                {
                    simulationState.isLeftLegLandedOnTargetPlanet = true;
                }

                if (landingProbeType == LandingProbeType.RightLandingProbe)
                {
                    simulationState.isRightLegLandedOnTargetPlanet = true;
                }
            }
        }

        // TODO: refactor
        private void HandleTriggerExit(Collider collider)
        {
            if (collider.CompareTag(tagHolder.basePlanet))
            {
                if (landingProbeType == LandingProbeType.LeftLandingProbe)
                {
                    simulationState.isLeftLegLandedOnBasePlanet = false;
                }

                if (landingProbeType == LandingProbeType.RightLandingProbe)
                {
                    simulationState.isRightLegLandedOnBasePlanet = false;
                }
            }

            if (collider.CompareTag(tagHolder.targetPlanet))
            {
                if (landingProbeType == LandingProbeType.LeftLandingProbe)
                {
                    simulationState.isLeftLegLandedOnTargetPlanet = false;
                }

                if (landingProbeType == LandingProbeType.RightLandingProbe)
                {
                    simulationState.isRightLegLandedOnTargetPlanet = false;
                }
            }
        }
    }
}