using UnityEngine;

public class ProperDroneAnimation : MonoBehaviour
{
    [SerializeField] private Transform[] props;
    [SerializeField] private float propSpeed = 1500f;

    void Update()
    {
        RotateProps();
    }

    void RotateProps()
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

