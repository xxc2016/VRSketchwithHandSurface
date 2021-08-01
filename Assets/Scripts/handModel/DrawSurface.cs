using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRPainting;
using CommandUndoRedo;
using Leap;
using Leap.Unity;
using Leap.Unity.Space;
using NURBS;
using Leap.Unity.Interaction;
using static RecognitionHand;

public class DrawSurface : MonoBehaviour
{
    //虚物体
    GameObject VirtualSurface;


    public LeapXRServiceProvider provider;
    Controller controller = new Controller();
    public Material mat;  //物体的材质

    //--------------------------------------------------------------------------
    List<Vector3> thumbJoints;
    List<Vector3> indexJoints;
    List<Vector3> otherJoints;
    Vector3 palmPosition;
    Vector3 thumbDir;
    Vector3 indexDir;
    Vector3 otherDir;
    Vector3 palmDir;
    Vector3 wristPosition;

    GestureType gesture = GestureType.none;
    LineRenderer line;
    //-----------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //create();
        //------------------------------------------------------
        thumbJoints = new List<Vector3>();
        indexJoints = new List<Vector3>();
        otherJoints = new List<Vector3>();
        palmPosition = new Vector3();
        thumbDir = new Vector3();
        otherDir = new Vector3();
        palmDir = new Vector3();
        wristPosition = new Vector3();

