using TMPro;
using UnityEngine;

public class DroneStatsForUI : MonoBehaviour
{
    // ================== INSPECTOR ==================
    [Header("Terrain")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Drone")]
    [SerializeField] private Rigidbody droneRigidBody;

    [Header("DroneStats UI")]
    [SerializeField] private TextMeshProUGUI speedValue;
    [SerializeField] private TextMeshProUGUI altitudeText;

    [Header("Subtitle UI")]
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Next Checkpoint")]
    [SerializeField] private GameObject checkpoint1_2Mesh;

    // ================== CONSTANTS ==================
    private const float KMPH_CONVERSION = 3.6f;
    private const float MAX_RAY_DISTANCE = 1000f;
    private const float ALTITUDE_OFFSET = 4f;

    private const float NEWTON_TRIGGER_ALTITUDE = 5f;
    private const float NEWTON_TOLERANCE = 0.6f;

    private const float TARGET_ALTITUDE = 15f;
    private const float ALTITUDE_TOLERANCE = 1.2f;
    private const float HOLD_DURATION = 5f;

    // ================== STATE ==================
    private enum CheckpointState
    {
        Inactive,
        Holding,
        Completed
    }

    private CheckpointState checkpointState = CheckpointState.Inactive;

    // ================== RUNTIME ==================
    private float droneSpeed;
    private float droneAltitude;
    private float holdTimer;
    private float instabilityTimer;

    private bool newtonLawShown = false;

    // ================== UNITY ==================
    private void Start()
    {
        subtitleText.gameObject.SetActive(false);

        if (checkpoint1_2Mesh != null)
            checkpoint1_2Mesh.SetActive(false);
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateAltitude();
        UpdateUI();
        HandleNewtonThirdLaw();
        HandleCheckpoint();
    }

    // ================== SPEED ==================
    private void UpdateSpeed()
    {
        droneSpeed = droneRigidBody.linearVelocity.magnitude * KMPH_CONVERSION;
    }

    // ================== ALTITUDE ==================
    private void UpdateAltitude()
    {
        Vector3 rayOrigin = droneRigidBody.transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down,
            out RaycastHit hit, MAX_RAY_DISTANCE, groundLayer))
        {
            droneAltitude = Mathf.Max(0f, hit.distance - ALTITUDE_OFFSET);
        }

        Debug.DrawRay(rayOrigin, Vector3.down * MAX_RAY_DISTANCE, Color.red);
    }

    // ================== UI ==================
    private void UpdateUI()
    {
        speedValue.text = Mathf.RoundToInt(droneSpeed) + " Km/h";
        altitudeText.text = Mathf.RoundToInt(droneAltitude) + " m";
    }

    // ================== NEWTON'S 3RD LAW ==================
    private void HandleNewtonThirdLaw()
    {
        if (newtonLawShown)
            return;

        if (Mathf.Abs(droneAltitude - NEWTON_TRIGGER_ALTITUDE) <= NEWTON_TOLERANCE)
        {
            newtonLawShown = true;

            subtitleText.gameObject.SetActive(true);
            subtitleText.text =
                "⬇ Thrust Force\n\n" +
                "Newton's 3rd Law:\n" +
                "Every action has an equal\n" +
                "and opposite reaction";

            Invoke(nameof(HideNewtonText), 4f);
        }
    }

    private void HideNewtonText()
    {
        if (checkpointState == CheckpointState.Inactive)
            subtitleText.gameObject.SetActive(false);
    }

    // ================== CHECKPOINT 1.1 ==================
    private void HandleCheckpoint()
    {
        if (checkpointState == CheckpointState.Completed)
            return;

        bool altitudeStable =
            Mathf.Abs(droneAltitude - TARGET_ALTITUDE) <= ALTITUDE_TOLERANCE;

        Vector3 vel = droneRigidBody.linearVelocity;

        bool velocityStable =
            Mathf.Abs(vel.y) < 0.8f &&
            new Vector2(vel.x, vel.z).magnitude < 1.2f;

        // ---------- ENTER HOLD ----------
        if (checkpointState == CheckpointState.Inactive && altitudeStable)
        {
            checkpointState = CheckpointState.Holding;
            holdTimer = 0f;
            instabilityTimer = 0f;

            subtitleText.gameObject.SetActive(true);
            subtitleText.text = "Hold position";
        }

        if (checkpointState != CheckpointState.Holding)
            return;

        // ---------- UNSTABLE ----------
        if (!altitudeStable || !velocityStable)
        {
            instabilityTimer += Time.deltaTime;

            if (instabilityTimer > 1.2f)
            {
                holdTimer = Mathf.Max(0f, holdTimer - Time.deltaTime * 2f);
                subtitleText.text = "Stabilize...";
            }
            return;
        }

        // ---------- STABLE ----------
        instabilityTimer = 0f;
        holdTimer += Time.deltaTime;

        subtitleText.text =
            $"Hold for {Mathf.CeilToInt(HOLD_DURATION - holdTimer)} seconds";

        // ---------- COMPLETE ----------
        if (holdTimer >= HOLD_DURATION)
        {
            checkpointState = CheckpointState.Completed;
            subtitleText.text = "Checkpoint 1.1 Completed";

            if (checkpoint1_2Mesh != null)
                checkpoint1_2Mesh.SetActive(true);
        }
    }
}