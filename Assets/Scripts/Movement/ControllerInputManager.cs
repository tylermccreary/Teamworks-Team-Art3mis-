using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;

    //Grabbing and Throwing
    public float throwForce = 1.5f;

    //Anticheat
    //public AntiCheat antiCheat;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    private void OnTriggerStay(Collider col)
    {
        Debug.Log("Your trigger is inside " + col.gameObject.name);
        if (col.gameObject.CompareTag("Throwable"))
        {
            /*if (antiCheat.isOutOfBounds)
            {
                Debug.Log("Enabling Anti_cheat mode");
                antiCheat.toggleCheatMode(true);
            }
            else
            {
                Debug.Log("Disabling Anti_cheat mode");
                antiCheat.toggleCheatMode(false);
            }*/
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(col);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(col);
            }
        }
        else if (col.gameObject.CompareTag("Structure"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                PlaceObject(col);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(col);
            }

            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                Destroy(col.gameObject);
            }
        }
    }

    void PlaceObject(Collider coli)
    {
        coli.transform.SetParent(null); // unparent the object from controller
        Rigidbody rigidBody = coli.GetComponent<Rigidbody>();
        rigidBody.isKinematic = true; // re-enable physics on object.

        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        Debug.Log("Placed object " + coli.gameObject.name);
    }

    void ThrowObject(Collider coli)
    {
        coli.transform.SetParent(null);
        Rigidbody rb = coli.GetComponent<Rigidbody>();
        if (coli.GetComponent<Rigidbody>())
        {
            rb.isKinematic = false; // re-enable physics on object
        }

        rb.velocity = device.velocity * throwForce; // add throw force and vectors/velocities from controller
        rb.angularVelocity = device.angularVelocity;
        Debug.Log("You have thrown the " + coli.gameObject.name);
    }

    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true; // Stop physics from acting on the object
        device.TriggerHapticPulse(2000);
        Debug.Log("You have grabbed the " + coli.gameObject.name);
    }
}
