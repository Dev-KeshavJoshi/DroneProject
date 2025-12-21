using UnityEngine;

public class BirdOut : MonoBehaviour
{
    public Transform exitPoint;
    bool start = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            gameObject.SetActive(true);
            start = true;
        }

        if (start)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                exitPoint.position,
                Time.deltaTime * 2f
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.one,
                Time.deltaTime
            );
        }
    }
}
