using UnityEngine;

public class PropellerRotate : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 rotationAxis = Vector3.forward;
    public bool invert;

    [Header("Speed Settings")]
    public float idleSpeed = 300f;     // drone idle spin
    public float maxSpeed = 2500f;     // full throttle spin

    [Header("Throttle (0 to 1)")]
    [Range(0f, 1f)]
    public float throttle;             // external control (force)

    void Update()
    {
        float dir = invert ? -1f : 1f;

        // throttle based speed
        float currentSpeed = Mathf.Lerp(idleSpeed, maxSpeed, throttle);

        transform.Rotate(rotationAxis * currentSpeed * dir * Time.deltaTime, Space.Self);
    }
}
