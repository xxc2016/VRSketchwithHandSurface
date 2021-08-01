using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommandUndoRedo;

namespace VRPainting
{
    public class StrokeModel : MonoBehaviour
    {
        public bool simplify = false;

        public float tolerance = 0.05f;

        public float distance2point = 0.01f;

        LineInfo lineInfo;


        private void Start()
        {
            lineInfo = GetComponent<VRController>().lineInfo;
            //slider.onValueChanged.AddListener(onWidthSliderChange);
        }


        // 当前的line
        Vector3 startp;
        GameObject curr;


        //private bool finish = false; //画完一笔
        //private bool start = false;  //正在绘制

        public GameObject Draw(Vector3 position,GameObject plane=null)
        {
            PaintingMode mode = GameObject.Find("Canvas").GetComponent<GlobalState>().mode;
            if (curr == null)
            {
                GameObject point;
                LineRenderer line;
                startp = position;
                point = new GameObject();
                line = point.AddComponent<LineRenderer>();
                InitLineRenderer(line);
                line.positionCount = 1;
                line.SetPosition(0, position);
                curr = point;
                return curr;
            }
            switch (mode)
            {
                case PaintingMode.FREEMODE:
                    DrawLine(curr,position);
                    break;
                case PaintingMode.STRAIGHTMODE:
                    DrawStraightLine(curr,startp,position,plane);
                    break;
                case PaintingMode.CIRCLEMODE:
                    DrawCircle(curr, startp,position,plane);
                    break;
                
            }
            return curr;

        }



        // 画点的时间间隔
        public float interval = 0.01f;
        private float actualInterval = 0;

        void DrawLine(GameObject curr,Vector3 position)
        {
            actualInterval += Time.deltaTime;
            if (actualInterval >= interval)
            {
                LineRenderer line = curr.GetComponent<LineRenderer>()??curr.AddComponent<LineRenderer>();
                AddPoint(line,position);
                actualInterval = 0;
            }
        }

