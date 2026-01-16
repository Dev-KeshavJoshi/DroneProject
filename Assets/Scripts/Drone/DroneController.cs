using UnityEngine;

public class DroneController : MonoBehaviour
{
    public Motor[] motors;

    [Header("PID Controllers")]
    public PID rollPID;
    public PID pitchPID;
    public PID yawPID;

    [Header("Throttle")]
    [Range(0f, 1f)]
    public float baseThrottle = 0.5f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float rollInput = Input.GetAxis("Horizontal");
        float pitchInput = Input.GetAxis("Vertical");
        float yawInput = Input.GetAxis("Yaw");
        float throttleInput = Input.GetAxis("Throttle");

        baseThrottle = Mathf.Clamp01(baseThrottle + throttleInput * 0.01f);

        Vector3 angVel = rb.angularVelocity * Mathf.Rad2Deg;

        float rollCorrection = rollPID.Update(rollInput - angVel.z, Time.fixedDeltaTime);
        float pitchCorrection = pitchPID.Update(pitchInput - angVel.x, Time.fixedDeltaTime);
        float yawCorrection = yawPID.Update(yawInput - angVel.y, Time.fixedDeltaTime);

        // X-quad mixing
        motors[0].throttle = baseThrottle + pitchCorrection + rollCorrection - yawCorrection;
        motors[1].throttle = baseThrottle + pitchCorrection - rollCorrection + yawCorrection;
        motors[2].throttle = baseThrottle - pitchCorrection - rollCorrection - yawCorrection;
        motors[3].throttle = baseThrottle - pitchCorrection + rollCorrection + yawCorrection;
    }
}
