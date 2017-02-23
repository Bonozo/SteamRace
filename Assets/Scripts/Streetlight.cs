using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streetlight : AudioUtility {

    private Rigidbody rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        if(rb)
        {
            rb.Sleep();
        }
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        AudioSource.Play();
	}
}
