using UnityEngine;

public class ProbeMonitor : MonoBehaviour {

    [SerializeField]
    private GameObject leftLegLandingProbe;

    [SerializeField]
    private GameObject rightLegLandingProbe;

    private bool leftLegLanded = false;
    private bool rightLegLanded = false;
    private bool accidentOccured = false;

    public delegate void ProbeCollidedAction(Probe.ProbeType probeType, Probe.TriggerType collisionType);
    public static event ProbeCollidedAction OnProbeCollided;

    // ---------------------------------- Public API ----------------------------------------------

    public static void SendProbeTriggerEvent(Probe.ProbeType probeType, Probe.TriggerType collisionType)
    {
        OnProbeCollided?.Invoke(probeType, collisionType);
    }

    public bool IsOnPlanet()
    {
        return leftLegLanded && rightLegLanded;
    }

    public bool AccidentOccured()
    {
        return accidentOccured;
    }

    public void Reset()
    {
        leftLegLanded = false;
        rightLegLanded = false;
        accidentOccured = false;
    }

    // ---------------------------------- Unity hook methods --------------------------------------

    private void OnEnable()
    {
        OnProbeCollided += ProbeTriggerHandler;
    }

    private void OnDisable()
    {
        OnProbeCollided -= ProbeTriggerHandler;
    }

    // ----------------------------------- Private helpers ----------------------------------------

    private void ProbeTriggerHandler(Probe.ProbeType probeType, Probe.TriggerType triggerType)
    {
        if (probeType == Probe.ProbeType.LEFT_LEG && triggerType == Probe.TriggerType.TRIGGER_STAY)
        {
            leftLegLanded = true;
        }

        if (probeType == Probe.ProbeType.RIGHT_LEG && triggerType == Probe.TriggerType.TRIGGER_STAY)
        {
            rightLegLanded = true;
        }

        if (probeType == Probe.ProbeType.HULL && triggerType == Probe.TriggerType.TRIGGER_ENTER)
        {
            accidentOccured = true;
        }
    }
}
