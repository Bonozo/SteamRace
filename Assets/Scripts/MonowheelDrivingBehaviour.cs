using UnityEngine;

public class MonowheelDrivingBehaviour : MonoBehaviour
{
    [SerializeField]
    private float motorTorque = 1000.0f;

    [SerializeField]
    private Transform wheelMeshTransform = null;

    [SerializeField]
    private WheelCollider wheelCollider = null;

    [SerializeField]
    private new Rigidbody rigidbody = null;

    [SerializeField]
    private AnimationCurve velocityLeanAngle = new AnimationCurve(new[]
    {
        new Keyframe(0.0f, 0.0f),
        new Keyframe(7.5f, 45.0f)
    });

    [SerializeField]
    private AnimationCurve velocityLeanSpeed = new AnimationCurve(new[]
    {
        new Keyframe(0.0f, 0.0f),
        new Keyframe(3.0f, 25.0f),
        new Keyframe(15.0f, 15.0f)
    });

    private float leanAngle;

    private void FixedUpdate()
    {
        // set angular velocity calculated from lean angle and speed
        rigidbody.angularVelocity = Vector3.up * -leanAngle * rigidbody.velocity.magnitude * 0.25f * Time.fixedDeltaTime;
    }

    private void Update()
    {
        // cache velocity magnitude as the current speed
        float currentSpeed = 0.0f;
        if (rigidbody != null)
        {
            currentSpeed = rigidbody.velocity.magnitude;
        }

        UpdateAcceleration(currentSpeed);
        UpdateTurning(currentSpeed);
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's forward and braking acceleration.
    /// </summary>
    private void UpdateAcceleration(float currentSpeed)
    {
        if (wheelCollider != null)
        {
            // input handling
            if (Input.GetKey(KeyCode.W))
            {
                // forward
                wheelCollider.motorTorque = motorTorque;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                // braking
                if (currentSpeed > 0.0f)
                {
                    wheelCollider.motorTorque = -(motorTorque * 0.5f);
                }
            }
            else
            {
                // coasting
                wheelCollider.motorTorque = 0.0f;
            }

            // wheel mesh rotation
            if (wheelMeshTransform != null)
            {
                wheelMeshTransform.Rotate(Vector3.up, wheelCollider.rpm / 60 * 360 * Time.deltaTime, Space.Self);
            }
        }
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's turning.
    /// </summary>
    private void UpdateTurning(float currentSpeed)
    {
        // input handling
        if (Input.GetKey(KeyCode.A))
        {
            // lean left
            leanAngle = leanAngle + (velocityLeanSpeed.Evaluate(currentSpeed) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // lean right
            leanAngle = leanAngle - (velocityLeanSpeed.Evaluate(currentSpeed) * Time.deltaTime);
        }

        // clamp the lean angle based on speed
        float absoluteMaximumLeanAngle = velocityLeanAngle.Evaluate(currentSpeed);
        leanAngle = Mathf.Clamp(leanAngle, -absoluteMaximumLeanAngle, absoluteMaximumLeanAngle);

        // sets the lean angle on the transform
        Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.z = leanAngle;
        transform.localEulerAngles = localEulerAngles;
    }
}