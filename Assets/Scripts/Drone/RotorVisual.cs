using UnityEngine;

public class RotorVisual : MonoBehaviour
{
    public Motor motor;
    public bool clockwise = true;

    [Header("Visual Settings")]
    public float maxVisualRPM = 3000f;
    public float blurStartRPM = 1500f;

    public GameObject solidBlade;
    public GameObject blurDisc;

    void Update()
    {
        float rpm = motor.CurrentRPM;
        float visualRPM = Mathf.Min(rpm, maxVisualRPM);

        float rotationSpeed = visualRPM * 6f * Time.deltaTime;
        transform.Rotate(Vector3.up * (clockwise ? rotationSpeed : -rotationSpeed));

        // Swap blade ? blur
        bool blur = rpm > blurStartRPM;
        solidBlade.SetActive(!blur);
        blurDisc.SetActive(blur);

        // Slight tilt for realism
        float tilt = motor.CurrentThrust * 0.2f;
        transform.localRotation *= Quaternion.Euler(tilt, 0f, tilt);
    }
}
