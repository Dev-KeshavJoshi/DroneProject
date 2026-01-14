using UnityEngine;
using System.Collections;

public class CheckpointGate : MonoBehaviour
{
    private Checkpoint1_3 _parentCheckpoint;
    private bool _triggered = false;
    private Renderer _renderer;
    private Material _originalMaterial;

    public void Initialize(Checkpoint1_3 parent)
    {
        _parentCheckpoint = parent;
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        // Check if it's the drone
        if (other.GetComponentInParent<Rigidbody>() != null || other.name.Contains("Drone"))
        {
            _triggered = true;
            if (_parentCheckpoint != null)
            {
                _parentCheckpoint.OnGatePassed(this);
            }
            
            // Visual feedback
            if (_renderer != null)
            {
                _renderer.material.color = Color.green;
            }
        }
    }
}
