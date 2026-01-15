using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class DroneControllerFull : MonoBehaviour
{
    [Header("Throttle Settings")]
    [SerializeField] private float maxThrottleForce = 20f;
    [SerializeField] private float throttleRampSpeed = 0.5f;
    [SerializeField] private float spoolDownSpeed = 0.5f;
    [SerializeField] private float throttleDelay = 3f;
    [SerializeField] private float hoveringDelay = 1f;

    [Header("Forces")]
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float yawForce = 5f;

    [Header("Rotation")]
    [SerializeField] private float tiltSpeed = 8f;
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float stabilizationSpeed = 2f;
    
    // Runtime
    public float CurrentThrottle { get; private set; }
    public float HoveringThrottle { get; private set; }

    private float hoverForce;
    private bool isHovering = false;
    private bool isThrottleAllowed = false;
    private Coroutine throttleCoroutine;
    private Coroutine hoveringCoroutine;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.linearDamping = 1.5f;
        rigidBody.angularDamping = 2f;
    }

    private void Start()
    {
        hoverForce = rigidBody.mass * Physics.gravity.magnitude;
    }

    private void Update()
    {
        HandleThrottleInput();
        HandleHoveringInput();
    }

    private void FixedUpdate()
    {
        ApplyThrottle();
        ApplyHovering();

        HandleMovement();
        HandleYaw();
        StabilizeDrone();
    }

    // ---------------- THROTTLE ----------------
    private void ApplyThrottle()
    {
        if (!isThrottleAllowed) return;

        rigidBody.AddForce(Vector3.up * (CurrentThrottle * maxThrottleForce), ForceMode.Force);
    }

    private void ApplyHovering()
    {
        if (!isHovering) return;

        rigidBody.AddForce(Vector3.up * hoverForce, ForceMode.Force);
    }

    private void HandleThrottleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (throttleCoroutine == null)
                throttleCoroutine = InvokeAfterDelay(() => isThrottleAllowed = true, throttleDelay);
        }

        if (Input.GetKey(KeyCode.Space) && isThrottleAllowed)
        {
            CurrentThrottle = Mathf.MoveTowards(
                CurrentThrottle,
                1f,
                throttleRampSpeed * Time.deltaTime
            );
        }
        else
        {
            // Throttle decay when released
            CurrentThrottle = Mathf.MoveTowards(
                CurrentThrottle,
                0f,
                spoolDownSpeed * Time.deltaTime
            );
        }
    }

    private void HandleHoveringInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (hoveringCoroutine == null)
                hoveringCoroutine = InvokeAfterDelay(() =>
                {
                    isHovering = true;
                    HoveringThrottle = 0.65f;
                }
                , hoveringDelay);
        }

        if (Input.GetKey(KeyCode.LeftShift) && isHovering)
        {
            isHovering = false;
            hoveringCoroutine = null;
            HoveringThrottle = 0;
        }
    }

    // ---------------- PITCH & ROLL ----------------
    private void HandleMovement()
    {
        float pitchInput = 0f;
        float rollInput = 0f;

        if (Input.GetKey(KeyCode.W)) pitchInput = 1f;


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

    public bool GetIsHovering()
    {
        return isHovering;
    }

    private Coroutine InvokeAfterDelay(Action action, float delay)
    {
        Coroutine coroutine = StartCoroutine(InvokeRoutine(action, delay));
        return coroutine; 
    }

    private IEnumerator InvokeRoutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
