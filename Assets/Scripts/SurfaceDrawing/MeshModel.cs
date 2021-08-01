using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NURBS;
using Unity.Collections;

namespace VRPainting
{
    public class MeshModel:MonoBehaviour
    {
        public GameObject line1go;

        public GameObject line2go;

        private GameObject subline;

        private GameObject subline2;

        public float rotateTime = 0;

        public float rotateSpeed = 120;

        public GameObject surface;

        float tolerance = 0.05f;

        private static MeshModel instance;

        public MeshModel() { }
        public static MeshModel getInstance()
        {
            if (instance == null)
                instance = new MeshModel();
            return instance;
        }

        public GameObject CreateSurface()
        {
            return CreateSurface(line1go.GetComponent<LineRenderer>(), line2go.GetComponent<LineRenderer>());
        }

        public void RotateSubline()
        {
            LineRenderer line1 = line1go.GetComponent<LineRenderer>();
            if (subline == null)
            {
                InitSubline(out subline,line1.GetPosition(0));
                InitSubline(out subline2, line1.GetPosition(line1.positionCount-1));
            }
            LineRenderer line2 = line2go.GetComponent<LineRenderer>();
            Vector3 axis = line2.GetPosition(0) - line2.GetPosition(line2.positionCount - 1);
            LineRenderer line = subline.GetComponent<LineRenderer>();
            if (rotateTime * rotateSpeed <= 360)
            {
                RotateSubline(subline.GetComponent<LineRenderer>(), axis, line2.GetPosition(0), rotateSpeed * rotateTime);
                RotateSubline(subline2.GetComponent<LineRenderer>(), axis, line2.GetPosition(0), rotateSpeed * rotateTime);
            }         
        }

        private void RotateSubline(LineRenderer subline,Vector3 axis,Vector3 center,float angle)
        {
            subline.positionCount += 1;
            subline.SetPosition(subline.positionCount - 1, RotateRound(subline.GetPosition(0), center, axis, angle));
        }

        private void InitSubline(out GameObject subline, Vector3 firstPositon)
        {
            subline = new GameObject();
            LineRenderer newline = subline.AddComponent<LineRenderer>();
            setMaterial(newline, Color.blue, 0.02f);
            newline.positionCount = 1;
            newline.SetPosition(0, firstPositon);
        }

        public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

        public GameObject CreateSurfaceByRotate()
        {
            float angle = Mathf.Min(360, rotateTime * rotateSpeed);
            LineRenderer line = line2go.GetComponent<LineRenderer>();
            Vector3 axis = line.GetPosition(0) - line.GetPosition(line.positionCount - 1);
            return CreateSurfaceByRotate(line1go.GetComponent<LineRenderer>(),line.GetPosition(0), axis, angle);
        }

        public void ClearAllLine()
        {
            if (line1go != null)
            {
                Destroy(line1go);
                line1go = null;
            }
            if (line2go != null)
            {
                Destroy(line2go);
                line2go = null;
            }
            if (subline != null)
            {
                Destroy(subline);
                Destroy(subline2);
                subline = null;
            }
            surface = null;
            rotateTime = 0;
        }

        public void ClearAndGetSurface(out GameObject output)
        {
            output = surface;
            ClearAllLine();
        }

