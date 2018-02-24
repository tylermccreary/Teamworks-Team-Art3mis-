﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;
    
    // Teleporter
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
    public LayerMask laserMask;
    public float yNudgeAmount = 0.0f; //specific to teleportAimerObject height

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        // Teleport Controls
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            laser.gameObject.SetActive(true);
            teleportAimerObject.SetActive(true);

            //set start pos of laser
            laser.SetPosition(0, gameObject.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
            {
                if(hit.transform.gameObject.layer == 8) // if layer is "walkable"
                {
                    teleportLocation = hit.point;
                    laser.SetPosition(1, teleportLocation);
                    //aimer position
                    teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
                }
                else if( hit.transform.gameObject.layer == 9) // layer is "stopTeleporter"
                {
                    laser.SetPosition(1, hit.point);
                    teleportAimerObject.transform.position = new Vector3(hit.point.x, 0.0f, hit.point.z);
                    teleportLocation = Vector3.zero;
                }
                
            }
            else
            {
                teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, transform.forward.y * 15 + transform.position.y, transform.forward.z * 15 + transform.position.z);
                RaycastHit groundRay;
                if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                {
                    teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
                    //aimer position
                    teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
                    laser.SetPosition(1, teleportAimerObject.transform.position);
                }
                else
                {
                    teleportLocation = Vector3.zero;
                }
                
            }
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (teleportLocation != Vector3.zero)
            {
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);
                player.transform.position = teleportLocation;
            }
            else
            {
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);
                player.transform.position = player.transform.position;
            }
        }
    }
}
