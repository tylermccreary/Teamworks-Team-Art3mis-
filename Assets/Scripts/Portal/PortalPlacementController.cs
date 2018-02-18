using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.StereoRendering;

public class PortalPlacementController : MonoBehaviour {

    public GameObject portalPlacerPrefab;
    public GameObject portalPrefab;
    public LayerMask groundLayerMask;

    private GameObject teleportPointerObject;

    private bool showPortalPlacer;
    private GameObject portalPlacer;
    private GameObject portal;
    private LineRenderer pointerLineRenderer;
    private StereoRenderer stereoRenderer;

    private SteamVR_TrackedObject trackedObj;
    // 2
    private SteamVR_Controller.Device Controller
    {
        get
        { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        pointerLineRenderer = GetComponentInChildren<LineRenderer>();
        teleportPointerObject = pointerLineRenderer.gameObject;
    }

    void Start()
    {
        showPortalPlacer = false;
        portalPlacer = Instantiate(portalPlacerPrefab);
    }

    // Update is called once per frame
    void Update () {
        teleportPointerObject.SetActive(true);
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            showPortalPlacer = !showPortalPlacer;
        }

        if (showPortalPlacer)
        {
            UpdatePortalPlacer();
        } else
        {
            portalPlacer.SetActive(false);
            teleportPointerObject.SetActive(false);
        }

        if (Controller.GetHairTriggerDown() && showPortalPlacer && teleportPointerObject.activeSelf)
        {
            showPortalPlacer = false;
            if (portal != null)
            {
                Destroy(portal);
            }

            portal = Instantiate(portalPrefab, portalPlacer.transform.position, portalPlacer.transform.rotation);
            stereoRenderer = portal.GetComponentInChildren<StereoRenderer>();
            stereoRenderer.canvasOriginPos = portal.transform.position;
            stereoRenderer.canvasOriginRot = portal.transform.rotation;
        }
	}

    void UpdatePortalPlacer()
    {
        Vector3 pointerStart = transform.position;
        Vector3 pointerEnd = transform.position;
        Vector3 pointerDir = transform.forward;

        Ray ray = new Ray();
        ray.origin = pointerStart;
        ray.direction = pointerDir;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                teleportPointerObject.SetActive(true);
                pointerEnd = hit.point;

                pointerLineRenderer.SetPosition(0, pointerStart);
                pointerLineRenderer.SetPosition(1, pointerEnd);

                portalPlacer.SetActive(true);
                portalPlacer.transform.position = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);
                portalPlacer.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            } else
            {
                teleportPointerObject.SetActive(false);
                portalPlacer.SetActive(false);
            }
        } else
        {
            teleportPointerObject.SetActive(false);
            portalPlacer.SetActive(false);
        }
    }
}
