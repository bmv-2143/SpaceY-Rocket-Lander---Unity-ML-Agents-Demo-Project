using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RocketLanderAgent : Agent
{
    /**
     * Note: this is not a production code, it has lots of issues, hacky and incomplete parts.
     * 
     *                              !!! Under Construction !!!
     */

    // ------------------------------------ Inspector Visible -------------------------------------

    [SerializeField]
    private RocketLanderAcademy academy;

    [SerializeField]
    private AudioControl audioControl;

    [SerializeField]
    private Transform planet1;

    [SerializeField]
    private Transform planet2;

    [SerializeField]
    private float rocketBorderSpawnMinDistX = 4f; // planet diameter / 2

    [SerializeField]
    private float rocketBorderSpawnMinDistY = 4f;

    [SerializeField]
    private Transform leftLegLandingProbe;

    [SerializeField]
    private Transform rightLegLandingProbe;

    [SerializeField]
    private GameObject environment;

    [SerializeField]
    private float spawnMinDist = 1f;

    [SerializeField]
    private Rocket rocket;

    [SerializeField]
    private Text paramsText1;

    [SerializeField]
    private Text paramsText2;

    [SerializeField]
    private Text paramsText3;

    [SerializeField]
    private Button switchBrainButton;

    [SerializeField]
    private Button switchScreenToHdButton;

    [SerializeField]
    private Button switchScreenToFullHdButton;

    [SerializeField]
    private Text successText;

    [SerializeField]
    private Text failText;

    [SerializeField]
    private GameObject playerControlsHint;

    [SerializeField]
    private Transform targetMarkerSphere;

    // Death Zones
    [SerializeField]
    private GameObject deathZoneTopGo;

    [SerializeField]
    private GameObject deathZoneBottomGo;

    [SerializeField]
    private GameObject deathZoneLeftGo;

    [SerializeField]
    private GameObject deathZoneRightGo;

    // ------------------------------------------- Private ----------------------------------------

    private float failureBorderBottom; 
    private float failureBorderTop; 
    private float failureBorderLeft;
    private float failureBorderRight;

    private int solved;
    private int failed;

    private float targetRadius = 3f;

    private Vector3 DEATH_ZONE_TOP;
    private Vector3 DEATH_ZONE_BOTTOM;
    private Vector3 DEATH_ZONE_LEFT;
    private Vector3 DEATH_ZONE_RIGHT;   

    private float previousDistanceToTarget = float.PositiveInfinity;

    private float timeStampCurrent;
    private float timeStampPrevious;
    private Vector3 rocketVelocityPrevious;
    private Vector3 rocketVelocityCurrent;

    private Vector3 linearAcceleration = Vector3.zero;

    private long myStepCountInLastSession = 0;
    private float myMeanEndSessionReward = 0;

    private float mySessionReward = 0;

    private int failCounterPosition;
    private int failCounterSpeed;
    private int failCounterRotation;
    private int failCounterAccident;

    private static float valueMinPrevious = float.MaxValue;
    private static float valueMaxPrevious = float.MinValue;

    // --------------------------------------------------------------------------------------------
    //                                      ML-agents API
    // --------------------------------------------------------------------------------------------

    public override void InitializeAgent()
    {
        // configure SwitchBrainButton
        switchBrainButton.onClick.AddListener(SwitchBrainType);
        UpdateSwitchBrainButtonTextAndShowHint();

        switchScreenToHdButton.onClick.AddListener(SwitchToHd);
        switchScreenToFullHdButton.onClick.AddListener(SwitchToFullHd);

        InitializeBorders();
        SetUpDeathZones();

        // TODO: remove target distance tracking
        previousDistanceToTarget = GetDistanceToTarget();
        targetMarkerSphere.transform.localScale = 2 * new Vector3(targetRadius, targetRadius, targetRadius);

        Vector3 rocketStartPos = GenerateRocketPosition();

        rocket.Initialize(rocketStartPos.x, rocketStartPos.y);
        planet1.transform.position = GetStartPlanetPos(rocketStartPos);
        planet2.transform.position = GenerateTargetPlanetPos(rocketStartPos, planet1.transform.position);
    }

    public override List<float> CollectState()
    {
        /**
         * Positional information of relevant GameObjects should be encoded in relative coordinates 
         * wherever possible. This is often relative to the agent position
         */
        List<float> state = new List<float>();

        // add position of the rocket relative to environment
        float relativePosX = rocket.GetLocalPosX() - environment.transform.position.x;
        state.Add(relativePosX); // state 1

        float relativePosY = rocket.GetLocalPosY() - environment.transform.position.y;
        state.Add(relativePosY); // state 2

        // relative position of planet 1
        float relativePosXPlanet1 = planet1.position.x - environment.transform.position.x;
        float relativePosYPlanet1 = planet1.position.y - environment.transform.position.y;
        state.Add(relativePosXPlanet1); // state 3
        state.Add(relativePosYPlanet1); // state 4

        // relative position of planet 2 (target planet)
        float relativePosXPlanet2 = planet2.position.x - environment.transform.position.x;
        float relativePosYPlanet2 = planet2.position.y - environment.transform.position.y;
        state.Add(relativePosXPlanet2); // state 5
        state.Add(relativePosYPlanet2); // state 6

        // relative position of leftLegLandingProbe
        float leftLegLandingProbePosX = leftLegLandingProbe.position.x - environment.transform.position.x;
        float leftLegLandingProbePosY = leftLegLandingProbe.position.y - environment.transform.position.y;
        state.Add(leftLegLandingProbePosX); // state 7
        state.Add(leftLegLandingProbePosY); // state 8

        // relative position of rightLegLandingProbe
        float rightLegLandingProbePosX = rightLegLandingProbe.position.x - environment.transform.position.x;
        float rightLegLandingProbePosY = rightLegLandingProbe.position.y - environment.transform.position.y;
        state.Add(rightLegLandingProbePosX); // state 9
        state.Add(rightLegLandingProbePosY); // state 10

        // x-speed
        float rocketVelocityX = rocket.GetVelocity().x;
        state.Add(rocketVelocityX); // state 11

        // y-speed
        float rocketVelocityY = rocket.GetVelocity().y;
        state.Add(rocketVelocityY); // state 12

        // rocket orientation relative to gravity vector
        float rocketOrientation = rocket.GetOrientation();
        state.Add(rocketOrientation); // state 13

        // angular rotation speed
        float rocketAngularVelocityZ = rocket.GetAngularVelocityZ();
        state.Add(rocketAngularVelocityZ); // state 14

        // distance (Y) from rocket to death_zone_top
        float distanceFromRocketToDeathZoneTop =
            deathZoneTopGo.transform.localPosition.y - rocket.GetLocalPosY();
        state.Add(distanceFromRocketToDeathZoneTop); // state 15

        // distance (Y) from rocket to death_zone_bottom
        float distanceFromRocketToDeathZoneBottom =
            rocket.GetLocalPosY() - deathZoneBottomGo.transform.localPosition.y;
        state.Add(distanceFromRocketToDeathZoneBottom); // state 16

        // distance (X) from rocket to death_zone_left
        float distanceFromRocketToDeathZoneLeft =
            rocket.GetLocalPosX() - deathZoneLeftGo.transform.localPosition.x;
        state.Add(distanceFromRocketToDeathZoneLeft); // state 17

        // distance (X) from rocket to death_zone_right
        float distanceFromRocketToDeathZoneRight =
            deathZoneRightGo.transform.localPosition.x - rocket.GetLocalPosX();
        state.Add(distanceFromRocketToDeathZoneRight); // state 18

        if (paramsText2 != null && Time.frameCount % 5 == 0)
        {
            paramsText2.text =
                string.Format("PosX:{0}, posY:{1}, velX:{2},\nvelY:{3}, orient:{4}, angVelZ:{5}, acclX:{6}, acclY:{7}",
                FormatFloat(relativePosX),
                FormatFloat(relativePosY),
                FormatFloat(rocketVelocityX),
                FormatFloat(rocketVelocityY),
                FormatFloat(rocketOrientation),
                FormatFloat(rocketAngularVelocityZ),
                FormatFloat(linearAcceleration.x),
                FormatFloat(linearAcceleration.y));
        }

        return state;
    }

    public override void AgentStep(float[] action)
    {
        myStepCountInLastSession++;

        reward = -0.005f;

        rocket.UpdateEngineForceDirections();

        if (paramsText1 != null && Time.frameCount % 15 == 0)
        {
            paramsText1.text = string.Format("S:{0},F:{1},stp:{2},",
                solved, failed, myStepCountInLastSession);

            paramsText3.text = string.Format("FP:{0},FA:{1},FS:{2},FR:{3}",
                failCounterPosition, failCounterAccident, failCounterSpeed, failCounterRotation);

        }

        // ----------------------------------------------------------------------------------------
        //                                   Movement
        // ----------------------------------------------------------------------------------------
        switch ((int)action[0])
        {
            case 0:
                // do nothing - engine is off - rocket falls down
                break;

            case 1:
                // power on engine - rocket moves up
                rocket.EngineMainEmitBurst();
                audioControl.PlaySfxMainEngine();
                break;

            case 2:
                // left engine on
                rocket.EngineDownLeftEmitBurst();
                audioControl.PlaySfxSideEngine();
                break;

            case 3:
                // right engine on
                rocket.EngineDownRightEmitBurst();
                audioControl.PlaySfxSideEngine();
                break;

            case 4:
                // right engine on
                rocket.EngineStopEmitBurst();
                audioControl.PlaySfxSideEngine();
                break;

            case 5:
                rocket.EngineUpLeftEmitBurst();
                audioControl.PlaySfxSideEngine();
                break;

            case 6:
                rocket.EngineUpRightEmitBurst();
                audioControl.PlaySfxSideEngine();
                break;

            default:
                break;
        }

        // ----------------------------------------------------------------------------------------
        //                             Reward \ Punish
        // ----------------------------------------------------------------------------------------

        //// Small punishments\rewards, disabled in this version

        //float targetingAngle = rocket.GetTargetingAngle(planet2.gameObject);
        ////Debug.Log("Targeting angle: " + targetingAngle);

        //const float ALLOWED_ANGLE_DEVIATION = 20; // degrees

        //const float GRAVITY_RANGE_PLANET_1 = 6;

        //// 1) Add small punishment if rocket is in gravity range of planet 1
        //if (Vector3.Distance(rocket.transform.position, planet1.transform.position) < GRAVITY_RANGE_PLANET_1)
        //{
        //    reward += -0.005f;
        //}

        //// 2) Add small punishment if rocket's landing angle (rocket's orientation) for target 
        //// planet is more then DELTA
        //// we need angle close to 180 degrees
        //else if (Math.Abs(targetingAngle - 180) > ALLOWED_ANGLE_DEVIATION)
        //{
        //    // punish if angle deviation is to big
        //    reward += -0.005f;
        //}

        //// 3) punish if speed is to high
        ////if (!IsRocketSpeedOk())
        ////{
        ////    reward += -0.05f;
        ////    //failed++;
        ////    //reward = -1f;
        ////    //done = true;

        ////    //ResetObjectsPosInScene();
        ////    //UpdateSessionReward();
        ////    //PrintEndSessionReward(false);
        ////    //return;
        ////}

        // ---------------------------------- FAILURE ---------------------------------------------
        if (rocket.GetLocalPosY() < failureBorderBottom
            || rocket.GetLocalPosY() > failureBorderTop
            || rocket.GetLocalPosX() > failureBorderRight
            || rocket.GetLocalPosX() < failureBorderLeft)
        {
            failed++;
            reward = -1f;
            done = true;

            failCounterPosition++;

            Debug.Log("Fail: position: " + failCounterPosition);
            ShowFailText("Fail: position");

            previousDistanceToTarget = GetDistanceToTarget();

            /**
             * This is required to reset rocket rotation when switching to a new session - when
             * rocket moves into left or right vertical wall (presumably at a high speed) and 
             * forward button (main engine) is pressed - the session is over with a failure (that
             * is expected), but the new session is started with rocket rotating around z-axis 
             * rapidly - it looks like in this case Agent.Reset() logic is not enough... Why?
             * 
             * Workaround: Adding rocket.Reset() here fixes this issue.
             */
            ResetObjectsPosInScene();
            UpdateSessionReward();
            PrintEndSessionReward(false);
            return;
        }

        if (rocket.AccidentOccured())
        {
            failed++;
            reward = -1f;
            done = true;

            failCounterAccident++;

            audioControl.PlaySfxAccident();

            Debug.Log("Fail: accident: " + failCounterAccident);
            ShowFailText("Fail: accident");

            ResetObjectsPosInScene();
            UpdateSessionReward();
            PrintEndSessionReward(false);
            return;
        }


        if (!IsPlayerBrain() && !IsRocketSpeedOk())
        {
            failed++;
            reward = -1f;
            done = true;

            failCounterSpeed++;

            Debug.Log("Fail: speed: " + failCounterSpeed);
            ShowFailText("Fail: speed");

            ResetObjectsPosInScene();
            UpdateSessionReward();
            PrintEndSessionReward(false);
            return;
        }

        if (!IsPlayerBrain() && Math.Abs(rocket.GetAngularVelocityZ()) > 0.8)
        {
            failed++;
            reward = -1f;
            done = true;

            failCounterRotation++;

            Debug.Log("Fail: angular velocity: " + failCounterRotation);
            ShowFailText("Fail: rotation speed");

            ResetObjectsPosInScene();
            UpdateSessionReward();
            PrintEndSessionReward(false);
            return;
        }

        // ------------------------------------- SUCCESS ------------------------------------------

        if (rocket.IsOnPlanet() && IsRocketSpeedOk())
        {
            reward = 1;
            solved++;
            done = true;

            ShowSuccessText();

            previousDistanceToTarget = GetDistanceToTarget();
            ResetObjectsPosInScene();
            UpdateSessionReward();
            PrintEndSessionReward(true);
            return;
        }

        previousDistanceToTarget = GetDistanceToTarget();
        UpdateSessionReward();
    }

    public override void AgentReset()
    {
        myStepCountInLastSession = 0;
        mySessionReward = 0;
        ResetObjectsPosInScene();

        InitializeBorders();
        SetUpDeathZones();

        // TODO: remove target sphere tracking (obsolete)
        // spawn target sphere at random position
        targetMarkerSphere.transform.localPosition = GenerateTargetPosition();
    }

    // --------------------------------------------------------------------------------------------
    //                                 Unity Lifecycle Methods
    // --------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        linearAcceleration = (rocket.GetVelocity() - rocketVelocityPrevious) / Time.fixedDeltaTime;
        rocketVelocityPrevious = rocket.GetVelocity();
    }

    // --------------------------------------------------------------------------------------------
    //                                      Public API
    // --------------------------------------------------------------------------------------------

    public void SwitchToHd()
    {
        academy.SetScreenToHd();
        ResetSession();
    }

    public void SwitchToFullHd()
    {
        academy.SetScreenToFullHd();
        ResetSession();
    }

    public void SwitchBrainType()
    {
        Brain brainComponent = academy.GetComponentInChildren<Brain>();

        if (brainComponent.brainType == BrainType.Player)
        {
            brainComponent.brainType = BrainType.Internal;
            Debug.Log("Switched to Internal brain");
        } 

        else if (brainComponent.brainType == BrainType.Internal)
        {
            brainComponent.brainType = BrainType.Player;
            Debug.Log("Switched to Player brain");
        }

        done = true;
        UpdateSwitchBrainButtonTextAndShowHint();

        ResetSession();
    }

    // --------------------------------------------------------------------------------------------
    //                                     Private Methods
    // --------------------------------------------------------------------------------------------

    private void ResetSession()
    {
        ResetCounters();

        academy.InitEnvironment();
        ResetObjectsPosInScene();
    }

    private void ResetCounters()
    {
        solved = 0;
        failed = 0;
        failCounterPosition = 0;
        failCounterAccident = 0;
        failCounterSpeed = 0;
        failCounterRotation = 0;
    }

    private void UpdateSwitchBrainButtonTextAndShowHint()
    {
        Text buttonText = switchBrainButton.GetComponentInChildren<Text>();

        if (brain.brainType == BrainType.Player)
        {
            playerControlsHint.SetActive(true);
            StartCoroutine(DisableObjectAfterTimeoutCoroutine(playerControlsHint, 4f));
            buttonText.text = "Brain: Player";
            Debug.Log("Switched to Internal brain");
        }

        else if (brain.brainType == BrainType.Internal)
        {
            playerControlsHint.SetActive(false);
            buttonText.text = "Brain: ML-Agent";
            Debug.Log("Switched to Player brain");
        }
    }

    private Vector3 GenerateTargetPosition()
    {
        float targetXPos = 0;
        float targetYPos = 0;

        float targetXPos1 = UnityEngine.Random.Range(failureBorderLeft + spawnMinDist, rocket.GetLocalPosX() - spawnMinDist);
        float targetXPos2 = UnityEngine.Random.Range(rocket.GetLocalPosX() + spawnMinDist, failureBorderRight - spawnMinDist);

        float[] choisesX = new float[] { targetXPos1, targetXPos2 };

        targetXPos = choisesX[UnityEngine.Random.Range(0, choisesX.Length)];

        float targetYPos1 = UnityEngine.Random.Range(failureBorderBottom + spawnMinDist, rocket.GetLocalPosY() - spawnMinDist);
        float targetYPos2 = UnityEngine.Random.Range(rocket.GetLocalPosY() + spawnMinDist, failureBorderTop - spawnMinDist);

        float[] choisesY = new float[] { targetYPos1, targetYPos2 };

        targetYPos = choisesY[UnityEngine.Random.Range(0, choisesY.Length)];

        return new Vector3(targetXPos, targetYPos, 0);
    }

    private Vector3 GenerateRocketPosition()
    {
        float rocketXPos = 0;
        float rocketYPos = 0;

        float rocketStartBorderLeft = -academy.rocketStartPosBorderX;
        float rocketStartBorderRight = academy.rocketStartPosBorderX;
        float rocketStartBorderBottom = -academy.rocketStartPosBorderY;
        float rocketStartBorderTop = academy.rocketStartPosBorderY;

        rocketXPos = UnityEngine.Random.Range(
            rocketStartBorderLeft, rocketStartBorderRight);

        rocketYPos = UnityEngine.Random.Range(
            rocketStartBorderBottom, rocketStartBorderTop);

        // ------------------------------------------------------------------------------------
        // avoid rocket spawning near death zone
        float rocketStartBorderLeftMin = failureBorderLeft + rocketBorderSpawnMinDistX;
        float rocketStartBorderRightMax = failureBorderRight - rocketBorderSpawnMinDistX;
        float rocketStartBorderBottomMin = failureBorderBottom + rocketBorderSpawnMinDistY;
        float rocketStartBorderTopMax = failureBorderTop - rocketBorderSpawnMinDistY;

        if (rocketXPos < rocketStartBorderLeftMin)
        {
            rocketXPos = rocketStartBorderLeftMin;
        }

        if (rocketXPos > rocketStartBorderRightMax)
        {
            rocketXPos = rocketStartBorderRightMax;
        }

        if (rocketYPos < rocketStartBorderBottomMin)
        {
            rocketYPos = rocketStartBorderBottomMin;
        }

        if (rocketYPos > rocketStartBorderTopMax)
        {
            rocketYPos = rocketStartBorderTopMax;
        }

        Vector3 rocketPos = new Vector3(rocketXPos, rocketYPos, 0);

        return new Vector3(rocketXPos, rocketYPos, 0);
    }

    private Vector3 GenerateTargetPlanetPos(Vector3 rocketPos, Vector3 startPlanetPos)
    {
        float targetPlanetPosX = 0;
        float targetPlanetPosY = 0;

        const float MIN_DIST_TO_ROCKET = 3f;

        // planet radius + planet1 grav range + planet2 grav range + some space
        const float MIN_DIST_TO_START_PLANET = 13f; 

        Vector3 targetPlanetPos = Vector3.zero;

        while (true)
        {
            targetPlanetPos.x = UnityEngine.Random.Range(
                    failureBorderLeft + rocketBorderSpawnMinDistX, 
                    failureBorderRight - rocketBorderSpawnMinDistX);

            targetPlanetPos.y = UnityEngine.Random.Range(
                 failureBorderBottom + rocketBorderSpawnMinDistY,
                 failureBorderTop - rocketBorderSpawnMinDistY);

            if (Vector3.Distance(targetPlanetPos, rocketPos) >= MIN_DIST_TO_ROCKET &&
                Vector3.Distance(targetPlanetPos, startPlanetPos) >= MIN_DIST_TO_START_PLANET)
            {
                break;
            }

        }

        return targetPlanetPos;
    }

    private Vector3 GetStartPlanetPos(Vector3 rocketStartPos)
    {
        float planetCenterBelowRocketCenter = 5.2f;
        return new Vector3(rocketStartPos.x, rocketStartPos.y - planetCenterBelowRocketCenter); 
    }

    private Vector3 GetLocalTargetPos()
    {
        return targetMarkerSphere.transform.localPosition;
    }

    private float GetDistanceToTarget()
    {
        return (GetLocalTargetPos() - rocket.GetLocalPosition()).magnitude;
    }

    private float GetTargetPosX()
    {
        return targetMarkerSphere.position.x;
    }

    private float GetTargetPosY()
    {
        return targetMarkerSphere.position.y;
    }

    private void SetUpDeathZones()
    {
        deathZoneTopGo.transform.localPosition = DEATH_ZONE_TOP;
        deathZoneBottomGo.transform.localPosition = DEATH_ZONE_BOTTOM;
        deathZoneLeftGo.transform.localPosition = DEATH_ZONE_LEFT;
        deathZoneRightGo.transform.localPosition = DEATH_ZONE_RIGHT;

        deathZoneTopGo.transform.localScale =
            new Vector3(
                Math.Abs(DEATH_ZONE_LEFT.x - DEATH_ZONE_RIGHT.x),
                deathZoneTopGo.transform.localScale.y, 
                deathZoneTopGo.transform.localScale.z);

        deathZoneBottomGo.transform.localScale = deathZoneTopGo.transform.localScale;

        deathZoneLeftGo.transform.localScale =
            new Vector3(
                deathZoneLeftGo.transform.localScale.x,
                Math.Abs(DEATH_ZONE_BOTTOM.y - DEATH_ZONE_TOP.y),
                deathZoneLeftGo.transform.localScale.z);

        deathZoneRightGo.transform.localScale = deathZoneLeftGo.transform.localScale;
    }

    private void UpdateAndCheckMinMaxCurrentValue(float val)
    {
        if (val > valueMaxPrevious)
        {
            valueMaxPrevious = val;
        }

        if (val < valueMinPrevious)
        {
            valueMinPrevious = val;
        }
    }

    /**
     * normalizedValue = (currentValue - minValue) / (maxValue - minValue) 
     */
    private float Normalize(float currentValue, float minValue, float maxValue)
    {
        // --------------- should I clamp borders? ---------------
        if (currentValue > maxValue)
        {
            currentValue = maxValue;
        }

        else if (currentValue < minValue)
        {
            currentValue = minValue;
        }

        return (currentValue - minValue) / (maxValue - minValue);
    }

    private float NormalizePosX(float posX)
    {
        return Normalize(posX, failureBorderLeft, failureBorderRight);
    }

    private float NormalizePosY(float posY)
    {
        return Normalize(posY, failureBorderBottom, failureBorderTop);
    }

    private float NormalizeSpeedX(float currentSpeedX)
    {
        return Normalize(currentSpeedX, -3.9f, 3.9f); // min max are experimental for current gravity
    }

    private float NormalizeSpeedY(float currentSpeedY)
    {
        return Normalize(currentSpeedY, -32f, 20f);
    }

    // ------------------------------------- velocity ---------------------------------------------
    // source: https://answers.unity.com/questions/48179/rigidbody-acceleration.html
    //acceleration = (rigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
    //lastVelocity = rigidbody.velocity;
    private Vector3 GetRocketAcceleration(Vector3 currentVelocity, Vector3 lastVelocity, float deltaTime)
    {
        Vector3 acceleration = (currentVelocity - lastVelocity) / deltaTime;
        return acceleration;
    }

    private void ResetObjectsPosInScene()
    {
        Vector3 rocketStartPos = GenerateRocketPosition();
        rocket.Reset(rocketStartPos);
        planet1.transform.position = GetStartPlanetPos(rocketStartPos);
        planet2.transform.position = GenerateTargetPlanetPos(rocketStartPos, planet1.transform.position);
    }

    private void UpdateSessionReward()
    {
        mySessionReward += reward;
    }

    private void PrintEndSessionReward(bool isSuccess)
    {
        //Debug.Log((isSuccess ? "Success: " : "Failure: ")
        //    + " sessionReward: " + mySessionReward);
    }

     private bool IsRocketSpeedOk()
    {
        return rocket.GetVelocity().x < 2.5 && rocket.GetVelocity().y < 2.5;
    }

    private void ShowSuccessText()
    {
        successText.text = "Success!";
        successText.gameObject.SetActive(true);
        StartCoroutine(DisableObjectAfterTimeoutCoroutine(successText.gameObject, 1f));
    }

    private void ShowFailText(string msg)
    {
        failText.text = msg;
        failText.gameObject.SetActive(true);
        StartCoroutine(DisableObjectAfterTimeoutCoroutine(failText.gameObject, 1f));
    }

    private IEnumerator DisableObjectAfterTimeoutCoroutine(GameObject goToDisable, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        goToDisable.SetActive(false);
    }

    private bool IsPlayerBrain()
    {
        return brain.brainType == BrainType.Player;
    }

    private void InitializeBorders()
    {
        failureBorderTop = academy.boardScaleY;
        failureBorderBottom = -academy.boardScaleY;
        failureBorderRight = academy.boardScaleX;
        failureBorderLeft = -academy.boardScaleX;

        DEATH_ZONE_TOP = new Vector3(0f, failureBorderTop, 0f);
        DEATH_ZONE_BOTTOM = new Vector3(0f, failureBorderBottom, 0f);
        DEATH_ZONE_LEFT = new Vector3(failureBorderLeft, 0f, 0f);
        DEATH_ZONE_RIGHT = new Vector3(failureBorderRight, 0f, 0f);
    }

    private string FormatFloat(float value)
    {
        const int PADDING = 6;
        return Math.Round(value, 2).ToString().PadLeft(PADDING);
    }
}