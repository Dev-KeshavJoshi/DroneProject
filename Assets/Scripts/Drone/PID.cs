using UnityEngine;

[System.Serializable]
public class PID
{
    public float kp = 1f;
    public float ki = 0f;
    public float kd = 0.1f;

    private float integral;
    private float lastError;

    public float Update(float error, float dt)
    {
        integral += error * dt;
        float derivative = (error - lastError) / dt;
        lastError = error;

        return kp * error + ki * integral + kd * derivative;
    }
}
