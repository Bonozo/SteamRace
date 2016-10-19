using UnityEngine;

public class MonowheelDrivingBehaviour : MonoBehaviour
{
    [SerializeField]
    private float motorTorque = 1000.0f;

    [SerializeField]
    private Transform wheelMeshTransform = null;

    [SerializeField]
    private WheelCollider wheelCollider = null;

    private void Update()
    {
        if (wheelCollider != null)
        {
            if (Input.GetKey(KeyCode.W))
            {
                wheelCollider.motorTorque = motorTorque;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                wheelCollider.motorTorque = -(motorTorque * 0.5f);
            }
            else
            {
                wheelCollider.motorTorque = 0.0f;
            }

            if (wheelMeshTransform != null)
            {
                wheelMeshTransform.Rotate(Vector3.up, wheelCollider.rpm / 60 * 360 * Time.deltaTime, Space.Self);
            }
        }
    }
}