using UnityEngine;
using TMPro;

public class Checkpoint1_2 : ICheckpoint
{
    private readonly MonoBehaviour _context;
    private readonly Rigidbody _droneRigidBody;
    private readonly TextMeshProUGUI _subtitleText;

    // Visuals
    private readonly GameObject _rollArrow;
    private readonly GameObject _pitchArrow;
    private readonly GameObject _yawArrow;
    private readonly GameObject _forceVectorDiagram;
    private readonly TextMeshProUGUI _angleText; // To display Phi, Theta, Psi

    private bool _completedRoll;
    private bool _completedPitch;
    private bool _completedYaw;
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public Checkpoint1_2(
        MonoBehaviour context,
        Rigidbody droneRigidBody,
        TextMeshProUGUI subtitleText,
        GameObject rollArrow,
        GameObject pitchArrow,
        GameObject yawArrow,
        GameObject forceVectorDiagram,
        TextMeshProUGUI angleText)
    {
        _context = context;
        _droneRigidBody = droneRigidBody;
        _subtitleText = subtitleText;
        _rollArrow = rollArrow;
        _pitchArrow = pitchArrow;
        _yawArrow = yawArrow;
        _forceVectorDiagram = forceVectorDiagram;
        _angleText = angleText;
    }

    public void Enter()
    {
        _isCompleted = false;
        _completedRoll = false;
        _completedPitch = false;
        _completedYaw = false;
        
        _subtitleText.gameObject.SetActive(true);
        _subtitleText.text = "Checkpoint 1.2: Axis Control\nPerform Roll, Pitch, and Yaw movements.";

        SetVisualsActive(false);
        if (_forceVectorDiagram) _forceVectorDiagram.SetActive(true);
    }

    private void SetVisualsActive(bool active)
    {
        if (_rollArrow) _rollArrow.SetActive(active);
        if (_pitchArrow) _pitchArrow.SetActive(active);
        if (_yawArrow) _yawArrow.SetActive(active);
        if (_angleText) _angleText.gameObject.SetActive(active);
    }

    public void UpdateCheckpoint()
    {
        if (_isCompleted) return;

        UpdateAxisLogic();
        UpdateUI();

        if (_completedRoll && _completedPitch && _completedYaw)
        {
            _isCompleted = true;
            _subtitleText.text = "Checkpoint 1.2 Completed";
            Exit();
        }
    }

    public void Exit()
    {
        SetVisualsActive(false);
        if (_forceVectorDiagram) _forceVectorDiagram.SetActive(false);
    }

    private void UpdateAxisLogic()
    {
        // Get localized angles. 
        // Unity Euler angles are 0-360. We convert to +/- 180 for easier reading.
        Vector3 euler = _droneRigidBody.transform.eulerAngles;
        float roll = NormalizeAngle(euler.z);  // Z is usually Roll in Unity
        float pitch = NormalizeAngle(euler.x); // X is Pitch
        // Yaw is specific. We might want to track input or delta, but absolute heading can also work if we just want "some yaw".
        // The requirement says "Circular arrow shows rotation". Let's track change or just current heading if we reset.
        // Or simpler: check inputs or just check if they reached a certain angle from start?
        // Let's check for significant tilt/rotation.

        bool isRolling = Mathf.Abs(roll) > 10f;
        bool isPitching = Mathf.Abs(pitch) > 10f;
        
        // For Yaw, since it's free spinning, we might check input or just rotation speed ideally.
        // But checking angular velocity is good too.
        bool isYawing = Mathf.Abs(_droneRigidBody.angularVelocity.y) > 0.5f;

        // Visuals Interaction
        bool showRoll = isRolling || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        bool showPitch = isPitching || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
        bool showYaw = isYawing || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E);

        if (_rollArrow) _rollArrow.SetActive(showRoll);
        if (_pitchArrow) _pitchArrow.SetActive(showPitch);
        if (_yawArrow) _yawArrow.SetActive(showYaw);

        // Completion Logic
        if (Mathf.Abs(roll) > 15f) _completedRoll = true;
        if (Mathf.Abs(pitch) > 15f) _completedPitch = true;
        
        // For Yaw, let's say if they hold it for a bit or rotate past a certain amount.
        // Simple check: if they create enough torque/speed.
        if (Mathf.Abs(_droneRigidBody.angularVelocity.y) > 1.0f) _completedYaw = true; 
        
        // Update Angle Text
        if (_angleText)
        {
            string status = "";
            if (showRoll) status += $"Roll(Φ): {roll:F1}° ";
            if (showPitch) status += $"Pitch(θ): {pitch:F1}° ";
            if (showYaw) status += $"Yaw(Ψ): {euler.y:F1}°"; // Yaw is absolute here 0-360
            
            if (string.IsNullOrEmpty(status))
                _angleText.text = "Neutral";
            else
                _angleText.text = status;
        }

        // We could verify via "demonstration logic" - e.g. "Good Roll!", "Good Pitch!" etc.
        // For now, we accumulate completion.
        
        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        string msg = "Checkpoint 1.2: Axis Control\n";
        msg += _completedRoll ? "[X] Roll " : "[ ] Roll ";
        msg += _completedPitch ? "[X] Pitch " : "[ ] Pitch ";
        msg += _completedYaw ? "[X] Yaw" : "[ ] Yaw";
        _subtitleText.text = msg;
    }

    private void UpdateUI()
    {
        // Force diagram update would happen here if we had a reference to the script controlling it.
        // For now, we assume the diagram is a static or animated visual that just needs to be Active.
        // If it needs real-time updating based on forces, we would need to calculate force vectors.
        // Requirements: "A simplified force vector diagram updates in real-time"
        // This suggests we might need to modify a rect transform of an arrow in the UI.
        // I will assume for now we just show it, as I don't have the assets for the diagram itself yet.
        // I'll leave a TODO or simple rotation of the diagram if it's a single arrow.
        
        if (_forceVectorDiagram != null)
        {
           // Simple feedback: Ensure it's visible. 
           // If it's a UI element representing net force, we could rotate it.
           // Let's try to rotate the whole diagram object to match drone tilt as a simple effect?
           // _forceVectorDiagram.transform.rotation = _droneRigidBody.rotation; 
           // (might be too complex for 2D UI without knowing structure).
        }
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
