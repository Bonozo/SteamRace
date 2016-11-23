using UnityEngine;

public class MonowheelDrivingBehaviour : MonoBehaviour
{
    [SerializeField]
    private float motorTorque = 1000.0f;

    [SerializeField]
    private float brakeTorque = 500.0f;

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

    [SerializeField]
    private AnimationCurve gyroscopicPickupRate = new AnimationCurve();

    [SerializeField]
    private AudioSource engineAudioSource = null;

    private float currentSpeed;
    private float deltaSpeed;
    private float leanAngle;

    private void FixedUpdate()
    {
        // set angular velocity calculated from lean angle and speed
        rigidbody.angularVelocity = Vector3.up * -leanAngle * rigidbody.velocity.magnitude * 0.25f * Time.fixedDeltaTime;

        // set current and delta speed values
        if (rigidbody != null)
        {
            float prevousSpeed = currentSpeed;
            currentSpeed = rigidbody.velocity.magnitude;
            deltaSpeed = currentSpeed - prevousSpeed;
        }

        UpdateAcceleration();
        UpdateGyroscopicPickup();
        UpdateTurning();
        UpdateEngineSound();
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's forward and braking acceleration.
    /// </summary>
    private void UpdateAcceleration()
    {
        if (wheelCollider != null)
        {
            wheelCollider.motorTorque = motorTorque * Input.GetAxis("Accelerator");
            wheelCollider.brakeTorque = brakeTorque * Input.GetAxis("Brake");

            // wheel mesh rotation
            if (wheelMeshTransform != null)
            {
                wheelMeshTransform.Rotate(Vector3.up, wheelCollider.rpm / 60 * 360 * Time.fixedDeltaTime, Space.Self);
            }
        }
    }

    /// <summary>
    ///     Handles the engine audio.
    /// </summary>
    private void UpdateEngineSound()
    {
        if (engineAudioSource != null)
        {
            engineAudioSource.pitch = 0.75f + (currentSpeed * 0.075f);
        }
    }

    /// <summary>
    ///     Handles gyroscopic pickup from acceleration forces.
    /// </summary>
    private void UpdateGyroscopicPickup()
    {
        if (leanAngle > 0.0f)
        {
            leanAngle = Mathf.Clamp(leanAngle - gyroscopicPickupRate.Evaluate(deltaSpeed), 0.0f, float.MaxValue);
        }
        else if (leanAngle < 0.0f)
        {
            leanAngle = Mathf.Clamp(leanAngle + gyroscopicPickupRate.Evaluate(deltaSpeed), float.MinValue, 0.0f);
        }
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's turning.
    /// </summary>
    private void UpdateTurning()
    {
        float leanAmount = Input.GetAxis("Lean");

        // input handler
        if (Mathf.Abs(leanAmount) > 0.0f)
        {
            leanAngle = leanAngle - (velocityLeanSpeed.Evaluate(currentSpeed) * leanAmount * Time.fixedDeltaTime);
        }

        // clamp the lean angle based on current speed
        float absoluteMaximumLeanAngle = velocityLeanAngle.Evaluate(currentSpeed);
        leanAngle = Mathf.Clamp(leanAngle, -absoluteMaximumLeanAngle, absoluteMaximumLeanAngle);

        // sets the lean angle on the transform
        Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.z = leanAngle;
        transform.localEulerAngles = localEulerAngles;
    }
}