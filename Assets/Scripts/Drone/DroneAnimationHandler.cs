using UnityEngine;

public class DroneAnimationHandler : MonoBehaviour
{
    [SerializeField] private Transform[] props;
    [SerializeField] private float propSpeed = 1500f;

    private void Update()
    {
        RotateProps();
    }

    private void RotateProps()
    {
        foreach (Transform prop in props)
        {
            if (prop != null)
            {
                prop.Rotate(Vector3.forward, propSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}

