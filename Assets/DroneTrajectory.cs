using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DroneTrajectory : MonoBehaviour
{
    public int pointCount = 30;
    public float spacing = 0.5f;

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = pointCount;
    }

    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 dir = transform.forward;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 point = startPos + dir * spacing * i;
            line.SetPosition(i, point);
        }
    }
}
