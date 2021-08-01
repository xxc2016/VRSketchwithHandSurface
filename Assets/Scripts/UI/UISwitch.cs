using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISwitch : SwitchInterface
{
    // Start is called before the first frame update
    void Start()
    {
        if (childrens.Count == 0)
        {
            InitPanel();
        }
        curr = 0;
        SetBtnOutline();
    }

    private void OnEnable() {
        SetBtnOutline();
    }


    public override void EnterEvent()
    {
        childrens[curr].GetComponent<EventTripple>()?.Activate();
    }

    public override void EscEvent()
    {
        ClearOutline();
        GameObject go = GameObject.Find("MainPanel");
        SwitchInterface ps = go.GetComponent<SwitchInterface>();
        ps.enabled = true;
        enabled = false;
    }
}
