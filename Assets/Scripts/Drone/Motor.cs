using UnityEngine;

[System.Serializable]
public class Motor
{
    public Transform motorTransform;

    [Header("Motor Properties")]
    public float maxRPM = 12000f;
    public float propDiameter = 0.254f; // 10 inches
    public float thrustCoefficient = 0.1f;
    public float airDensity = 1.225f;

    [Range(0f, 1f)]
    public float throttle;

    public float CurrentRPM { get; private set; }
    public float CurrentThrust { get; private set; }

    public void UpdateMotor(float targetThrottle)
    {
        throttle = Mathf.Clamp01(targetThrottle);

        // Smooth RPM response (ESC lag)
        CurrentRPM = Mathf.Lerp(CurrentRPM, throttle * maxRPM, Time.fixedDeltaTime * 10f);

        float n = CurrentRPM / 60f;
        CurrentThrust = thrustCoefficient * airDensity * n * n * Mathf.Pow(propDiameter, 4);
    }
}