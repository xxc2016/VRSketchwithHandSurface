using RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using VRPainting;

public class CanvasHelper : MonoBehaviour
{
    public SteamVR_Behaviour_Pose pose;

    public SteamVR_Action_Boolean Menu = SteamVR_Input.GetBooleanAction("Menu");

    public SteamVR_Action_Boolean LeftBtn = SteamVR_Input.GetBooleanAction("SideBtn");

    //public SteamVR_Action_Boolean RightBtn = SteamVR_Input.GetBooleanAction("Menu");
    // Update is called once per frame
    void Update()
    {
        if (Menu.GetStateDown(pose.inputSource)) {
            GameObject MainPanel = transform.Find("MainPanel").gameObject;
            if (!MainPanel.activeSelf) {
                MainPanel.SetActive(true);
                transform.position = GameObject.Find("Camera").transform.position + transform.forward;
                transform.rotation = GameObject.Find("Camera").transform.rotation;
                GameObject controller = GameObject.Find("Controller (right)");
                controller.GetComponent<VRPaintController>().enabled = false;
                //ActionState lastAction = GetComponent<GlobalState>().action;
                //MainPanel.GetComponent<PanelSwitch>().SetLastAction(lastAction);
                //GetComponent<GlobalState>().SetAction(ActionState.UI);
            }
        }
        if (LeftBtn.GetStateDown(pose.inputSource))
        {
            GlobalState gs = GetComponent<GlobalState>();
            if(gs.action == ActionState.PAINT || (gs.action == ActionState.SURFACE&&(gs.surfaceMode == SurfaceMode.ONESTROKE||gs.surfaceMode==SurfaceMode.ROTATE)))
            {
                gs.nextMode();
            }
            else if(gs.action == ActionState.GIZMOS)
            {
                TransformGizmo tg = GameObject.Find("Camera").GetComponent<TransformGizmo>();
                tg.nextType();
            }
        }
        //if (RightBtn.GetStateDown(pose.inputSource))
        //{
        //    GlobalState gs = GetComponent<GlobalState>();
        //    if (gs.action == ActionState.PAINT || gs.action == ActionState.SURFACE)
        //    {
        //        gs.lastMode();
        //    }
        //    else if (gs.action == ActionState.GIZMOS)
        //    {
        //        TransformGizmo tg = GameObject.Find("Camera").GetComponent<TransformGizmo>();
        //        tg.lastType();
        //    }
        //}
    }
}
