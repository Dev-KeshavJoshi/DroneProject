using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint1_3 : ICheckpoint
{
    private readonly MonoBehaviour _context;
    private readonly Rigidbody _droneRigidBody;
    private readonly TextMeshProUGUI _subtitleText;
    private readonly TextMeshProUGUI _tutorialText;
    private readonly TextMeshProUGUI _speedText; // To change color

    // integrity
    private float _motorIntegrity = 100f;
    private const float MAX_INTEGRITY = 100f;
    
    // Speed constraints
    private const float MIN_SPEED_THRESHOLD = 15f; // Below this causes damage
    private const float MAX_SPEED_THRESHOLD = 25f; // Above this causes damage
    private const float OPTIMAL_MIN = 18f;
    private const float OPTIMAL_MAX = 22f;
    private const float DECAY_RATE = 20f; // Integrity loss per second
    private const float REGEN_RATE = 5f;  // Integrity regen per second (if needed, or 0)

    // Gates
    private List<GameObject> _gates;
    private int _gatesPassed = 0;
    private int _totalGates = 0;
    private bool _isCompleted = false;

    // Visuals
    private GameObject _gateContainer;

    public bool IsCompleted => _isCompleted;

    public Checkpoint1_3(
        MonoBehaviour context,
        Rigidbody droneRigidBody,
        TextMeshProUGUI subtitleText,
        TextMeshProUGUI tutorialText,
        TextMeshProUGUI speedText)
    {
        _context = context;
        _droneRigidBody = droneRigidBody;
        _subtitleText = subtitleText;
        _tutorialText = tutorialText;
        _speedText = speedText;
    }

    public void Enter()
    {
        _isCompleted = false;
        _motorIntegrity = 100f;
        _gatesPassed = 0;

        ShowIntroduction();
        SpawnGates();
    }

    private void ShowIntroduction()
    {
        if (_subtitleText)
        {
            _subtitleText.gameObject.SetActive(true);
            _subtitleText.text = "Checkpoint 1.3: Controlled Flight\nMaintain 20 km/h (+/- 2). Pass all gates.";
        }
        _context.StartCoroutine(HideSubtitleText(5f));
    }

    public void UpdateCheckpoint()
    {
        if (_isCompleted) return;

        float speedKmph = _droneRigidBody.linearVelocity.magnitude * 3.6f;

        UpdateIntegrity(speedKmph);
        UpdateUI(speedKmph);

        // Fail condition (Optional: Reset checkpoint if integrity 0)
        if (_motorIntegrity <= 0)
        {
             // Reset
             ResetCheckpoint();
        }

        if (_gatesPassed >= _totalGates && _totalGates > 0)
        {
            CompleteCheckpoint();
        }
    }

    private void  UpdateIntegrity(float speed)
    {
        bool inDanger = speed < MIN_SPEED_THRESHOLD || speed > MAX_SPEED_THRESHOLD;

        // If we are grounded (not flying), maybe don't drain integrity for low speed?
        // Requirement says "rapid altitude loss" but we are just simulating integrity decay.
        // Let's assume low speed danger only applies if we are airborne? 
        // For simplicity: integrity decays if speed is out of bounds.

        if (speed >= OPTIMAL_MIN && speed <= OPTIMAL_MAX)
        {
            // Green zone: Recover slightly or stable
             _motorIntegrity = Mathf.Min(_motorIntegrity + REGEN_RATE * Time.deltaTime, MAX_INTEGRITY);
        }
        else if (inDanger)
        {
             _motorIntegrity -= DECAY_RATE * Time.deltaTime;
        }
    }

    private void UpdateUI(float speed)
    {
        // HUD Update
        string integrityColor = _motorIntegrity > 50 ? "green" : "red";
        string status = $"Checkpoint 1.3: Gates {_gatesPassed}/{_totalGates}\n";
        status += $"Integrity: <color={integrityColor}>{_motorIntegrity:F0}%</color>\n";
        
        if (speed < OPTIMAL_MIN) status += "<color=yellow>Speed Low!</color>";
        else if (speed > OPTIMAL_MAX) status += "<color=red>Speed High!</color>";
        else status += "<color=green>Speed Optimal</color>";

        if (_tutorialText) _tutorialText.text = status;

        // Speedometer Color
        if (_speedText)
        {
            if (speed >= OPTIMAL_MIN && speed <= OPTIMAL_MAX) _speedText.color = Color.green;
            else if (speed > MAX_SPEED_THRESHOLD) _speedText.color = Color.red;
            else _speedText.color = Color.white;
        }
    }

    private void SpawnGates()
    {
        if (_gateContainer != null) Object.Destroy(_gateContainer);
        _gateContainer = new GameObject("Checkpoint1_3_Gates");
        _gates = new List<GameObject>();

        // Create a simple forward path logic relative to drone start or fixed in world
        // Let's create 4 gates in a line/curve forward from current position
        Vector3 startPos = _droneRigidBody.position + _droneRigidBody.transform.forward * 10f + Vector3.up * 2f;
        Vector3 direction = _droneRigidBody.transform.forward;

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = startPos + (direction * 20f * i) + (Vector3.up * (i % 2 == 0 ? 0 : 5)); // Slight sine wave alt
            CreateGate(pos, i);
        }
        
        _totalGates = _gates.Count;
    }

    private void CreateGate(Vector3 position, int index)
    {
        GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube); // Placemarker visual
        gate.name = $"Gate_{index}";
        gate.transform.position = position;
        gate.transform.localScale = new Vector3(8f, 5f, 1f);
        gate.GetComponent<Collider>().isTrigger = true;
        
        // Remove solid collider, make it trigger
        // Make it semi-transparent
        Renderer r = gate.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Standard")); // Or simpler
        r.material.color = new Color(0, 1, 1, 0.3f);
        StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Transparent);

        // Add script
        CheckpointGate cg = gate.AddComponent<CheckpointGate>();
        cg.Initialize(this);
        
        gate.transform.SetParent(_gateContainer.transform);
        _gates.Add(gate);
    }

    public void OnGatePassed(CheckpointGate gate)
    {
        if (_gates.Contains(gate.gameObject))
        {
            _gatesPassed++;
            // Optional: Destroy gate or turn green? Script handles green.
            // Disable trigger to prevent double count
             gate.GetComponent<Collider>().enabled = false;
        }
    }

    private void ResetCheckpoint()
    {
        _motorIntegrity = 100f;
        _gatesPassed = 0;
        if (_subtitleText) _subtitleText.text = "Integrity Failure! Rerunning Checkpoint...";
        _context.StartCoroutine(HideSubtitleText(2f));
        SpawnGates(); 
    }

    private void CompleteCheckpoint()
    {
        _isCompleted = true;
        
        if (_subtitleText)
        {
             _subtitleText.gameObject.SetActive(true);
             _subtitleText.text = "Checkpoint 1.3 Completed.";
        }
        Exit();
    }

    public void Exit()
    {
        if (_gateContainer) Object.Destroy(_gateContainer);
        if (_speedText) _speedText.color = Color.white; // Reset
    }

    private IEnumerator HideSubtitleText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_subtitleText) _subtitleText.gameObject.SetActive(false);
    }
}

// Utility for setting Transparency at runtime if standard shader is used
public static class StandardShaderUtils
{
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }
    }
}
