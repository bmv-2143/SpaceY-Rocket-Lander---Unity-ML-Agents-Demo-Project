using HumbleGames.Commons.Attributes;
using RoboRyanTron.QuickButtons;
using UnityEngine;
using UnityEngine.Assertions;

namespace HumbleGames.MLAgents
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
        private GameObject planet1;
        
        [SerializeField]
        private GameObject planet2;

        public QuickButton UpdatePlanetsPos = new QuickButton(nameof(classHelper.DesignatePlanets));

        [Header("Rocket")]

        [SerializeField]
        private GameObject rocket;

        [SerializeField]
        private GameObject planetBodyToDesignateOn;

        /// <summary>
        /// This offset depends on the size of a specific rocket and a specific planet body. It is used to
        /// precisely position rocket on the planet.
        /// </summary>
        [SerializeField]
        private float rocketToPlanetBodyOffset = 1;

        public QuickButton UpdateRocketPos = new QuickButton(nameof(classHelper.DesignateRocket));

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
            minDistanceBetweenPlanets = planetDesignationAreaWidth / 3;
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
                Debug.LogFormat("{0}: {1}", LOG_TAG, nameof(classHelper.DesignatePlanet));
                DesignatePlanet(planet1);
                DesignatePlanet(planet2);
            } 
            while (Vector3.Distance(planet1.transform.position, planet2.transform.position) < minDistanceBetweenPlanets);

            DesignateRocket();
        }

        // Position Rocket on Planet 1
        private void DesignateRocket()
        {
            rocket.transform.localPosition = planetBodyToDesignateOn.transform.parent.localPosition;

            // shift the rocket to put it precisely on the planet
            rocket.transform.localPosition = 
                new Vector3(rocket.transform.localPosition.x,
                            rocket.transform.localPosition.y + planetBodyToDesignateOn.transform.localScale.x - rocketToPlanetBodyOffset, 
                            0);
        }
    }
}