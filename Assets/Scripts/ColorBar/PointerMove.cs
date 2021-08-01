using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class PointerMove : MonoBehaviour
{
    public UnityEvent ChangeColor;

    public SwitchInterface tripper;

    public SteamVR_Behaviour_Pose pose;

    public SteamVR_Action_Boolean LeftPress = SteamVR_Input.GetBooleanAction("SnapTurnLeft");

    public SteamVR_Action_Boolean RightPress = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public SteamVR_Action_Boolean UpPress = SteamVR_Input.GetBooleanAction("SnapTurnUp");

    public SteamVR_Action_Boolean DownPress = SteamVR_Input.GetBooleanAction("SnapTurnDown");

    public SteamVR_Action_Boolean MiddlePress = SteamVR_Input.GetBooleanAction("SnapTurnMiddle");

    public SteamVR_Action_Boolean Menu = SteamVR_Input.GetBooleanAction("Menu");

    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

    SteamVR_Action_Boolean isTouch = SteamVR_Input.GetBooleanAction("Touch");

    ISteamVR_Action_Vector2 TouchPosition = SteamVR_Input.GetVector2Action("TouchPosition");

    private void Awake()
    {
        //bs = GameObject.Find("Canvas").GetComponent<ButtonSwitch>();
    }
    

    // Update is called once per frame
    void Update()
    {
        RectTransform pointer = transform.GetChild(0).GetComponent<RectTransform>();
        ColorPickClick cp = GetComponent<ColorPickClick>();
        if (UpPress.GetState(pose.inputSource))
        {
            pointer.anchoredPosition += new Vector2(0,1);
        }
        if (DownPress.GetState(pose.inputSource))
        {
            pointer.anchoredPosition += new Vector2(0, -1);
        }
        if (LeftPress.GetState(pose.inputSource))
        {
            pointer.anchoredPosition += new Vector2(-1, 0);
        }
        if (RightPress.GetState(pose.inputSource))
        {
            pointer.anchoredPosition += new Vector2(1, 0);
        }
        if (Menu.GetStateDown(pose.inputSource) || interactWithUI.GetStateDown(pose.inputSource)) {
            tripper.enabled = true;
            enabled = false;
        }
        if (isTouch.GetStateDown(pose.inputSource)) {
            Vector2 posi = TouchPosition.axis;
            pointer.anchoredPosition += posi;
        }
        cp.ClickPoint = new Vector3(pointer.anchoredPosition.x, pointer.anchoredPosition.y, 0);
        ChangeColor.Invoke();
    }

    public void onColorBarClick()
    {
        //bs.Push(gameObject);
        enabled = true;
    }

    public void onColorBarExit()
    {
        //bs.BackwardPop();
        enabled = false;
    }
 
}
