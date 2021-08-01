using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRPainting;

public class PanelSwitch : SwitchInterface
{
    private ActionState lastAction;
    // Start is called before the first frame update
    void Start()
    {
        if (childrens.Count == 0)
        {
            InitPanel();
        }
        curr = 0;
        SetBtnOutline();
        childrens[curr].GetComponent<PanelObject>().OnResume();
    }

    private void OnEnable() {
        SetBtnOutline();
        childrens[curr].GetComponent<PanelObject>().OnResume();
    }


    public override void RightEvent()
    {
        childrens[curr].GetComponent<PanelObject>().OnPause();
        NextBtn();
        childrens[curr].GetComponent<PanelObject>().OnResume();
        SetBtnOutline();
    }

    public override void DownEvent()
    {
        childrens[curr].GetComponent<PanelObject>().OnPause();
        NextBtn();
        childrens[curr].GetComponent<PanelObject>().OnResume();
        SetBtnOutline();
    }

    public override void LeftEvent()
    {
        childrens[curr].GetComponent<PanelObject>().OnPause();
        LastBtn();
        childrens[curr].GetComponent<PanelObject>().OnResume();
        SetBtnOutline();
    }

    public override void UpEvent()
    {
        childrens[curr].GetComponent<PanelObject>().OnPause();
        LastBtn();
        childrens[curr].GetComponent<PanelObject>().OnResume();
        SetBtnOutline();
    }

    public override void EnterEvent()
    {
        GameObject panel = childrens[curr].GetComponent<PanelObject>().panelGO;
        panel.GetComponent<SwitchInterface>().enabled = true;
        enabled = false;
    }

    public override void EscEvent()
    {
        foreach (GameObject child in childrens) {
            child.GetComponent<PanelObject>().panelGO.SetActive(false);
        }
        GameObject controller = GameObject.Find("Controller (right)");
        controller.GetComponent<VRPaintController>().enabled = true;
        this.gameObject.SetActive(false);
    }

    public void SetLastAction(ActionState action) {
        lastAction = action;
    }
}