        public bool hasBothLines()
        {
            if (line1go != null && line2go != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void addLine(GameObject newLine)
        {
            if (line1go == null)
            {
                line1go = newLine ;
                setMaterial(line1go.GetComponent<LineRenderer>(), Color.red, 0.03f);
            }
            else if(line2go == null)
            {
                line2go = newLine;
                StandardLine();
                setMaterial(line2go.GetComponent<LineRenderer>(), Color.yellow, 0.03f);
            }
        }

        private void StandardLine()
        {
            LineRenderer line2 = line2go.GetComponent<LineRenderer>();
            LineRenderer line1 = line1go.GetComponent<LineRenderer>();
            if (Vector3.Distance(line1.GetPosition(0), line2.GetPosition(0)) < 0.1f)
            {
                line2.SetPosition(0, line1.GetPosition(0));
            }
            else if (Vector3.Distance(line1.GetPosition(line1.positionCount - 1), line2.GetPosition(0)) < 0.1f)
            {
                line2.SetPosition(0, line1.GetPosition(line1.positionCount - 1));
            }
        }

        public void setMaterial(LineRenderer line,Color color,float width)
        {
            line.material= gameObject.GetComponent<VRPaintController>().SpecialLineMat;
            line.startColor = line.endColor = color;
            line.startWidth = line.endWidth = width;
        }


        public GameObject CreateSurface(LineRenderer line1,LineRenderer line2)
        {
            GameObject newSurface;
            List<CP> cpList1 = Line2CPList(line1, new Vector3());
            if (cpList1.Count > 40)
            {
                cpList1 = RamerDouglasPeucker.Reduce(cpList1, tolerance);
            }
            List<CP> cpList2 = Line2CPList(line2, new Vector3());
            if (cpList2.Count > 40)
            {
                cpList2 = RamerDouglasPeucker.Reduce(cpList2, tolerance);
            }
            Pipeline.PanAndRender(out newSurface, cpList1,cpList2,2,2,line1.loop,line2.loop);
            newSurface.AddComponent<MeshCollider>();
            newSurface.transform.position = new Vector3(0f,0f,0f);
            newSurface.transform.SetParent(GameObject.Find("Draw Surface").transform);
            newSurface.GetComponent<MeshRenderer>().material = gameObject.GetComponent<VRPaintController>().material;
            newSurface.transform.position += line1.bounds.center- newSurface.GetComponent<MeshRenderer>().bounds.center + line2.bounds.center - line2.GetPosition(0);
            surface = newSurface;
            return newSurface;
        }


        public GameObject CreateSurfaceByRotate(LineRenderer line,Vector3 center, Vector3 axis, float angle)
        {
            GameObject surface;
            List<CP> cpList = Line2CPList(line, center, true);//拉默-道格拉斯-普克算法，直线简化
            //List<CP> cpList = RamerDouglasPeucker.Reduce(Line2CPList(line,center,true), tolerance);//拉默-道格拉斯-普克算法，直线简化
            Pipeline.RotateAndRender(out surface, cpList, axis, angle, 2, 2, line.loop);//旋转角
            surface.transform.SetParent(GameObject.Find("Draw Surface").transform);
            return surface;
        }


        public GameObject CtreateSphere(Vector3 position)
        {
            if (surface == null)
            {
                surface = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            surface.transform.position = position;
            return surface;
        }

        public GameObject CtreatePlane(Vector3 position)
        {
            if (surface == null)
            {
                surface = GameObject.CreatePrimitive(PrimitiveType.Plane);
            }
            surface.transform.position = position;
            return surface;
        }

        public GameObject CreatePrimitive(Vector3 position,PrimitiveType type,float scale)
        {
            if (surface == null)
            {
                surface = GameObject.CreatePrimitive(type);
            }
            Destroy(surface.GetComponent<Collider>());
            surface.AddComponent<MeshCollider>();
            surface.transform.SetParent(GameObject.Find("surfaces").transform);
            surface.GetComponent<MeshRenderer>().material = gameObject.GetComponent<VRPaintController>().material;
            surface.transform.position = position;
            surface.transform.localScale = new Vector3(scale,scale,scale);
            return surface;
        }

        private List<CP> Line2CPList(LineRenderer line,Vector3 center,bool decentration=false)
        {
            List<CP> cpList = new List<CP>();
            for(int i = 0; i < line.positionCount; i++)
            {
                CP cp = new CP(decentration?line.GetPosition(i)-center:line.GetPosition(i), 1);
                cpList.Add(cp);
            }
            return cpList;
        }
    }
}



