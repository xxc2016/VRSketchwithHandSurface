using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelObject : MonoBehaviour
{
    // √Ê∞ÂGameObject
    public GameObject panelGO;

    public UnityEvent onCreate;

    public UnityEvent onResume;

    public UnityEvent onPause;

    public UnityEvent onExit;

    public void OnCreate()
    {
        onCreate.Invoke();
    }

    public void OnPause()
    {
        onPause.Invoke();
    }

    public void OnResume()
    {
        onResume.Invoke();
    }

    public void OnExit()
    {
        onExit.Invoke();
    }


}
