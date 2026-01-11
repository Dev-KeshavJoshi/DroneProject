using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float rotationSpeedMultiplier = 2f;

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotateDir = +1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotateDir = -1f;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            rotateDir *= rotationSpeedMultiplier;
        }

        transform.eulerAngles += new Vector3(0, rotateDir * rotationSpeed * Time.deltaTime, 0);
    }
}