        void AddPoint(LineRenderer line,Vector3 position)
        {
            if (line.positionCount == 0)
            {
                line.positionCount = 1;
                line.SetPosition(0, position);
            }
            else
            {
                if (Vector3.Distance(position, line.GetPosition(line.positionCount - 1)) > distance2point)
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, position);
                }
            }
                        
        }



        void DrawStraightLine(GameObject curr,Vector3 startp, Vector3 position,GameObject plane)
        {
            if (plane == null)
            {
                LineRenderer line = curr.GetComponent<LineRenderer>() ?? curr.AddComponent<LineRenderer>();
                line.positionCount = 3;
                line.SetPosition(0, startp);
                line.SetPosition(1, (position + startp) / 2);
                line.SetPosition(2, position);
            }
            else
            {
                LineRenderer lineRenderer = curr.GetComponent<LineRenderer>() ?? curr.AddComponent<LineRenderer>();
                lineRenderer.positionCount = 0;
                Vector3 endPoint = position;
                Camera VRcamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                float distance = Vector3.Distance(startp, endPoint)* 32;
                for (int i = 0; i < distance; i++)
                {
                    Vector3 tmpPoint = Vector3.Lerp(startp, endPoint, i / distance);
                    Vector3 tmpPointInWorld = Vector3.zero;
                    //Ray ray = VRcamera.ScreenPointToRay(tmpPoint);
                    Ray ray = new Ray(VRcamera.transform.position, (tmpPoint - VRcamera.transform.position).normalized);
                    RaycastHit hit;//
                    if (Physics.Raycast(ray, out hit, 1000 , 1 << LayerMask.NameToLayer("hidden_surface")))
                    {
                        tmpPointInWorld = hit.point;
                        Debug.Log(tmpPointInWorld);
                        lineRenderer.positionCount = lineRenderer.positionCount + 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, tmpPointInWorld);
                    }
                }
            }
        }


        void DrawCircle(GameObject curr,Vector3 starp, Vector3 position,GameObject plane)
        {
            LineRenderer line = curr.GetComponent<LineRenderer>() ?? curr.AddComponent<LineRenderer>();
            if (plane == null)
            {
                DrawCircle(line, starp, position);
            }
            else
            {
                DrawCircleOnPlane(line, starp, position,plane);
            }
        }


        void DrawCircle(LineRenderer line,Vector3 position1,Vector3 position2)
        {
            if (Vector3.Distance(position1, position2) < 0.1)
            {
                return;
            }
            Vector3 startp = position1;
            Vector3 endp = position2;
            float radius = Vector3.Distance(startp, endp) / 2;
            Vector3 circleCentre = (startp + endp) / 2;
            line.positionCount = 33;
            for (int i = 0; i < line.positionCount; i++)
            {
                double angle =2* i * Math.PI / 16;
                float x = (float)(radius * Math.Cos(angle));
                float y = (float)(radius * Math.Sin(angle));
                Vector3 point = new Vector3(x, y,0);
                Vector3 rotateAngle = new Vector3(startp.x - circleCentre.x, startp.y - circleCentre.y, 0);
                Quaternion q = Quaternion.AngleAxis(Vector3.Angle(rotateAngle, new Vector3(-1, 0, 0)), new Vector3(0, 1,0));
                line.SetPosition(i, q*point+circleCentre);
            }
            line.loop = true;
        }


        void DrawCircleOnPlane(LineRenderer line,Vector3 position1,Vector3 position2,GameObject plane)
        {
            Camera VRcamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Vector3 screenPosi1 = VRcamera.WorldToScreenPoint(position1);
            Vector3 screenPosi2 = VRcamera.WorldToScreenPoint(position2);
            line.positionCount = 0;
            Vector3 startPoint = screenPosi1;
            Vector3 endPoint = screenPosi2;
            float distance = Vector3.Distance(startPoint, endPoint);
            float radius = distance / 2;
            double pi = Math.PI;
            Vector3 circleCentre = Vector3.Lerp(startPoint, endPoint, 0.5f);
            double perimeter = pi * distance/5;
            for (int i = 0; i < perimeter; i++)
            {
                double angle = 2 * pi * i / perimeter;
                float x = (float)(circleCentre.x - radius * Math.Cos(angle));
                float y = (float)(circleCentre.y + radius * Math.Sin(angle));
                Vector3 tmpPoint = new Vector3(x, y, circleCentre.z);
                Vector3 tmpPointInWorld = Vector3.zero;

                Ray ray = VRcamera.ScreenPointToRay(tmpPoint);
                RaycastHit hit;//
                if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("hidden_surface")))
                {
                    tmpPointInWorld = hit.point;
                    line.positionCount = line.positionCount + 1;
                    line.SetPosition(line.positionCount - 1, tmpPointInWorld);
                }
            }
            line.loop = true;
        }




        // 曲线粗细与绘画速度适应
        void FitAnimationCurve()
        {
            LineRenderer lineRenderer = curr.GetComponent<LineRenderer>();
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            AnimationCurve animationCurve = SetLineWidthCurve(positions);
            if (animationCurve != null)
            {
                lineRenderer.widthCurve = animationCurve;
            }
        }

        // 速度采样的切片大小
        public int sliceCount = 5;
        // 创建粗细曲线
        AnimationCurve SetLineWidthCurve(Vector3[] positions)
        {
            if (positions.Length <= sliceCount)
            {
                return null;
            }
            Keyframe[] ks = new Keyframe[positions.Length/sliceCount];
            ks[0] = new Keyframe(0,lineInfo.width);
            for (int i = 1; i < positions.Length / sliceCount; i++)
            {
                float widthFrame = lineInfo.width*(Math.Min(2,0.5f/ (Vector3.Distance(positions[sliceCount * i - 1], positions[sliceCount * i]) + Vector3.Distance(positions[sliceCount * i + 1], positions[sliceCount * i]))));
                ks[i] = new Keyframe((float)i / ks.Length, widthFrame);
            }
            
            //ks[0].inTangent = 0;
            return new AnimationCurve(ks);
        }


        


        public void ClearCurr(out GameObject output,GameObject plane)
        {
            
            PaintingMode mode = GameObject.Find("Canvas").GetComponent<GlobalState>().mode;
            output = curr;
            if (output == null) {
                return;
            }
            curr = null;
            LineRenderer line = output.GetComponent<LineRenderer>();
            if (line==null||line.positionCount <= 1)
            {
                Destroy(output);
                return;
            }
            if (mode==PaintingMode.FREEMODE && simplify)
            {
                line.Simplify(tolerance);
            }
            if (plane != null) {
                output.transform.SetParent(plane.transform);
            }
            else {
                output.transform.SetParent(GameObject.Find("Default").transform);
            }

            output.layer = LayerMask.NameToLayer("line");

            UndoRedoManager.Insert(new DrawCommand(output));
            if (!ObjectMananger.AddObject(output))
            {
                Destroy(output);
            }
        }


        void InitLineRenderer(LineRenderer line,LineInfo lineInfo)
        {
            line.useWorldSpace = false;
            line.startWidth = lineInfo.width;
            line.endWidth = lineInfo.width;
            // 设置材质
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = lineInfo.color;
            line.endColor = lineInfo.color;
        }


        private void InitLineRenderer(LineRenderer lineRenderer)
        {
            InitLineRenderer(lineRenderer, this.lineInfo);
        }


    }

}