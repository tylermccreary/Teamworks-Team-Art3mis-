using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MyTeleport : MonoBehaviour {

    public LayerMask traceLayerMask;
    public Color pointerValidColor;
    public Color pointerInvalidColor;
    public Color pointerTeleportColor;

    public float teleportFadeTime = 0.1f;
    public float meshFadeTime = 0.2f;
    public float arcDistance = 10.0f;
    public Transform invalidReticleTransform;
    public GameObject playerHMD;
    public Transform playerTrackingOriginTransform;
    public Material teleportPointMaterial;

    private TeleportArc teleportArc = null;
   // private LineRenderer pointerLineRenderer;
    private bool visible = false;

    private bool teleporting = false;
    private bool teleport = false;
    private float currentFadeTime = 0.0f;

    private float meshAlphaPercent = 1.0f;
    private float pointerShowStartTime = 0.0f;
    private float pointerHideStartTime = 0.0f;
    private bool meshFading = false;
    private float fullTintAlpha;

    private Vector2 trackPadStart;
    


    private SteamVR_TrackedObject trackedObj;
    // 2
    private SteamVR_Controller.Device Controller
    {
        get
        { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    public Vector3 feetPositionGuess
    {
        get
        {
            Transform hmd = playerHMD.transform;
            if (hmd)
            {
                return playerTrackingOriginTransform.position + Vector3.ProjectOnPlane(hmd.position - playerTrackingOriginTransform.position, playerTrackingOriginTransform.up);
            }
            return playerTrackingOriginTransform.position;
        }
    }


    void Awake()
    {
       // pointerLineRenderer = GetComponentInChildren<LineRenderer>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        teleportArc = GetComponent<TeleportArc>();
        teleportArc.traceLayerMask = traceLayerMask;
        invalidReticleTransform.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            ShowPointer();
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            HidePointer();
            if (teleport)
            {
                Teleport();
                teleporting = true;
                teleport = false;
            }
        }



        if (visible)
        {
            UpdatePointer();

            if (meshFading)
            {
                UpdateTeleportColors();
            }
        }
    }

    private void ShowPointer()
    {
        pointerShowStartTime = Time.time;
        trackPadStart = Controller.GetAxis();


        visible = true;
        meshFading = true;

        /////////
        ////////////// use for laser pointer
       // teleportPointerObject.SetActive(true);
        teleportArc.Show();
    }

    private void HidePointer()
    {
        //if (visible)
        //{
        //pointerHideStartTime = Time.time;
        //}
        visible = false;

        teleportArc.Hide();
        invalidReticleTransform.gameObject.SetActive(false);
    }

    private void UpdatePointer()
    {
        if (Controller.GetAxis().y - trackPadStart.y > .2)
        {
            teleport = true;
        } else
        {
            teleport = false;
        }
        Vector3 pointerStart = trackedObj.transform.position;
        Vector3 pointerEnd;
        Vector3 pointerDir = trackedObj.transform.forward;
        bool hitSomething = false;
        Vector3 playerFeetOffset = playerTrackingOriginTransform.position - feetPositionGuess;

        Vector3 arcVelocity = pointerDir * arcDistance;

        TeleportMarkerBase hitTeleportMarker = null;

        //Check pointer angle
        float dotUp = Vector3.Dot(pointerDir, Vector3.up);
        float dotForward = Vector3.Dot(pointerDir, playerHMD.transform.forward);
        bool pointerAtBadAngle = false;
        if ((dotForward > 0 && dotUp > 0.75f) || (dotForward < 0.0f && dotUp > 0.5f))
        {
            pointerAtBadAngle = true;
        }

        //Trace to see if the pointer hit anything
        RaycastHit hitInfo;
        teleportArc.SetArcData(pointerStart, arcVelocity, true, pointerAtBadAngle);
        if (teleportArc.DrawArc(out hitInfo))
        {
            hitSomething = true;
            hitTeleportMarker = hitInfo.collider.GetComponentInParent<TeleportMarkerBase>();
        }

        if (pointerAtBadAngle)
        {
            hitTeleportMarker = null;
        }

        if (teleport)
        {
            teleportArc.SetColor(pointerTeleportColor);
            teleportPointMaterial.SetColor("_TintColor", pointerTeleportColor);
        }
        else
        {
            teleportArc.SetColor(pointerValidColor);
            teleportPointMaterial.SetColor("_TintColor", pointerValidColor);
        }


        /////////////////////////
        ////////////////////////
        ///////////////
        ////////////
        /////////
        ///////
        /////////////////LOOK AT teleportPointerObject for line color

        invalidReticleTransform.gameObject.SetActive(!pointerAtBadAngle);

        //Orient the invalid reticle to the normal of the trace hit point
        Vector3 normalToUse = hitInfo.normal;
        float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
        if (angle < 15.0f)
        {
            normalToUse = Vector3.up;
        }

        float distanceFromPlayer = Vector3.Distance(hitInfo.point, playerHMD.transform.position);

        if (hitSomething)
        {
            pointerEnd = hitInfo.point;
        }
        else
        {
            pointerEnd = teleportArc.GetArcPositionAtTime(teleportArc.arcDuration);
        }

        invalidReticleTransform.position = pointerEnd;

        //pointerLineRenderer.SetPosition(0, pointerStart);
       // pointerLineRenderer.SetPosition(1, pointerEnd);
    }



    private void UpdateTeleportColors()
    {
        float deltaTime = Time.time - pointerShowStartTime;
        if (deltaTime > meshFadeTime)
        {
            meshAlphaPercent = 1.0f;
            meshFading = false;
        }
        else
        {
            meshAlphaPercent = Mathf.Lerp(0.0f, 1.0f, deltaTime / meshFadeTime);
        }
    }

    private void Teleport()
    {
        Vector3 newPosition = invalidReticleTransform.transform.position;
        InitiateTeleportFade();
        playerTrackingOriginTransform.position = newPosition;
        Invoke("InitiateTeleportFadeOut", teleportFadeTime);
    }

    private void InitiateTeleportFade()
    {
        Debug.Log("Fade: " + Time.time);
        teleporting = true;

        SteamVR_Fade.Start(Color.clear, 0.0f);
        SteamVR_Fade.Start(Color.black, teleportFadeTime);
    }

    private void InitiateTeleportFadeOut()
    {
        Debug.Log("FadeOut: " + Time.time);
        teleporting = false;

        SteamVR_Fade.Start(Color.black, 0.0f);
        SteamVR_Fade.Start(Color.clear, teleportFadeTime);
    }
}
