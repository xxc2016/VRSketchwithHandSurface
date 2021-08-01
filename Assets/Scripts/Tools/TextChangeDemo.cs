using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRPainting
{
    public class TextChangeDemo : MonoBehaviour
    {
        public string txtContent;


        // Update is called once per frame
        void Update()
        {
            GlobalState gb = GameObject.Find("Canvas").GetComponent<GlobalState>();
            txtContent = gb.action.ToString();
            Text t = GetComponent<Text>();
            t.text = txtContent;
        }
    }
}
