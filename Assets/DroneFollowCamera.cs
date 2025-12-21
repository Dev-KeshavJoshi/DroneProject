using UnityEngine;

public class DroneFollowCamera : MonoBehaviour
{
    public Transform drone;              // Haanth
    public Vector3 offset = new Vector3(0, 2.5f, -6f);
    public float followSpeed = 5f;
    public float rotateSpeed = 5f;

    void LateUpdate()
    {
        if (drone == null) return;

        // Target position
        Vector3 desiredPosition = drone.position + drone.rotation * offset;

        // Smooth position follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * followSpeed
        );

        // Smooth rotation (always look at drone)
        Quaternion desiredRotation = Quaternion.LookRotation(
            drone.position - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * rotateSpeed
        );
    }
}
