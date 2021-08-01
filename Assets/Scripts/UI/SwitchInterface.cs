using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SwitchInterface : MonoBehaviour
{
    public List<GameObject> childrens;

    public int curr; // 当前选中的对象序号

    public SteamVR_Behaviour_Pose pose;

    public SteamVR_Action_Boolean Menu = SteamVR_Input.GetBooleanAction("Menu");

    public SteamVR_Action_Boolean LeftPress = SteamVR_Input.GetBooleanAction("SnapTurnLeft");

    public SteamVR_Action_Boolean RightPress = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public SteamVR_Action_Boolean UpPress = SteamVR_Input.GetBooleanAction("SnapTurnUp");

    public SteamVR_Action_Boolean DownPress = SteamVR_Input.GetBooleanAction("SnapTurnDown");

    public SteamVR_Action_Boolean InteractUI = SteamVR_Input.GetBooleanAction("InteractUI");

    //// Update is called once per frame
    void Update()
    {
        if (RightPress.GetStateDown(pose.inputSource))
            RightEvent();
        if (LeftPress.GetStateDown(pose.inputSource))
            LeftEvent();
        if (UpPress.GetStateDown(pose.inputSource))
            UpEvent();
        if (DownPress.GetStateDown(pose.inputSource))
            DownEvent();
        if (InteractUI.GetStateDown(pose.inputSource))
            EnterEvent();
        if (Menu.GetStateDown(pose.inputSource))
            EscEvent();
    }

    public void InitPanel()
    {
        childrens.Clear();
        foreach (Transform child in transform)
            childrens.Add(child.gameObject);
        if (childrens.Count > 0)
            curr = 0;
    }


    protected void NextBtn()
    {
        if (childrens.Count == 0) return;
        curr = curr + 1 == childrens.Count ? 0 : curr + 1;
    }

    protected void LastBtn()
    {
        if (childrens.Count == 0) return;
        curr = curr == 0 ? childrens.Count - 1 : curr - 1;
    }


    protected void SetBtnOutline()
    {
        ClearOutline();
        if (childrens.Count > 0)
            AddOutline(childrens[curr]);
    }

    protected void ClearOutline()
    {
        for (int i = 0; i < childrens.Count; i++)
        {
            Outline outline = childrens[i].GetComponent<Outline>();
            if (outline != null)
                Destroy(outline);
        }
    }


    protected void AddOutline(GameObject go)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
    }

    public virtual void UpEvent() {
        LastBtn();
        SetBtnOutline();
    }

    public virtual void DownEvent() {
        NextBtn();
        SetBtnOutline();
    }

    public virtual void LeftEvent() {
        LastBtn();
        SetBtnOutline();
    }

    public virtual void RightEvent() {
        NextBtn();
        SetBtnOutline();
    }

    public virtual void EnterEvent() { }

    public virtual void EscEvent() { }
}
