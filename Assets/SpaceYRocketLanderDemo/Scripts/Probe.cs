using UnityEngine;

public class Probe : MonoBehaviour {

    public enum ProbeType
    {
        NONE,
        LEFT_LEG,
        RIGHT_LEG,
        HULL
    }

    public enum TriggerType
    {
        NONE,
        TRIGGER_ENTER,
        COLLISION_EXIT,
        TRIGGER_STAY
    }

    [SerializeField]
    private ProbeType probeType;

    private ProbeMonitor probeMonitor;

    // from docs: https://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    // Collision events are only sent if one of the colliders also has a non-kinematic rigidbody attached.
    void OnTriggerEnter(Collider other)
    {
        ProbeMonitor.SendProbeTriggerEvent(probeType, TriggerType.TRIGGER_ENTER);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("TARGET_PLANET"))
        {
            ProbeMonitor.SendProbeTriggerEvent(probeType, TriggerType.TRIGGER_STAY);
        }
    }

    //void OnTriggerExit(Collider other)
    //{
    //    //Debug.Log("Probe: OnTriggerExit: " + probeType);
    //}
}