        gesture = GestureType.none;
        line = gameObject.AddComponent<LineRenderer>();
        gameObject.GetComponent<Renderer>().enabled = false;
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
        }
        //------------------------------------------------------       
    }




    float waitTime = 0f;
    float interval = 1f;
    public bool isCollider;

    // Update is called once per frame
    void Update()
    {
        Frame source = controller.Frame();
        Frame dest = new Frame();
        provider.transformFrame(source, dest);

        

        foreach (var hand in dest.Hands)
        {
            if (hand.IsRight) {
                continue;
            }
            palmPosition = hand.PalmPosition.ToVector3();

            thumbJoints.Clear();
            indexJoints.Clear();
            otherJoints.Clear();

            List<Finger> others = hand.Fingers;
            Finger thumb = others[0];
            Finger index = others[1];
            others.RemoveAt(0);
            others.RemoveAt(0);
            thumbDir = thumb.Direction.ToVector3();
            indexDir = index.Direction.ToVector3();
            palmDir = hand.PalmNormal.ToVector3();
            wristPosition= hand.WristPosition.ToVector3();
            foreach (var item in others)
            {
                otherDir += item.Direction.ToVector3();
            }
            otherDir /= 3.0f;
            foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
            {
                try
                {
                    thumbJoints.Add(thumb.Bone(boneType).NextJoint.ToVector3());
                    indexJoints.Add(index.Bone(boneType).NextJoint.ToVector3());

                    Vector3 avg = new Vector3();
                    foreach (var other in others)
                    {
                        avg += other.Bone(boneType).NextJoint.ToVector3();
                    }
                    otherJoints.Add(avg / 3.0f);
                }
                catch (System.IndexOutOfRangeException) { };
            }
        }


        waitTime += Time.deltaTime;
        if (dest.Hands.Count == 0) {
            if (VirtualSurface != null) {
                Destroy(VirtualSurface);
            }
        }
        else {
            foreach (var hand in dest.Hands) {
                if (hand.IsLeft) {
                    if (isCollider) {
                        if (VirtualSurface != null) {
                            Destroy(VirtualSurface);
                        }
                        break;
                    }
                    if (VirtualSurface != null) {
                        SetPosition(VirtualSurface,gesture);
                    }
                    GestureType CurrGesture = RecognitionHand.recognizeHand(calculateHandAngles());
                    //Debug.Log(gesture.ToString());
                    if (waitTime > interval) {
                        if (VirtualSurface != null) {
                            Destroy(VirtualSurface);
                        }
                        VirtualSurface = CreateSurface(CurrGesture);
                        gesture = CurrGesture;
                        waitTime = 0;
                    }
                }

            }
        }
        

        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (transform.Find("Plane"))
            {
                Debug.Log("Deleting Plane");
                Destroy(transform.Find("Plane").gameObject);
            }
            if (transform.Find("Surface"))
            {
                Destroy(transform.Find("Surface").gameObject);
            }
            if (transform.Find("Cylinder"))
            {
                Destroy(transform.Find("Cylinder").gameObject);
            }
            if (transform.Find("Cone"))
            {
                Destroy(transform.Find("Cone").gameObject);
            }
            if (transform.Find("Sphere"))
            {
                Destroy(transform.Find("Sphere").gameObject);
            }
        }
    }

    void create() {
        List<CP> cpList21 = new List<CP>();
        cpList21.Add(new CP(new Vector3(-0.5f, 0, 0), 1));
        cpList21.Add(new CP(new Vector3(0.5f, 0, 0), 1));

        List<CP> cpList22 = new List<CP>();
        cpList22.Add(new CP(new Vector3(0, 0, -0.5f), 1));
        cpList22.Add(new CP(new Vector3(0, 0, 0.5f), 1));

        //GameObject cone;
        //List<CP> cpList = new List<CP>() { new CP(new Vector3(1,0,0), 1), new CP(new Vector3(1,1,0), 1) }; //去中心化
        //Vector3 axis4 = new Vector3(0,1,0);  //轴的方向向量
        //float angle4 = 360;//旋转角
        //Pipeline.RotateAndRender(out cone, cpList, axis4, angle4, 2, 2, false);

        GameObject surface;
        Pipeline.PanAndRender(out surface, cpList21, cpList22, 2, 2, false, false);
    }


    GameObject CreateSurface(GestureType gesture) {
        switch (gesture) {
            case GestureType.ping:
                //List<CP> cpList11 = new List<CP>();
                //cpList11.Add(new CP(indexJoints[0], 1));
                //cpList11.Add(new CP(indexJoints[3], 1));

                //List<CP> cpList12 = new List<CP>();
                //cpList12.Add(new CP(indexJoints[3], 1));
                //cpList12.Add(new CP(otherJoints[3], 1));

                GameObject plane;
                // Pipeline.PanAndRender(out plane, cpList11, cpList12, 2, 2, false, false);
                plane = CreatePlane();
                InitSurface(plane, "Plane", Color.grey);
                return plane;
                break;
            case GestureType.qu:
                //List<CP> cpList21 = new List<CP>();
                //foreach (Vector3 indexJoint in indexJoints) {
                //    cpList21.Add(new CP(indexJoint, 1));
                //}

                //List<CP> cpList22 = new List<CP>();
                //cpList22.Add(new CP(indexJoints[3], 1));
                //cpList22.Add(new CP(otherJoints[3], 1));

                GameObject surface;
                //Pipeline.PanAndRender(out surface, cpList21, cpList22, 2, 2, false, false);
                surface = CreateQuMian();
                InitSurface(surface, "Surface", Color.grey);
                return surface;
                break;
            case GestureType.zhu:
                GameObject cylinder;
                //List<Vector3> line31 = new List<Vector3>();
                //line31.Add(indexJoints[2]);
                //line31.Add(thumbJoints[2]);

                //List<Vector3> line32 = new List<Vector3>();
                //line32.Add(indexJoints[3]);
                //line32.Add(thumbJoints[1]);

                //Vector3 center3 = (indexJoints[2] + thumbJoints[2]) / 2;
                //Vector3 axis3 = indexJoints[2] - otherJoints[2];
                //List<CP> cpList3 = new List<CP>();
                //cpList3.Add(new CP(indexJoints[2] - center3, 1));
                //cpList3.Add(new CP(otherJoints[2] - center3, 1));
                //float angle3 = 360;
                //Pipeline.RotateAndRender(out cylinder, cpList3, axis3, angle3, 2, 2, false);//旋转角
                cylinder = CreateCylinder();
                InitSurface(cylinder, "Cone", Color.grey);
                return cylinder;
                return cylinder;
                break;
            case GestureType.zhui:
                GameObject cone;
                ////三个点确定锥面
                //Vector3 pointA = thumbJoints[2];
                //Vector3 pointB = indexJoints[2];
                //Vector3 pointC = otherJoints[1];//可以改成otherJoints[1]
                //Vector3 ABCenter = (pointA + pointB) / 2;    //AB中点

                //float angelBetween = Vector3.Angle(pointB - pointA, pointC - pointB) / 180 * Mathf.PI; //两向量夹角
                //if (angelBetween == 0) {
                //    return null;  //俩向量垂直时无法生成
                //}
                //Vector3 pointD = pointB - (Vector3.Distance(pointA, pointB) / 2) / Mathf.Cos(angelBetween) * ((pointC - pointB) / Vector3.Distance(pointB, pointC));   //锥顶坐标
                //Vector3 center4 = ABCenter;
                //List<CP> cpList = new List<CP>() { new CP(pointD - center4, 1), new CP(pointC - center4, 1) }; //去中心化
                //Vector3 axis4 = pointD - ABCenter;  //轴的方向向量
                //float angle4 = 360;//旋转角
                //Pipeline.RotateAndRender(out cone, cpList, axis4, angle4, 2, 2, false);
                cone = CreateCone();
                InitSurface(cone, "Cone", Color.grey);
                return cone;
                break;

            case GestureType.qiu:
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                InitSurface(sphere, "Sphere", Color.grey);
                sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                return sphere;
                break;
            case GestureType.none:
                return null;
                break;
            default:
                return null;
                break;
        }
    }

    void SetPosition(GameObject surface,GestureType gestureType) {

        switch (gestureType) {
            case GestureType.ping:
                surface.transform.position = 3 * (indexJoints[3] - otherJoints[3]) + 3 * (indexJoints[3] - indexJoints[0]) + palmPosition;

                surface.transform.rotation = Quaternion.LookRotation(Vector3.Cross(otherJoints[0] - indexJoints[0], -palmDir), -palmDir);

                break;
            case GestureType.qiu:
                surface.transform.position = palmPosition;
                break;
            case GestureType.qu:
                surface.transform.position = -2*(indexJoints[3] - otherJoints[3]) - (indexJoints[3] - indexJoints[0]) + palmPosition;
                surface.transform.rotation = Quaternion.LookRotation(-wristPosition+(indexJoints[0]+otherJoints[0])/2, -palmDir);
                break;
            case GestureType.zhu:
                Vector3 axis1 = Vector3.Cross(indexJoints[0] - thumbJoints[2], indexJoints[1] - thumbJoints[0]);
                surface.transform.position = palmPosition;
                surface.transform.up = axis1;
                break;
            case GestureType.zhui:
                Vector3 axis = Vector3.Cross(indexJoints[0] - thumbJoints[2], indexJoints[1] - thumbJoints[0]);
                surface.transform.position = palmPosition;
                surface.transform.up = axis;
                break;
            default:
                surface.transform.position = palmPosition;
                surface.transform.forward = palmDir;
                break;
        }
    }

    GameObject CreatePlane() {
        GameObject plane = new GameObject();
        MeshFilter mf =plane.AddComponent<MeshFilter>();
        mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/plane.asset");
        plane.AddComponent<MeshRenderer>();
        plane.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        return plane;
    }


    GameObject CreateCylinder() {
        float angle = Vector3.Angle(otherJoints[3] - otherJoints[0], wristPosition - otherJoints[0]);
        float ratio = (Mathf.Max(angle, 80) - 60) / 60;
        GameObject cylinder = new GameObject();
        MeshFilter mf = cylinder.AddComponent<MeshFilter>();
        Debug.Log(angle);
        if (angle > 160) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhu1.asset");
        }
        else if (angle > 145) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhu2.asset");
        }
        else if (angle > 130) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhu3.asset");

        }
        else {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhu4.asset");
        }
        cylinder.AddComponent<MeshRenderer>();
        cylinder.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        return cylinder;
    }


    GameObject CreateCone() {
        float angle = Vector3.Angle(otherJoints[3] - otherJoints[0], wristPosition - otherJoints[0]);
        float ratio = (Mathf.Max(angle,80) - 60) / 60;
        GameObject cone = new GameObject();
        MeshFilter mf = cone.AddComponent<MeshFilter>();
        Debug.Log(angle);
        if (angle > 160) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhui1.asset");
        }
        else if (angle > 145) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhui2.asset");
        }
        else if (angle > 130) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhui3.asset");

        }
        else {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/zhui4.asset");
        }
        cone.AddComponent<MeshRenderer>();
        cone.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
        return cone;



        
    }

    GameObject CreateQuMian() {
        GameObject cone = new GameObject();
        MeshFilter mf = cone.AddComponent<MeshFilter>();
        float angle = Vector3.Angle(indexJoints[0] - indexJoints[1], indexJoints[1] - indexJoints[3]);
        if (angle < 20) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/bakedMesh1.asset");
        }
        else if (angle < 40) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/bakedMesh2.asset");
        }
        else if (angle < 60) {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/bakedMesh3.asset");

        }
        else {
            mf.mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Resources/bakedMesh4.asset");
        }
        cone.AddComponent<MeshRenderer>();
        cone.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        return cone;
    }


    void InitSurface(GameObject surface,string name,Color color)
    {
        if (surface.GetComponent<Collider>()) {
            Destroy(surface.GetComponent<Collider>());
        }
        MeshCollider MC = surface.AddComponent<MeshCollider>();
        MC.convex = true;
        surface.layer = LayerMask.NameToLayer("virtual_surface");
        surface.name = name;
        //surface.transform.position = position;
        surface.GetComponent<MeshRenderer>().material = mat;
        surface.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g,color.b,0.3f) ;
    }

    public void FinishSurface() {
        GameObject surface = VirtualSurface;
        Rigidbody Rb = surface.AddComponent<Rigidbody>();
        Rb.useGravity = false;
        Rb.isKinematic = true;
        InteractionBehaviour IB = surface.AddComponent<InteractionBehaviour>();
        IB.manager = GameObject.Find("Interaction Manager").GetComponent<InteractionManager>();
        IB.ignoreContact = true;
        surface.AddComponent<ManipulationHand>();
        surface.transform.SetParent(GameObject.Find("Draw Surface").transform);
        surface.layer = LayerMask.NameToLayer("hidden_surface");
        VirtualSurface = null;
    }

    List<String> calculateHandAngles()
    {
        // logData.Add(new List<String>(){thumbDir.ToString(),indexDir.ToString(),otherDir.ToString(),palmDir.ToString(),
        return new List<String>(){
                ""+Vector3.Angle (thumbDir, indexDir),""+Vector3.Angle (thumbDir, otherDir),""+Vector3.Angle (indexDir, otherDir),
                ""+Vector3.Angle (thumbDir, palmDir),""+Vector3.Angle (indexDir, palmDir),""+Vector3.Angle (otherDir, palmDir)};
    }

    private List<CP> Line2CPList(LineRenderer line, Vector3 center, bool decentration = false)
    {
        List<CP> cpList = new List<CP>();
        for (int i = 0; i < line.positionCount; i++)
        {
            CP cp = new CP(decentration ? line.GetPosition(i) - center : line.GetPosition(i), 1);
            cpList.Add(cp);
        }
        return cpList;
    }

}