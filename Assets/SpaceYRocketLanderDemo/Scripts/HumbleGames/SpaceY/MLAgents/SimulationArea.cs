using HumbleGames.SpaceY.Attributes;
using HumbleGames.SpaceY.Simulation;
using RoboRyanTron.QuickButtons;
using UnityEngine;
using UnityEngine.Assertions;

namespace HumbleGames.SpaceY.MLAgents
{
    public class SimulationArea : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SimulationArea).Name;

        [Header("Death Zones")]

        #region Inspector Visible Members
        [SerializeField]
        private GameObject deathZone;

        [SerializeField]
        private GameObject deathZoneTop;

        [SerializeField]
        private GameObject deathZoneBottom;

        [SerializeField]
        private GameObject deathZoneLeft;

        [SerializeField]
        private GameObject deathZoneRight;

        [Space]

        [SerializeField]
        private float deathZoneWidth = 64f;

        [SerializeField]
        private float deathZoneHeight = 40f;

        //public QuickButton UpdateDeathZones = new QuickButton("UpdateDeathZoneSizesAndPositions");
        public QuickButton UpdateDeathZones = new QuickButton(nameof(classHelper.UpdateDeathZoneSizesAndPositions));

        [Header("Planet Designation Area")]

        [SerializeField]
        private float planetDesignationAreaWidth = 40;

        [SerializeField]
        private float planetDesignationAreaHeight = 25;

        [SerializeField]
        private bool showPlanetDesignationArea = true;

        [Header("Planets")]

        [SerializeField]
        private GameObject basePlanet;
        
        [SerializeField]
        private GameObject targetPlanet;

        public QuickButton UpdatePlanetsPos = new QuickButton(nameof(classHelper.DesignatePlanets));

        [Header("Rocket")]

        [SerializeField]
        private GameObject rocket;

        [SerializeField]
        private GameObject basePlanetBody;
        
        /// <summary>
        /// This offset depends on the size of a specific rocket and a specific planet body. It is used to
        /// precisely position rocket on the planet.
        /// </summary>
        [SerializeField]
        private float rocketToBasePlanetBodyOffset = 1;

        public QuickButton PutRocketOnBlasePlanet = new QuickButton(nameof(classHelper.DesignateRocketOnBlasePlanet));

        [Space]

        [SerializeField]
        private GameObject targetPlanetBody;

        [SerializeField]
        private float rocketToTargetPlanetBodyMinOffset = 1;
        
        [SerializeField]
        private float rocketToTargetPlanetBodyMaxOffset = 1;

        [ReadOnly]
        [SerializeField]
        private float angleDeg;

        [ReadOnly]
        [SerializeField]
        private float rocketPositioningAngleDistortion;

        [SerializeField]
        private float rocketNearTargetPlanetProbability = 50;

        public QuickButton PutRocketNearTargetPlanet = new QuickButton(nameof(classHelper.DesignateRocketNearTargetPlanet));

        #endregion

        private float minDistanceBetweenPlanets;

        private const SimulationArea classHelper = null;

        private float deathZoneBorderThickness = 0.05f;

        // ------------------------------------------------------------------------------------------------------------
        //                                       Public API
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets the <see cref="SimulationArea"/>
        /// </summary>
        public void Reset()
        {
            DesignatePlanets();
        }

        // ------------------------------------------------------------------------------------------------------------
        //                                  Unity Lifecycle Methods
        // ------------------------------------------------------------------------------------------------------------

        private void OnValidate()
        {
            Assert.IsTrue(planetDesignationAreaWidth <= deathZoneWidth);
            Assert.IsTrue(planetDesignationAreaHeight <= deathZoneHeight);
            Assert.IsTrue(minDistanceBetweenPlanets < planetDesignationAreaHeight || 
                          minDistanceBetweenPlanets < planetDesignationAreaWidth);

            UpdateMinDistanceBetweenPlanets();
        }

        private void OnEnable()
        {
            UpdateMinDistanceBetweenPlanets();
        }

        // ------------------------------------------------------------------------------------------------------------

        private void UpdateMinDistanceBetweenPlanets()
        {
            minDistanceBetweenPlanets = planetDesignationAreaWidth / 2;
        }

        /// <summary>
        /// Updates DeathZone GameObject sizes and position so that the rectangular area that is limitted by DeathZones
        /// has width == <see cref="deathZoneWidth"/> and height == <see cref="deathZoneHeight"/>
        /// </summary>
        private void UpdateDeathZoneSizesAndPositions()
        {
            UpdateDeathZoneTransforms(deathZoneTop, deathZoneBottom, deathZoneLeft, deathZoneRight, deathZoneWidth,
                             deathZoneHeight, deathZoneBorderThickness);
        }

        private void UpdateDeathZoneTransforms(GameObject top, GameObject bottom, GameObject left, GameObject right,
                                      float width, float height, float borderThickness)
        {
            top.transform.localScale = new Vector3(width, borderThickness, borderThickness);

            top.transform.localPosition = new Vector3(0, height / 2, 0);

            bottom.transform.localScale = top.transform.localScale;

            bottom.transform.localPosition = new Vector3(0, -height / 2, 0);

            left.transform.localScale = new Vector3(borderThickness, height, borderThickness);

            left.transform.localPosition = new Vector3(-width / 2, 0, 0);

            right.transform.localScale = left.transform.localScale;

            right.transform.localPosition = new Vector3(width / 2, 0, 0);
        }

        private void OnDrawGizmos()
        {
            if (showPlanetDesignationArea)
            {
                Gizmos.color = Color.blue;

                Vector3 planetDesignationAreaCornerTopLeft =
                    new Vector3(transform.localPosition.x - planetDesignationAreaWidth / 2,
                                transform.localPosition.x + planetDesignationAreaHeight / 2,
                                0);

                Vector3 planetDesignationAreaCornerTopRight =
                    new Vector3(transform.localPosition.x + planetDesignationAreaWidth / 2,
                                transform.localPosition.x + planetDesignationAreaHeight / 2,
                                0);

                Vector3 planetDesignationAreaCornerBottomRight =
                    new Vector3(transform.localPosition.x + planetDesignationAreaWidth / 2,
                                transform.localPosition.x - planetDesignationAreaHeight / 2,
                                0);

                Vector3 planetDesignationAreaCornerBottomLeft =
                    new Vector3(transform.localPosition.x - planetDesignationAreaWidth / 2,
                                transform.localPosition.x - planetDesignationAreaHeight / 2,
                                0);

                Gizmos.DrawLine(planetDesignationAreaCornerTopLeft, planetDesignationAreaCornerTopRight);
                Gizmos.DrawLine(planetDesignationAreaCornerTopRight, planetDesignationAreaCornerBottomRight);
                Gizmos.DrawLine(planetDesignationAreaCornerBottomRight, planetDesignationAreaCornerBottomLeft);
                Gizmos.DrawLine(planetDesignationAreaCornerBottomLeft, planetDesignationAreaCornerTopLeft);
            }
        }

        // Position planets not to close to DeathZone and to each other
        private void DesignatePlanet(GameObject planet)
        {
            float planetPosX = Random.Range(-planetDesignationAreaWidth / 2, planetDesignationAreaWidth / 2);
            float planetPosY = Random.Range(-planetDesignationAreaHeight / 2, planetDesignationAreaHeight / 2);
            planet.transform.localPosition = new Vector3(planetPosX, planetPosY, 0);
        }

        private void DesignatePlanets()
        {
            do
            {
                //Debug.LogFormat("{0}: {1}", LOG_TAG, nameof(classHelper.DesignatePlanet));
                DesignatePlanet(basePlanet);
                DesignatePlanet(targetPlanet);
            } 
            while (Vector3.Distance(basePlanet.transform.position, targetPlanet.transform.position) < 
                  minDistanceBetweenPlanets);

            DesignateRocket();
        }


        private void DesignateRocket()
        {
            if (Random.value < rocketNearTargetPlanetProbability / 100)
            {
                DesignateRocketNearTargetPlanet();
            }

            else
            {
                DesignateRocketOnBlasePlanet();
            }
        }

        // Position Rocket on base planet
        private void DesignateRocketOnBlasePlanet()
        {
            rocket.transform.localPosition = basePlanetBody.transform.parent.localPosition;

            // shift the rocket to put it precisely on the planet
            rocket.transform.localPosition = 
                new Vector3(rocket.transform.localPosition.x,
                            rocket.transform.localPosition.y + basePlanetBody.transform.localScale.x - rocketToBasePlanetBodyOffset, 
                            0);

            rocket.transform.localEulerAngles = Vector3.zero;
        }

        // Position Rocket near target planet
        private void DesignateRocketNearTargetPlanet()
        {
            //rocket.transform.localPosition = targetPlanetBody.transform.parent.localPosition;

            // circle radius - offset to target planet
            float r = Random.Range(rocketToTargetPlanetBodyMinOffset, rocketToTargetPlanetBodyMaxOffset);

            // random angle to position rocket around the target planet
            angleDeg = 360 * Random.value;
            float angleRad = Mathf.Deg2Rad * angleDeg;

            float rocketPosX = targetPlanetBody.transform.position.x + (r * Mathf.Cos(angleRad));
            float rocketPosY = targetPlanetBody.transform.position.y + (r * Mathf.Sin(angleRad));

            rocket.transform.position = new Vector3(rocketPosX, rocketPosY, 0);

            rocketPositioningAngleDistortion = 120 * Random.value - 60; // random angle in range [-45, 45]

            // orient rocket so that its legs precisely point at the target planet
            //rocket.transform.localEulerAngles = new Vector3(0, 0, angleRad * Mathf.Rad2Deg - 90);

            // orient rocket so that its legs point at the target planet (not precisely, added distortion angle)
            rocket.transform.localEulerAngles = new Vector3(
                0, 0, angleRad * Mathf.Rad2Deg - 90 + rocketPositioningAngleDistortion);

            // if the rocket is spawned to close to the base planet => redesignate the rocket
            if (Vector3.Distance(rocket.transform.position, basePlanet.transform.position) < 
                rocketToTargetPlanetBodyMinOffset) 
            {
                Debug.LogFormat("{0}: Rocket is to close to base planet: redesignation", LOG_TAG);
                DesignateRocketNearTargetPlanet();
            }
        }
    }
}