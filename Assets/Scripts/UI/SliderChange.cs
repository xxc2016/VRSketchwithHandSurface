using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SliderChange : MonoBehaviour
{
    Slider slider;

    public SwitchInterface tripper;

    float speed;

    public SteamVR_Behaviour_Pose pose;

    public SteamVR_Action_Boolean LeftPress = SteamVR_Input.GetBooleanAction("SnapTurnLeft");

    public SteamVR_Action_Boolean RightPress = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public SteamVR_Action_Boolean UpPress = SteamVR_Input.GetBooleanAction("SnapTurnUp");

    public SteamVR_Action_Boolean DownPress = SteamVR_Input.GetBooleanAction("SnapTurnDown");

    public SteamVR_Action_Boolean MiddlePress = SteamVR_Input.GetBooleanAction("SnapTurnMiddle");

    public SteamVR_Action_Boolean Menu = SteamVR_Input.GetBooleanAction("Menu");

    public SteamVR_Action_Single squeeze = SteamVR_Input.GetSingleAction("squeeze");
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        speed = slider.maxValue / 200;
    }

    // Update is called once per frame
    void Update()
    {
        if (RightPress.GetState(pose.inputSource))
        {
            slider.value = slider.value + speed > slider.maxValue ? slider.maxValue : slider.value + speed;
        }

        if (LeftPress.GetState(pose.inputSource))
        {
            slider.value = slider.value - speed < slider.minValue ? slider.minValue : slider.value - speed;
        }

        if (Menu.GetStateDown(pose.inputSource))
        {
            tripper.enabled = true;
            enabled = false;
        }
    }
}
