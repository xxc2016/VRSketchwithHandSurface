using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

public class ManipulationHand : MonoBehaviour
{
    public LeapXRServiceProvider provider;
    private Controller controller;
    private bool isPinched = false;
    private bool isGrabbed = false;
    private InteractionBehaviour interactionBehaviour;

    private float prevGrabDegree = 0.0f;
    private Vector3 prevPinchPosition;
    private Quaternion prevPinchQuaternion;

    const float deltaCloseFinger = 30.0f;
    const float deltaFarFinger = 50.0f;
    const float deltaCloseGrabDegree = 0.2f;

    void Start()
    {
        controller = new Controller();
        interactionBehaviour = GetComponent<InteractionBehaviour>();
        provider = GameObject.Find("Main Camera").GetComponent<LeapXRServiceProvider>();
        interactionBehaviour.OnGraspStay = () =>
        {
            //Debug.Log(interactionBehaviour.moveObjectWhenGrasped);
            Frame source = controller.Frame();
            Frame dest = new Frame();
            provider.transformFrame(source, dest);
            foreach (Hand hand in dest.Hands) {
                if (hand.IsLeft) {
                    if (GetIsMaxPinch(hand)) {
                        //interactionBehaviour.ignoreGrasping = false;
                    }
                    else {
                        interactionBehaviour.ReleaseFromGrasp();
                        interactionBehaviour.ignoreGrasping = true;
                    }
                }
            }
        };
    }
 
    
    // 开始接触
    void OnTriggerEnter(Collider collider) {
        if (!enabled) return;
        GameObject.Find("Draw Surface").GetComponent<DrawSurface>().isCollider = true;
        if (collider.tag.Equals("hand"))
            Debug.Log("开始接触");
        Frame source = controller.Frame();
        Frame dest = new Frame();
        provider.transformFrame(source, dest);
        foreach (Hand hand in dest.Hands) {
            if (hand.IsLeft) {
                prevGrabDegree = GetGrabDegree(hand);
            }
        }
    }

    // 接触结束
    void OnTriggerExit(Collider collider) {
        GameObject.Find("Draw Surface").GetComponent<DrawSurface>().isCollider = false;
        if (!enabled) return;
        if (collider.tag.Equals("hand"))
            Debug.Log("接触结束");
        isGrabbed = false;
        isPinched = false;
    }

    // 接触持续中
    void OnTriggerStay(Collider collider) {
        if (!enabled) return;
        //if (!collider.tag.Equals("hand")) return;
        Debug.Log("接触持续中");
        Frame source = controller.Frame();
        Frame dest = new Frame();
        provider.transformFrame(source, dest);
        foreach (Hand hand in dest.Hands) {
            if (hand.IsLeft) {
                //if (isPinched) {
                //    isGrabbed = false;
                //    interactionBehaviour.ignoreGrasping = false;
                //    isPinched = GetIsMaxPinch(hand);
                //}
                //else {
                //    interactionBehaviour.ReleaseFromGrasp();
                //    interactionBehaviour.ignoreGrasping = true;
                //    isPinched = GetIsMinPinch(hand);
                //    if (!isPinched && isGrabbed) {
                //        //isPinched = false;
                //        Scale(hand);
                //    }
                //    else {
                //        isGrabbed = GetIsGrab(hand);
                //    }
                //}
                if (GetIsPinch(hand)) {
                    if (isPinched) {
                        isGrabbed = false;
                        interactionBehaviour.ignoreGrasping = false;
                        isPinched = GetIsMaxPinch(hand);
                    }
                    else {
                        interactionBehaviour.ReleaseFromGrasp();
                        interactionBehaviour.ignoreGrasping = true;
                        isPinched = GetIsMinPinch(hand);
                    }
                }
                else {
                    if (isPinched) {
                        interactionBehaviour.ReleaseFromGrasp();
                        interactionBehaviour.ignoreGrasping = true;
                        isPinched = false;
                    }
                    if (isGrabbed) {
                        Scale(hand);
                    }
                    else {
                        isGrabbed = GetIsGrab(hand);
                    }
                }
            }
        }
    }

 
    /// <summary>
    /// 缩放
    /// </summary>
    public void Scale(Hand hand)
    {
        float currGrabDegree = GetGrabDegree(hand);
        if (Mathf.Abs( currGrabDegree - prevGrabDegree) <= deltaCloseGrabDegree) {
            Debug.Log(currGrabDegree - prevGrabDegree);
            return;
        }
        Vector3 value = transform.localScale;
        float rate =  0.5f * (prevGrabDegree - currGrabDegree);
        transform.localScale += new Vector3(value.x * rate, value.y * rate, value.z * rate);
        prevGrabDegree = GetGrabDegree(hand);
    }

    public bool GetIsGrab(Hand hand) {
        float indexMag = (hand.Fingers[1].TipPosition - hand.Fingers[0].TipPosition).Magnitude;
        float midMag = (hand.Fingers[2].TipPosition - hand.Fingers[0].TipPosition).Magnitude;
        //Debug.Log(midMag - indexMag);
        return hand.PinchDistance >= deltaCloseFinger && midMag >= indexMag && midMag - indexMag <= deltaCloseFinger;
    }

    public bool GetIsMinPinch(Hand hand){
        //Debug.Log(hand.PinchDistance);
        float indexMag = (hand.Fingers[1].TipPosition - hand.PalmPosition).Magnitude;
        float midMag = (hand.Fingers[2].TipPosition - hand.PalmPosition).Magnitude;
//        float ringMag = (hand.Fingers[3].TipPosition - hand.PalmPosition).Magnitude;
//        float pinkyMag = (hand.Fingers[4].TipPosition - hand.PalmPosition).Magnitude;
        return hand.PinchDistance <= deltaCloseFinger;

    }

    public bool GetIsPinch(Hand hand) {
        float indexMag = (hand.Fingers[1].TipPosition - hand.PalmPosition).Magnitude;
        float midMag = (hand.Fingers[2].TipPosition - hand.PalmPosition).Magnitude;
        //        float ringMag = (hand.Fingers[3].TipPosition - hand.PalmPosition).Magnitude;
        //        float pinkyMag = (hand.Fingers[4].TipPosition - hand.PalmPosition).Magnitude;
        return midMag <= 1.02f*indexMag;

    }

    public bool GetIsMaxPinch(Hand hand) {
        //Debug.Log(hand.PinchDistance);
        float indexMag = (hand.Fingers[1].TipPosition - hand.PalmPosition).Magnitude;
        float midMag = (hand.Fingers[2].TipPosition - hand.PalmPosition).Magnitude;
        //        float ringMag = (hand.Fingers[3].TipPosition - hand.PalmPosition).Magnitude;
        //        float pinkyMag = (hand.Fingers[4].TipPosition - hand.PalmPosition).Magnitude;
        return hand.PinchDistance <= deltaFarFinger;

    }

    public float GetGrabDegree(Hand hand){
        return hand.GrabStrength;//0-open,1-fist
    }
    
}