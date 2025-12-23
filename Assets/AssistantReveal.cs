using UnityEngine;

public class AssistantReveal : MonoBehaviour
{
    public Vector3 finalScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float riseHeight = 1.2f;
    public float speed = 1.5f;

    Vector3 startPos;
    Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // come out of drone
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            startPos + Vector3.up * riseHeight,
            Time.deltaTime * speed
        );

        // grow in size
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            finalScale,
            Time.deltaTime * speed
        );
    }
}
