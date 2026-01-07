using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneControllerFull : MonoBehaviour
{
    [Header("Forces")]
    public float throttleForce = 15f;
    public float moveForce = 10f;
    public float yawForce = 5f;

    [Header("Rotation")]
    public float tiltSpeed = 8f;
    public float maxTiltAngle = 30f;
    public float stabilizationSpeed = 2f;

    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.linearDamping = 1.5f;
        rigidBody.angularDamping = 2f;
    }

    private void FixedUpdate()
    {
        HandleThrottle();
        HandleMovement();
        HandleYaw();
        StabilizeDrone();
    }

    // ---------------- THROTTLE ----------------
    private void HandleThrottle()
    {
        if (Input.GetKey(KeyCode.Space))
            rigidBody.AddForce(Vector3.up * throttleForce, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftControl))
            rigidBody.AddForce(Vector3.down * throttleForce, ForceMode.Force);
    }

    // ---------------- PITCH & ROLL ----------------
    private void HandleMovement()
    {
        float pitchInput = 0f;
        float rollInput = 0f;

        if (Input.GetKey(KeyCode.W)) pitchInput = 1f;
        if (Input.GetKey(KeyCode.S)) pitchInput = -1f;

        if (Input.GetKey(KeyCode.A)) rollInput = -1f;
        if (Input.GetKey(KeyCode.D)) rollInput = 1f;

        float targetPitch = pitchInput * maxTiltAngle;
        float targetRoll = rollInput * maxTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(
            targetPitch,
            transform.eulerAngles.y,
            -targetRoll
        );

        rigidBody.MoveRotation(
            Quaternion.Slerp(rigidBody.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime)
        );

        Vector3 movementForce =
            transform.forward * pitchInput * moveForce +
            transform.right * rollInput * moveForce;

        rigidBody.AddForce(movementForce, ForceMode.Force);
    }

    // ---------------- YAW ----------------
    private void HandleYaw()
    {
        float yawInput = 0f;

        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;

        rigidBody.AddTorque(Vector3.up * yawInput * yawForce, ForceMode.Force);
    }

    // ---------------- AUTO STABILIZATION ----------------
    private void StabilizeDrone()
    {
        Vector3 rot = transform.localEulerAngles;
        rot.x = NormalizeAngle(rot.x);
        rot.z = NormalizeAngle(rot.z);

        Vector3 stabilizeTorque = new Vector3(
            -rot.x * stabilizationSpeed,
            0f,
            -rot.z * stabilizationSpeed
        );

        rigidBody.AddRelativeTorque(stabilizeTorque, ForceMode.Force);
    }

    // ---------------- HELPER ----------------
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
