using UnityEngine;
using TMPro;
using System.Collections;

public class Checkpoint1_1 : ICheckpoint
{
    private readonly MonoBehaviour _context;
    private readonly Rigidbody _droneRigidBody;
    private readonly TextMeshProUGUI _subtitleText;
    private readonly TextMeshProUGUI _tutorialText;
    private readonly LayerMask _groundLayer;

    private const float MAX_RAY_DISTANCE = 1000f;
    private const float ALTITUDE_OFFSET = 0.4f; // Based on original script
    private const float NEWTON_TRIGGER_ALTITUDE = 5f;
    private const float NEWTON_TOLERANCE = 0.6f;
    private const float TARGET_ALTITUDE = 15f;
    private const float ALTITUDE_TOLERANCE = 1.2f;
    private const float HOLD_DURATION = 5f;

    private float _droneAltitude;
    private float _holdTimer;
    private float _instabilityTimer;
    private bool _newtonLawShown;
    private bool _isCompleted;
    private bool _isHolding;

    public bool IsCompleted => _isCompleted;

    public Checkpoint1_1(
        MonoBehaviour context,
        Rigidbody droneRigidBody,
        TextMeshProUGUI subtitleText,
        TextMeshProUGUI tutorialText,
        LayerMask groundLayer)
    {
        _context = context;
        _droneRigidBody = droneRigidBody;
        _subtitleText = subtitleText;
        _tutorialText = tutorialText;
        _groundLayer = groundLayer;
    }

    public void Enter()
    {
        _isCompleted = false;
        _newtonLawShown = false;
        _isHolding = false;

        _subtitleText.gameObject.SetActive(true);
        _subtitleText.text = "Checkpoint 1.1: Liftoff & Hover";

        _context.StartCoroutine(HideSubtitleText(4f));

        _tutorialText.gameObject.SetActive(true);
        _tutorialText.text = "Handle Throttle:  Space Key = Up Thrust & Left Control = Down Thrust ";
    }

    public void UpdateCheckpoint()
    {
        if (_isCompleted) return;

        UpdateAltitude();
        HandleNewtonThirdLaw();
        HandleHoldLogic();
    }

    public void Exit()
    {
        // Code for cleanup.
        _tutorialText.gameObject.SetActive(true);
        _tutorialText.text = "Checkpoint 1.1 Completed";

        _context.StartCoroutine(HideTutorialText(4f));
    }

    private void UpdateAltitude()
    {
        Vector3 rayOrigin = _droneRigidBody.transform.position - (Vector3.up * ALTITUDE_OFFSET);
        if (Physics.Raycast(rayOrigin, Vector3.down,
            out RaycastHit hit, MAX_RAY_DISTANCE, _groundLayer))
        {
            _droneAltitude = Mathf.Max(0f, hit.distance);
            Debug.DrawRay(rayOrigin, Vector3.down, Color.red); // Ray for visualization.
        }
        else
        {
            _droneAltitude = 0; // Default if no ground found
        }
    }

    private void HandleNewtonThirdLaw()
    {
        if (_newtonLawShown) return;

        if (Mathf.Abs(_droneAltitude - NEWTON_TRIGGER_ALTITUDE) <= NEWTON_TOLERANCE)
        {
            _newtonLawShown = true;
            _tutorialText.gameObject.SetActive(false);
            _subtitleText.gameObject.SetActive(true);
            _subtitleText.text = "⬇ Thrust Force\n\nNewton's 3rd Law: Every action has an equal and opposite reaction";
            
            // Note: In a pure non-MonoBehaviour class, we can't use Invoke directly comfortably without context.
            // Using a coroutine or a timer variable is better, but to keep it simple and close to original:
            _context.StartCoroutine(HideSubtitleText(4f));

            _tutorialText.gameObject.SetActive(true);
            _tutorialText.text = "Stablize altitude at 15 m.";
        }
    }

    private IEnumerator HideSubtitleText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_isHolding && !_isCompleted)
        {
            _subtitleText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideTutorialText(float delay)
    {
        yield return new WaitForSeconds(delay);
        _tutorialText.gameObject.SetActive(false);
    }

    private void HandleHoldLogic()
    {
        bool altitudeStable = Mathf.Abs(_droneAltitude - TARGET_ALTITUDE) <= ALTITUDE_TOLERANCE;
        Vector3 droneVelocity = _droneRigidBody.linearVelocity;
        bool velocityStable = Mathf.Abs(droneVelocity.y) < 0.8f && new Vector2(droneVelocity.x, droneVelocity.z).magnitude < 1.2f;

        if (!_isHolding && altitudeStable)
        {
            _isHolding = true;
            _holdTimer = 0f;
            _instabilityTimer = 0f;
            _tutorialText.gameObject.SetActive(true);
            _tutorialText.text = "Hold position";
        }

        if (!_isHolding) return;

        if (!altitudeStable || !velocityStable)
        {
            _instabilityTimer += Time.deltaTime;
            if (_instabilityTimer > 1.2f)
            {
                _holdTimer = Mathf.Max(0f, _holdTimer - Time.deltaTime * 2f);
                _tutorialText.text = "Stablize altitude at 15 m.";
            }
            return;
        }

        _instabilityTimer = 0f;
        _holdTimer += Time.deltaTime;
        _tutorialText.text = $"Hold for {Mathf.CeilToInt(HOLD_DURATION - _holdTimer)} seconds";

        if (_holdTimer >= HOLD_DURATION)
        {
            _isCompleted = true;
        }
    }
}
