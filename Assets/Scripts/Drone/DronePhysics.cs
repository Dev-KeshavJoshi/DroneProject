using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DronePhysics : MonoBehaviour
{
    public Motor[] motors;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (Motor motor in motors)
        {
            motor.UpdateMotor(motor.throttle);

            Vector3 force = motor.motorTransform.up * motor.CurrentThrust;
            rb.AddForceAtPosition(force, motor.motorTransform.position);
        }
    }
}
