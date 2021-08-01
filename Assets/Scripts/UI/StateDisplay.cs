using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRPainting
{
    public class StateDisplay : MonoBehaviour
    {
        Text text;

        GlobalState global;
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<Text>();
            global = GameObject.Find("Canvas").GetComponent<GlobalState>();
        }

        // Update is called once per frame
        void Update()
        {
            float fps = 1f / Time.deltaTime;
            string action = global.action.ToString();
            string mode = global.mode.ToString();
            string surfaceMode = global.surfaceMode.ToString();
            text.text = $"当前操作:{action}\n绘图模式:{mode}\n表面模式:{surfaceMode}\nFPS:{fps.ToString("f2")}";
        }
    }

}