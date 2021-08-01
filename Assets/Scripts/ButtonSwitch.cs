using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitch : MonoBehaviour
{
    public static Stack<GameObject> stackPanel=new Stack<GameObject>();

    public List<GameObject> childrens;

    GameObject activePanel;
     
    int curr;

    // Start is called before the first frame update
    //void Start()
    //{
    //    Push("MainPanel");
    //}



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            childrens[curr].GetComponent<PanelObject>().OnPause();
            NextBtn();
            childrens[curr].GetComponent<PanelObject>().OnResume();
            SetBtnOutline();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            childrens[curr].GetComponent<PanelObject>().OnPause();
            LastBtn();
            childrens[curr].GetComponent<PanelObject>().OnResume();
            SetBtnOutline();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (childrens.Count <= 0)
            {
                return;
            }
            if (childrens[curr].GetComponent<EventTripple>() != null)
            {
                childrens[curr].GetComponent<EventTripple>().Activate();
            }
            else
            {
                //Push(childrens[curr]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activePanel == null)
            {
                return;
            }
            if (activePanel.GetComponent<EventTripple>() != null)
            {
                activePanel.GetComponent<EventTripple>().Activate();
            }
            else
            {
                BackwardPop();
            }
        }
    }


    public void ReturnHome()
    {
        ClearChildren();
        stackPanel.Clear();
        Push("MainPanel");
    }


    public void Push(string path)
    {
        GameObject parent = GameObject.Find("Canvas");
        GameObject panel = parent.transform.Find(path).gameObject;
        panel.SetActive(true);
        stackPanel.Push(panel);
        activePanel = panel;
        UpdateChildren();
    }


    public void Push(GameObject panel)
    {
        panel.SetActive(true);
        stackPanel.Push(panel);
        activePanel = panel;
        UpdateChildren();
    }

    public GameObject ExitPop()
    {
        //BackwardPop()?.SetActive(false);
        if (stackPanel.Count > 0)
        {
            GameObject lastPanel = stackPanel.Pop();
            lastPanel.SetActive(false);
            if (stackPanel.Count > 0)
            {
                activePanel = stackPanel.Peek();
                UpdateChildren();
            }
            else
            {
                activePanel = null;
                ClearChildren();
            }

            return lastPanel;
        }
        return null;
    }


    public GameObject BackwardPop()
    {
        if (stackPanel.Count > 0)
        {
            GameObject lastPanel = stackPanel.Pop();
            //lastPanel.SetActive(false);
            if (stackPanel.Count > 0)
            {
                activePanel = stackPanel.Peek();
                UpdateChildren();
            }
            else
            {
                activePanel = null;
                ClearChildren();
            }

            return lastPanel;
        }
        return null;
    }



    void NextBtn()
    {
        if (childrens.Count == 0) return;
        curr = curr + 1 == childrens.Count? 0 : curr + 1;
        //activeChildren = childrens[curr];
    }

    void LastBtn()
    {
        if (childrens.Count == 0) return;
        curr = curr == 0 ? childrens.Count-1  : curr - 1;
        //activeChildren = childrens[curr];
    }

    void ClearChildren()
    {
        ClearOutline();
        childrens.Clear();
        curr = 0;
    }


    void UpdateChildren()
    {
        ClearChildren();
        foreach (Transform child in activePanel.transform)
        {
            childrens.Add(child.gameObject);
        }
        if (childrens.Count > 0)
        {
            //activeChildren = childrens[0];
            curr = 0;
            SetBtnOutline();
        }
    }


    // Update is called once per frame
    void SetBtnOutline()
    {
        ClearOutline();

        if (childrens.Count > 0)
        {
            AddOutline(childrens[curr]);
        }
    }

    void ClearOutline()
    {
        for (int i = 0; i < childrens.Count; i++)
        {
            Outline outline = childrens[i].GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }
        }
    }


    void AddOutline(GameObject go)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
    }
}
