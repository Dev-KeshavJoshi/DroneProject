using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;

    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f
        );

        mat.color = Color.cyan * intensity;
    }
}
