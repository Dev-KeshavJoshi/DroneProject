using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AltitudeHoldThrottlePID : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Throttle Settings")]
    [SerializeField, Range(0f, 1f)] private float hoverThrottle = 0.5f;
    [SerializeField] private float maxLiftForce = 30f;
    [SerializeField] private float minThrottle = 0f;
    [SerializeField] private float maxThrottle = 1f;

    [Header("PID Gains")]
    [SerializeField] private float Kp = 0.08f;
    [SerializeField] private float Ki = 0.02f;
    [SerializeField] private float Kd = 0.05f;

    //Limits
    [Range(0f, 1f)] private float integralLimit = 1f;
    private float integral;

    public bool isThrottleAllowed { get; set; } = true;
    public float currentAltitude { get; private set; } = 0f;
    private float targetAltitude = 0f;
    private float previousError;
    
    private Rigidbody rigidBody;

    private const float RAY_DISTANCE = 1000f;
    private const float ALTITUDE_OFFSET = 0.2f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateAltitude();
        ApplyPID();
    }

    public void SetTargetAltitude()
    {
        targetAltitude = currentAltitude;
    }

    public void ManualThrottleInput(float input)
    {
        targetAltitude += input * 5f * Time.deltaTime;
    }

    private void UpdateAltitude()
    {
        Vector3 origin = rigidBody.transform.position - (Vector3.up * ALTITUDE_OFFSET);

        if (Physics.Raycast(origin, Vector3.down,
            out RaycastHit hit, RAY_DISTANCE, groundLayer))
        {
            currentAltitude = Mathf.Max(0f, hit.distance);
            Debug.DrawRay(origin, Vector3.down, Color.red); // Ray for visualization.
        }
    }

    private void ApplyPID()
    {
        float error = targetAltitude - currentAltitude;

        // Integral
        integral += error * Time.fixedDeltaTime;
        integral = Mathf.Clamp(integral, -integralLimit, integralLimit);

        // Derivative
        float derivative = (error - previousError) / Time.fixedDeltaTime;
        previousError = error;

        // PID correction (throttle space)
        float pidCorrection =
            (error * Kp) +
            (integral * Ki) +
            (derivative * Kd);

        float throttle = hoverThrottle + pidCorrection;
        throttle = Mathf.Clamp(throttle, minThrottle, maxThrottle);

        Debug.Log("Throttle: " + throttle);

        rigidBody.AddForce(
            Vector3.up * (throttle * maxLiftForce),
            ForceMode.Force
        );
    }
}

