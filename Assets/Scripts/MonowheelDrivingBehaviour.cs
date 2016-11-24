using UnityEngine;

public class MonowheelDrivingBehaviour : MonoBehaviour
{
    [SerializeField]
    private float brakeTorque = 500.0f;

    [SerializeField]
    private Transform wheelMeshTransform = null;

    [SerializeField]
    private Transform tailfinTransform = null;

    [SerializeField]
    private WheelCollider wheelCollider = null;

    [SerializeField]
    private new Rigidbody rigidbody = null;

    [SerializeField]
    private AnimationCurve motorTorqueCurve = new AnimationCurve();

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
    private AnimationCurve angularVelocityCurve = new AnimationCurve();

    [SerializeField]
    private AnimationCurve gyroscopicPickupRate = new AnimationCurve();

    [SerializeField]
    private AudioSource engineAudioSource = null;

    private float currentSpeed;
    private float deltaSpeed;
    private float leanAngle;

    private void FixedUpdate()
    {
        // set current and delta speed values
        if (rigidbody != null)
        {
            float prevousSpeed = currentSpeed;
            currentSpeed = rigidbody.velocity.magnitude;
            deltaSpeed = currentSpeed - prevousSpeed;
        }

        // set angular velocity calculated from lean angle and speed
        rigidbody.angularVelocity = Vector3.up * -leanAngle * angularVelocityCurve.Evaluate(rigidbody.velocity.magnitude) * Time.fixedDeltaTime;

        UpdateAcceleration();
        UpdateGyroscopicPickup();
        UpdateTurning();
        UpdateEngineSound();
    }

    private void Update()
    {
        UpdateTailfinAngle();
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's forward and braking acceleration.
    /// </summary>
    private void UpdateAcceleration()
    {
        if (wheelCollider != null)
        {
            wheelCollider.motorTorque = motorTorqueCurve.Evaluate(currentSpeed) * Input.GetAxis("Accelerator");
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
            engineAudioSource.volume = 0.5f + (Input.GetAxis("Accelerator") * 0.5f);
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
    ///     Handles the tailfin angle.
    /// </summary>
    private void UpdateTailfinAngle()
    {
        float desiredTailfinAngle = -Input.GetAxis("Lean") * 30.0f;

        Vector3 localEulerAngles = tailfinTransform.localEulerAngles;
        localEulerAngles.y = Mathf.LerpAngle(localEulerAngles.y, desiredTailfinAngle, Time.deltaTime * 5.0f);
        tailfinTransform.localEulerAngles = localEulerAngles;
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
        localEulerAngles.z = Mathf.LerpAngle(localEulerAngles.z, leanAngle, Time.fixedDeltaTime * 10.0f);
        transform.localEulerAngles = localEulerAngles;
    }
}