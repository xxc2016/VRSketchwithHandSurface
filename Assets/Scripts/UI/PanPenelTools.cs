using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanPenelTools : MonoBehaviour
{
    public GameObject slider;

    public GameObject saturation;

    public GameObject hui;

    public void ActivateSlider()
    {
        slider.transform.Find("Slider").GetComponent<SliderChange>().enabled = true;
        GetComponent<SwitchInterface>().enabled = false;
    }

    public void ActivateSaturation() {
        saturation.GetComponent<PointerMove>().enabled = true;
        GetComponent<SwitchInterface>().enabled = false;
    }

    public void ActivateHui() {
        hui.GetComponent<PointerMove>().enabled = true;
        GetComponent<SwitchInterface>().enabled = false;
    }
}
