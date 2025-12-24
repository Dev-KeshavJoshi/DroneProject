using UnityEngine;

public class BirdOut : MonoBehaviour
{
    public Transform exitPoint;     // Outside drone
    public Transform insidePoint;   // Inside drone

    bool moveOut = false;
    bool moveIn = false;

    void Start()
    {
        // Start inside & invisible
        transform.position = insidePoint.position;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            moveOut = true;
            moveIn = false;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            moveIn = true;
            moveOut = false;
        }

        // ----- MOVE OUT -----
        if (moveOut)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                exitPoint.position,
                Time.deltaTime * 3f
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.one,
                Time.deltaTime * 3f
            );
        }

        // ----- MOVE IN -----
        if (moveIn)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                insidePoint.position,
                Time.deltaTime * 3f
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.zero,
                Time.deltaTime * 3f
            );
        }
    }
}
