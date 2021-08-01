using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 供UI交互的功能
/// </summary>
public class UITools : MonoBehaviour
{
    public void SetGameObjectInActive(GameObject sender)
    {
        sender.SetActive(false);
    }

    public void SetGameObjectActive(GameObject sender)
    {
        sender.SetActive(true);
    }

}
