using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Space;



//学长的项目的主要逻辑
public class ExtractHandLines : MonoBehaviour
{
    
    public LeapXRServiceProvider provider;
    Controller controller = new Controller();
    LineRenderer line;
    //手指的关节
    List<Vector3> thumbJoints = new List<Vector3>();
    List<Vector3> indexJoints = new List<Vector3>();
    List<Vector3> otherJoints = new List<Vector3>();
    //大拇指的方向
    Vector3 thumbDir = new Vector3();
    //食指的方向
    Vector3 indexDir = new Vector3();
    //其他手指的方向
    Vector3 otherDir = new Vector3();
    //手掌心的方向
    Vector3 palmDir = new Vector3();
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Frame source = controller.Frame();
        Frame dest = new Frame();
        // dest.CopyFrom(source).Transform(transform.GetLeapMatrix());//desktop
        provider.transformFrame(source,dest);
        foreach (var hand in dest.Hands)//左手和右手
        {
            //对各个关节点做一次清空
            thumbJoints.Clear();
            indexJoints.Clear();
            otherJoints.Clear();

            List<Finger> others = hand.Fingers;//手指们
            Finger thumb = others[0];//大拇指
            Finger index = others[1];//食指
            others.RemoveAt(0);
            others.RemoveAt(0);
            thumbDir = thumb.Direction.ToVector3();//拇指方向
            indexDir = index.Direction.ToVector3();//食指方向
            palmDir = hand.PalmNormal.ToVector3();//手掌掌心方向
            foreach (var item in others)
            {
                otherDir += item.Direction.ToVector3();
            }
            otherDir/=3.0f;//计算其他手指的平均方向
            //Enum.GetValues 检索指定类型中常数值的数组
            foreach (Bone.BoneType boneType in (Bone.BoneType[]) Enum.GetValues(typeof(Bone.BoneType)))
            {
                try
                {
                    //获取每个关节点
                    thumbJoints.Add(thumb.Bone(boneType).NextJoint.ToVector3());
                    indexJoints.Add(index.Bone(boneType).NextJoint.ToVector3());

                    Vector3 avg = new Vector3();
                    foreach (var other in others)
                    {
                        avg += other.Bone(boneType).NextJoint.ToVector3();
                    }
                    otherJoints.Add(avg/3.0f);           
                }
                catch (System.IndexOutOfRangeException){};//打印下标溢出异常
            }
            //画线
            drawByDebug ();
        }
        //当按下Z键的时候，在命令行打出是什么手势
        if (Input.GetKeyDown(KeyCode.Z))
            Debug.Log(RecognitionHand.recognizeHand(calculateHandAngles()));
    }

    //计算手的角度
    List<String> calculateHandAngles(){
        // logData.Add(new List<String>(){thumbDir.ToString(),indexDir.ToString(),otherDir.ToString(),palmDir.ToString(),
        return new List<String>(){
                ""+Vector3.Angle (thumbDir, indexDir),""+Vector3.Angle (thumbDir, otherDir),""+Vector3.Angle (indexDir, otherDir),
                ""+Vector3.Angle (thumbDir, palmDir),""+Vector3.Angle (indexDir, palmDir),""+Vector3.Angle (otherDir, palmDir)};
    }

    //在VR环境中画线
    void drawByLineRenderer()
    {
        //Hand Models
        if(line==null)
            line = this.gameObject.AddComponent<LineRenderer>();
        //只有设置了材质 setColor才有作用
        //line.material = new Material(Shader.Find("Particles/Additive"));

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

    //在命令行下画线
    void drawByDebug()
    {
        //设置指示线的起点和终点
        //拇指
        for (int i = 1; i < thumbJoints.Count; i++)
        {
            Debug.DrawLine (thumbJoints[i-1],thumbJoints[i],Color.red);
        }
        //食指
        for (int i = 1; i < indexJoints.Count; i++)
        {
            Debug.DrawLine (indexJoints[i-1],indexJoints[i],Color.red);
        }
        //其他手指
        for (int i = 1; i < otherJoints.Count; i++)
        {
            Debug.DrawLine (otherJoints[i-1],otherJoints[i],Color.red);
        }                    
    }
}
