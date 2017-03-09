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

    [SerializeField]
    private AudioSource impactSfx = null;

    [SerializeField]
    private GameObject explosionObject;

    [SerializeField]
    [Tooltip("Min. speed before halt or bounceback to determine crash")]
    private float crashThreshold = -4.0f;

    [SerializeField]
    [Tooltip("Drag coefficient for when not in air")]
    private float dragOnGround = 0.5f;

    public float maxSpeed = 20.0f;

    public float currentSpeed;
    public float altitude;

    private float deltaSpeed;
    private float leanAngle;
    private bool isGrounded;

    void Awake()
    {
    }

    private void FixedUpdate()
    {
        // set current and delta speed values
        float prevSpeed = currentSpeed;
        if (rigidbody != null)
        {
            currentSpeed = rigidbody.velocity.magnitude;
            deltaSpeed = currentSpeed - prevSpeed;
        }

        // set angular velocity calculated from lean angle and speed
        float vehicleDirection = Mathf.Sign(wheelCollider.motorTorque);
        rigidbody.angularVelocity = Vector3.up * -leanAngle * (vehicleDirection * angularVelocityCurve.Evaluate(rigidbody.velocity.magnitude)) * Time.fixedDeltaTime;

        UpdateGroundDetection();
        UpdateCrashDetection(prevSpeed);
        UpdateAcceleration();
        UpdateMaxSpeed();
        UpdateGyroscopicPickup();
        UpdateTurning();
        UpdateEngineSound();
    }

    private void UpdateMaxSpeed()
    {
        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
    }

    private void UpdateCrashDetection(float prevSpeed)
    {
        Vector3 velocity = rigidbody.velocity;
        Vector3 localVel = transform.InverseTransformDirection(velocity);

        if(prevSpeed > crashThreshold)
        {
            // if halted or bounced back after going fast, we crashed
            if (localVel.z <= 0.0f)
            {
                Instantiate(explosionObject, transform.position, Quaternion.identity);
            }
        }
    }


    private void UpdateGroundDetection()
    {
        isGrounded = true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 500.0f))
        {
            altitude = hit.distance;

            if (altitude > 0.9f)
            {
                rigidbody.drag = 0.0f;
                isGrounded = false;
            }
            else
            {
                rigidbody.drag = dragOnGround;
            }
        }
    }

    //Run Along Normals.
    private void UpdateCollision()
    {
        RaycastHit hit;

        // cast a ray to the right of the player object
        if (Physics.Raycast(transform.position+ new Vector3(0,2,0), transform.TransformDirection(Vector3.forward), out hit, 5))
        {
            // orient the Moving Object's Left direction to Match the Normals on his Right
            var RunnerRotation = Quaternion.FromToRotation(Vector3.left, hit.normal);

            //Smooth rotation
            //transform.rotation = Quaternion.Slerp(transform.rotation, RunnerRotation, Time.deltaTime * 10);
            //transform.RotateAround(Vector3.up, 10.0f);
        }
    }


    private void Update()
    {
        //UpdateCollision();
        UpdateTailfinAngle();
    }

    /// <summary>
    ///     Handles input and updating of the vehicle's forward and braking acceleration.
    /// </summary>
    private void UpdateAcceleration()
    {
        if(isGrounded == false)
        {
            return;
        }

        if (wheelCollider != null)
        {
            wheelCollider.motorTorque = motorTorqueCurve.Evaluate(currentSpeed) * Input.GetAxis("Accelerator")
                + -motorTorqueCurve.Evaluate(currentSpeed) * Input.GetAxis("Brake");
            //wheelCollider.brakeTorque = brakeTorque * Input.GetAxis("Brake");

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
        float leanMultiplier = 1.0f;
        if(Input.GetButton("LeanHard"))
        {
            //Debug.Log("Leanhard");
            leanMultiplier = 1.3f;
        }

        float leanAmount = Input.GetAxis("Lean");

        // input handler
        if (Mathf.Abs(leanAmount) > 0.0f)
        {
            leanAngle = leanAngle - (velocityLeanSpeed.Evaluate(currentSpeed) * leanAmount * Time.fixedDeltaTime);
        }

        // clamp the lean angle based on current speed
        float absoluteMaximumLeanAngle = velocityLeanAngle.Evaluate(currentSpeed);
        leanAngle = Mathf.Clamp(leanAngle, -absoluteMaximumLeanAngle * leanMultiplier, absoluteMaximumLeanAngle * leanMultiplier);

        // sets the lean angle on the transform
        Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.z = Mathf.LerpAngle(localEulerAngles.z, leanAngle, Time.fixedDeltaTime * 10.0f);
        transform.localEulerAngles = localEulerAngles;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ramp")
        {
            rigidbody.velocity *= 1.2f;
            return;
        }

        // process slowdown on bump.  note: adjust bump sensitivity with relativeVelocity 
        if (collision.relativeVelocity.magnitude >= 0)
        {
            rigidbody.velocity *= 0.8f;

            impactSfx.Play();
        }
    }

    /*
        void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
        }
    */
}