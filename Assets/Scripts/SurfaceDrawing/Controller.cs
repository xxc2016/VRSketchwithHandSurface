using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using VRPainting;

public abstract class VRController:MonoBehaviour
{
    public SteamVR_Behaviour_Pose pose;

    public SteamVR_Action_Single squeeze = SteamVR_Input.GetSingleAction("squeeze");

    public SteamVR_Action_Boolean InteractUI = SteamVR_Input.GetBooleanAction("InteractUI");

    public float deleteDistance = 0.2f;

    public GameObject canvas;

    public bool useVR = false;

    public Material material;

    public Material SpecialLineMat;

    public LineInfo lineInfo;

    public Slider slider;

    public Image colorBar;    // 颜色板

    public KeyCode ActionKey = KeyCode.LeftShift;

    public KeyCode UndoAction = KeyCode.Z;

    public KeyCode RedoAction = KeyCode.Y;

    protected GlobalState global;

    public float dist = 0.5f;

    public int maxUndoStored = 100;

    public Vector3 screenPosition;//将物体从世界坐标转换为屏幕坐标

    public Vector3 mousePositionOnScreen;//获取到点击屏幕的屏幕坐标

    public Vector3 mousePositionInWorld;//将点击屏幕的屏幕坐标转换为世界坐标

    public Vector3 ControllerPosition; //控制器的位置

    public Vector3 TargetPosition; //投影点的位置

    public GameObject hitObject; //投影的对象

    public Mesh mesh;  //

}
