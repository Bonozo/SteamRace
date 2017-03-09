using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour {

    public GameObject monoWheel;
    public Text speedometerText;
    public Text altitudeText;

    private MonowheelDrivingBehaviour drivingBehavior;

    void Awake()
    {
        drivingBehavior = monoWheel.GetComponent<MonowheelDrivingBehaviour>();
    }


    // Update is called once per frame
    void Update ()
    {
        Quaternion rot = monoWheel.transform.rotation;
        transform.localRotation = Quaternion.Inverse(rot);

        if(drivingBehavior)
        {
            speedometerText.text = "Speed " + (int)drivingBehavior.currentSpeed;
            altitudeText.text = "Altitude " + drivingBehavior.altitude.ToString("F2");
            if(drivingBehavior.altitude > 1.0f)
            {
                altitudeText.color = Color.red;
            }
            else
            {
                altitudeText.color = Color.green;
            }
        }
    }
}
