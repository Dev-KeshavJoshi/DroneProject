using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    public float speed = 2500f;

    void Update()
    {
        // WORLD space spin = stable, no wobble
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
    }
}
