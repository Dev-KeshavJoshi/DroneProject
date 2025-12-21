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

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 1.5f;
        rb.angularDrag = 2f;
    }

    void FixedUpdate()
    {
        HandleThrottle();
        HandleMovement();
        HandleYaw();
        StabilizeDrone();
    }

    // ---------------- THROTTLE ----------------
    void HandleThrottle()
    {
        if (Input.GetKey(KeyCode.Space))
            rb.AddForce(Vector3.up * throttleForce, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftControl))
            rb.AddForce(Vector3.down * throttleForce, ForceMode.Force);
    }

    // ---------------- PITCH & ROLL ----------------
    void HandleMovement()
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

        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime)
        );

        Vector3 movementForce =
            transform.forward * pitchInput * moveForce +
            transform.right * rollInput * moveForce;

        rb.AddForce(movementForce, ForceMode.Force);
    }

    // ---------------- YAW ----------------
    void HandleYaw()
    {
        float yawInput = 0f;

        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;

        rb.AddTorque(Vector3.up * yawInput * yawForce, ForceMode.Force);
    }

    // ---------------- AUTO STABILIZATION ----------------
    void StabilizeDrone()
    {
        Vector3 rot = transform.localEulerAngles;
        rot.x = NormalizeAngle(rot.x);
        rot.z = NormalizeAngle(rot.z);

        Vector3 stabilizeTorque = new Vector3(
            -rot.x * stabilizationSpeed,
            0f,
            -rot.z * stabilizationSpeed
        );

        rb.AddRelativeTorque(stabilizeTorque, ForceMode.Force);
    }

    // ---------------- HELPER ----------------
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
