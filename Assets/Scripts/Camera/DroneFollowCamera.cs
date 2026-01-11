using UnityEngine;

public class DroneFollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform drone;

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0f, 2.5f, -6f);
    public float followSmoothTime = 0.3f;
    public float lookAheadMultiplier = 0.1f;

    [Header("Rotation Settings")]
    public float rotationSmoothSpeed = 7.5f;

    private Vector3 _currentVelocity;
    private Rigidbody rb;
    private Vector3 lookAheadTarget;
    
    private void Start()
    {
        rb = drone.GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (drone == null) return;

        lookAheadTarget = drone.position + rb.linearVelocity * lookAheadMultiplier;

        // --- Drone position ---
        Vector3 desiredPosition = drone.TransformPoint(offset);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _currentVelocity,
            followSmoothTime
        );

        // --- Drone rotation ---
        Quaternion lookRotation = Quaternion.LookRotation(
            lookAheadTarget - transform.position,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSmoothSpeed
        );
    }
}