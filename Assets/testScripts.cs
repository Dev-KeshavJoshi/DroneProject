using UnityEngine;

public class testScripts : MonoBehaviour
{
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            rigidBody.AddForce(Vector3.up * 10, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftControl))
            rigidBody.AddForce(Vector3.down * 10, ForceMode.Force);

    }
}
