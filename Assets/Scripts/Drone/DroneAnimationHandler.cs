using UnityEngine;

public class DroneAnimationHandler : MonoBehaviour
{
    [Header("Propellers")]
    [SerializeField] private Transform Prop_FrontLeft;
    [SerializeField] private Transform Prop_FrontRight;
    [SerializeField] private Transform Prop_BackLeft;
    [SerializeField] private Transform Prop_BackRight;

    [Header("Spin Settings")]
    [SerializeField] private float maxRPM = 3500f;
    [SerializeField] private float spinSmoothness = 3f;

    private float currentRPM;
    private Transform[] Propellers;
    private DroneControllerFull droneControllerFull;

    private void Awake()
    {
        Propellers = new Transform[] { Prop_FrontLeft, Prop_FrontRight, Prop_BackLeft, Prop_BackRight };
        droneControllerFull = GetComponent<DroneControllerFull>();
    }

    private void Update()
    {
        RotatePropellers();
    }

    private void RotatePropellers()
    {
        float targetRPM = 0;

        if (!droneControllerFull.GetIsHovering())
        {
            targetRPM = droneControllerFull.CurrentThrottle * maxRPM;
        }
        else
        {
            targetRPM = droneControllerFull.HoveringThrottle * maxRPM;
        }

        currentRPM = Mathf.Lerp(
            currentRPM,
            targetRPM,
            spinSmoothness * Time.deltaTime
        );


        foreach (Transform Propeller in Propellers)
        {
            if (Propeller != null)
            {
                Propeller.Rotate(Vector3.forward, currentRPM * Time.deltaTime, Space.Self);
            }
        }
    }
}