using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Space;

public class myTest : MonoBehaviour
{
    // Transform transform;
    public LeapXRServiceProvider provider;
    Controller controller = new Controller();
    LineRenderer line;

    List<Vector3> thumbJoints = new List<Vector3>();
    List<Vector3> indexJoints = new List<Vector3>();
    List<Vector3> otherJoints = new List<Vector3>();
    Vector3 thumbDir = new Vector3();
    Vector3 indexDir = new Vector3();
    Vector3 otherDir = new Vector3();
    Vector3 palmDir = new Vector3();

    protected float handForwardDegree = 30;
    protected float deltaCloseFinger = 0.05f;

    List<List<string>> logData = new List<List<string>>();
    void Start()
    {
        logData.Add(new List<string>(){"thumbDir","indexDir","otherDir","palmDir","tiAngle",
        "toAngle","ioAngle","tpAngle","ipAngle","opAngle","type"});
    }

    void Update()
    {
        Frame source = controller.Frame();
        Frame dest = new Frame();
        // dest.CopyFrom(source).Transform(transform.GetLeapMatrix());//desktop
        provider.transformFrame(source,dest);
        // dest.CopyFrom(source).Transform(UnityMatrixExtension.GetLeapMatrix(Hands.Provider.transform));
        foreach (var hand in dest.Hands)
        {
            // hand.Transform(GetLeapTransform(Vector3.zero, Quaternion.identity));
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
            foreach (var item in others)
            {
                otherDir += item.Direction.ToVector3();
            }
            otherDir/=3.0f;
            foreach (Bone.BoneType boneType in (Bone.BoneType[]) Enum.GetValues(typeof(Bone.BoneType)))
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
                    otherJoints.Add(avg/3.0f);           
                }
                catch (System.IndexOutOfRangeException){};
            }
            Draw2 ();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(RecognitionHand.recognizeHand(logAdd("PING"))); 
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            logAdd("QU");             
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            logAdd("ZHU");             
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            logAdd("ZHUI");             
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            logAdd("QIU");             
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            Debug.Log("save start");
            RecordCsv.Write(RecordCsv.path, logData);
            Debug.Log("save finish");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            Debug.Log("test start");
            RecordCsv.Write(RecordCsv.testpath, logData);
            Debug.Log("test finish");
        }
    }

    List<String> logAdd(string str){
        // logData.Add(new List<String>(){thumbDir.ToString(),indexDir.ToString(),otherDir.ToString(),palmDir.ToString(),
        logData.Add(new List<String>(){
                ""+Vector3.Angle (thumbDir, indexDir),""+Vector3.Angle (thumbDir, otherDir),""+Vector3.Angle (indexDir, otherDir),
                ""+Vector3.Angle (thumbDir, palmDir),""+Vector3.Angle (indexDir, palmDir),""+Vector3.Angle (otherDir, palmDir),str});
        Debug.Log(str+logData.Count);
        return logData[logData.Count-1];
    }

    void Draw()
    {
        if(line==null)
            line = this.gameObject.AddComponent<LineRenderer>();
        //只有设置了材质 setColor才有作用
        // line.material = new Material(Shader.Find("Particles/Additive"));

        //line.SetVertexCount(thumbJoints.Count);//设置两点
        line.positionCount = thumbJoints.Count;
        //line.SetColors(Color.yellow, Color.red); //设置直线颜色
        line.startColor = Color.yellow;
        line.endColor = Color.red;
        //line.SetWidth(0.01f, 0.01f);//设置直线宽度
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.useWorldSpace = true;

        //设置指示线的起点和终点
        for (int i = 0; i < thumbJoints.Count; i++)
        {
            line.SetPosition(i, thumbJoints[i]);
        }
                  
    }
    void Draw2()
    {
        //设置指示线的起点和终点
        for (int i = 1; i < thumbJoints.Count; i++)
        {
            Debug.DrawLine (thumbJoints[i-1],thumbJoints[i],Color.red);
        }
        for (int i = 1; i < indexJoints.Count; i++)
        {
            Debug.DrawLine (indexJoints[i-1],indexJoints[i],Color.red);
        } 
        for (int i = 1; i < otherJoints.Count; i++)
        {
            Debug.DrawLine (otherJoints[i-1],otherJoints[i],Color.red);
        }                    
    }  

    protected bool isSameDirection (Vector3 a, Vector3 b)
    {
        //判断两个向量是否 相同 方向
        return Vector3.Angle (a, b) < handForwardDegree;
    }

    protected bool isOppositeDirection (Vector3 a, Vector3 b)    
    {
        //判断两个向量是否 相反 方向
        return Vector3.Angle (a, b) > (180 - handForwardDegree);
    }
    protected bool isPalmNormalSameDirectionWith (Hand hand, Vector3 dir)
    {
        //判断手的掌心方向于一个  向量   是否方向相同 
        return isSameDirection (hand.PalmNormal.ToVector3(), dir);
    }  
    protected bool isCloseHand (Hand hand)     //是否握拳 
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++) { //循环遍历所有的手
            Finger finger = listOfFingers [f];    
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger)  
            {  
                count++;
                //if (finger.Type == Finger.FingerType.TYPE_THUMB)
                //Debug.Log ((finger.TipPosition - hand.PalmPosition).Magnitude);
            }
        }
        return (count == 5);
    } 
    protected bool isOpenFullHand (Hand hand)         //手掌全展开
    {
        //Debug.Log (hand.GrabStrength + " " + hand.PalmVelocity + " " + hand.PalmVelocity.Magnitude);
        return hand.GrabStrength == 0;
    }
    
}
