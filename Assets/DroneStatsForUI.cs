using TMPro;
using UnityEngine;

public class DroneStatsForUI : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] private LayerMask groundLayer;
    [Space(10)]

    [Header("Drone")]
    [SerializeField] private Rigidbody droneRigidBody;
    [Space(10)]

    [Header("DroneStats_UI")]
    [SerializeField] private TextMeshProUGUI speedValue;
    [SerializeField] private TextMeshProUGUI altitudeText;
    [Space(10)]

    private float droneSpeedKmph;
    private float droneAltitude;
    private const float KMPH_CONVERSION_NUM = 3.6f;
    private const float MAX_ALTITUDE_IN_METER = 100f;
    //private const float GROUND_LEVEL = 35f; //For 1st method of finding altitude.

    private void Update()
    {
        droneSpeedKmph = droneRigidBody.linearVelocity.magnitude * KMPH_CONVERSION_NUM;
        //1st method of finding altitude.
        //droneAltitude = droneRigidBody.transform.position.y - GROUND_LEVEL; 

        //2nd method of finding altitude.
        if (Physics.Raycast(droneRigidBody.transform.position, Vector3.down, 
            out RaycastHit hit, MAX_ALTITUDE_IN_METER, groundLayer))
        {
            droneAltitude = hit.distance;
        }

        speedValue.text = Mathf.RoundToInt(droneSpeedKmph) + " Km/h";
        altitudeText.text = Mathf.RoundToInt(droneAltitude) + " m";
    }
}
