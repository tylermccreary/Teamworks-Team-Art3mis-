using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    public Camera vrCamera;
    public LayerMask redMask;
    public LayerMask greenMask;
    public LayerMask blueMask;

    private enum TimeEnum { Red, Blue, Green };
    private TimeEnum currentTime;

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
        currentTime = TimeEnum.Red;
        vrCamera.cullingMask = redMask;
    }
	
	// Update is called once per frame
	void Update () {
		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            ChangeTime();
        }
	}

    void ChangeTime()
    {
        if (Controller.GetAxis().x > 0)
        {
            switch (currentTime)
            {
                case TimeEnum.Red:
                    currentTime = TimeEnum.Green;
                    vrCamera.cullingMask = greenMask;
                    break;
                case TimeEnum.Green:
                    currentTime = TimeEnum.Blue;
                    vrCamera.cullingMask = blueMask;
                    break;
                case TimeEnum.Blue:
                    currentTime = TimeEnum.Red;
                    vrCamera.cullingMask = redMask;
                    break;
            }
        } else
        {
            switch (currentTime)
            {
                case TimeEnum.Red:
                    currentTime = TimeEnum.Blue;
                    vrCamera.cullingMask = blueMask;
                    break;
                case TimeEnum.Green:
                    currentTime = TimeEnum.Red;
                    vrCamera.cullingMask = redMask;
                    break;
                case TimeEnum.Blue:
                    currentTime = TimeEnum.Green;
                    vrCamera.cullingMask = greenMask;
                    break;
            }
        }
    }
}
