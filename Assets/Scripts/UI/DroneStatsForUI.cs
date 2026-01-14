using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    [Header("Helper UI")]
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Checkpoint 1.2 Visuals")]
    [SerializeField] private GameObject rollArrow;
    [SerializeField] private GameObject pitchArrow;
    [SerializeField] private GameObject yawArrow;
    [SerializeField] private GameObject forceVectorDiagram;
    [SerializeField] private TextMeshProUGUI angleText; // Assign a UI Text to show angles

    // ================== CONSTANTS ==================
    private const float KMPH_CONVERSION = 3.6f;
    private const float MAX_RAY_DISTANCE = 1000f;
    private const float ALTITUDE_OFFSET = 0.2f;

    // ================== RUNTIME ==================
    private float droneSpeed;
    private float droneAltitude;

    // Checkpoint System
    private List<ICheckpoint> _checkpoints;
    private int _currentCheckpointIndex = -1;
    private ICheckpoint _currentCheckpoint;

    // ================== UNITY ==================
    private void Start()
    {
        InitializeCheckpoints();

        // Start first checkpoint
        if (_checkpoints.Count > 0)
        {
            SetCheckpoint(0);
        }
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateAltitude();
        UpdateUI();

        if (_currentCheckpoint != null)
        {
            _currentCheckpoint.UpdateCheckpoint();

            if (_currentCheckpoint.IsCompleted)
            {
                AdvanceCheckpoint();
            }
        }
    }

    private void InitializeCheckpoints()
    {
        _checkpoints = new List<ICheckpoint>();

        // Checkpoint 1.1
        _checkpoints.Add(new Checkpoint1_1(
            this,
            droneRigidBody,
            subtitleText,
            tutorialText,
            groundLayer
        ));

        // Checkpoint 1.2
        _checkpoints.Add(new Checkpoint1_2(
            this,
            droneRigidBody,
            subtitleText,
            tutorialText,
            rollArrow,
            pitchArrow,
            yawArrow,
            forceVectorDiagram,
            angleText
        ));

        // Checkpoint 1.3
        _checkpoints.Add(new Checkpoint1_3(
            this,
            droneRigidBody,
            subtitleText,
            tutorialText,
            speedValue
        ));
    }

    private void SetCheckpoint(int index)
    {
        if (index < 0 || index >= _checkpoints.Count) return;

        if (_currentCheckpoint != null)
        {
            _currentCheckpoint.Exit();
        }

        _currentCheckpointIndex = index;
        _currentCheckpoint = _checkpoints[index];
        _currentCheckpoint.Enter();
        
        Debug.Log($"Started Checkpoint {_currentCheckpointIndex + 1}");
    }

    private void AdvanceCheckpoint()
    {
        int nextIndex = _currentCheckpointIndex + 1;
        if (nextIndex < _checkpoints.Count)
        {
            SetCheckpoint(nextIndex);
        }
        else
        {
            // All completed
            _currentCheckpoint.Exit();
            _currentCheckpoint = null;
            tutorialText.text = "All Checkpoints Completed!";
            Debug.Log("All Checkpoints Completed");
        }
    }


    // ================== SHARED LOGIC ==================
    private void UpdateSpeed()
    {
        if (droneRigidBody != null)
            droneSpeed = droneRigidBody.linearVelocity.magnitude * KMPH_CONVERSION;
    }

    private void UpdateAltitude()
    {
        // Simple visual update for main UI, checkpoints do their own precise logic if needed,
        // or we could expose this. For now keeping it essentially same as before for main UI.
        Vector3 rayOrigin = droneRigidBody.transform.position - (Vector3.up * ALTITUDE_OFFSET);

        if (Physics.Raycast(rayOrigin, Vector3.down,
            out RaycastHit hit, MAX_RAY_DISTANCE, groundLayer))
        {
            droneAltitude = Mathf.Max(0f, hit.distance);
            Debug.DrawRay(rayOrigin, Vector3.down, Color.red); // Ray for visualization.
        }
    }

    private void UpdateUI()
    {
        if (speedValue) speedValue.text = Mathf.RoundToInt(droneSpeed) + " Km/h";
        if (altitudeText) altitudeText.text = Mathf.RoundToInt(droneAltitude) + " m";
    }
}