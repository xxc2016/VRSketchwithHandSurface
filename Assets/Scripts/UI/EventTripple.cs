using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventTripple:MonoBehaviour
{
    public UnityEvent ActivateEvent;


    public void Activate()
    {
        ActivateEvent.Invoke();
    }


}
