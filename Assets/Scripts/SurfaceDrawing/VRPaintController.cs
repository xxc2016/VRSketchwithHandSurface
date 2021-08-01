using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;
using CommandUndoRedo;
using UnityEngine.UI;
using Valve.VR;

namespace VRPainting
{
    public class VRPaintController : VRController
    {
        StrokeModel strokeModel;

        SelectModel selectModel;

        MeshModel meshModel;

        private bool hit = false;      //是否碰到平面

        string HiddenLayer = "hidden_surface";

        string SelectedLayer = "selected_surface";

        // Start is called before the first frame update
        void Start()
        {
            lineInfo = new LineInfo();
            global = GameObject.Find("Canvas").GetComponent<GlobalState>();
            selectModel = gameObject.AddComponent<SelectModel>();
            strokeModel = gameObject.AddComponent<StrokeModel>();
            meshModel = gameObject.AddComponent<MeshModel>();
        }

        void Update()
        {
            HandleUndoRedo();
            //MouseFollow();
            ControllerFollow();
            UpdateLineInfo();
            PointerHighlight();

            if (global.action == ActionState.PAINT)
            {
                DrawOneLine();
            }
            else if (global.action == ActionState.GIZMOS)
            {
                SelectAndHighlight();
            }
            else if (global.action == ActionState.UI)
            {

            }
            else if (global.action == ActionState.SURFACE)
            {
                GameObject newSurface = CreateOneSurface();
                if (newSurface != null)
                {
                    global.changeState(ActionState.PAINT);
                    selectModel.SetTarget(newSurface);
                }
            }
            else if(global.action == ActionState.FREE)
            {
                
                //selectDemo.SelectAndHighlight(new Ray(transform.position, transform.forward),dist);
                //selectDemo.SelectAndHighlight(Camera.main.ScreenPointToRay(Input.mousePosition));
            }
            else if(global.action == ActionState.DELETE)
            {
                DeleteLine(ControllerPosition);
            }
            GameObject.Find("Camera").GetComponent<TransformGizmo>().enabled = global.action == ActionState.GIZMOS;

        }

        private void OnRenderObject()
        {
            if (global.action == ActionState.DELETE) {
                if (hit)
                {
                    showNearLine(TargetPosition);
                }
                else
                {
                    showNearLine(ControllerPosition);
                }
            }
            
        }

        void showNearLine(Vector3 position)
        {
            Collider[] colliders = Physics.OverlapSphere(ControllerPosition, deleteDistance, LayerMask.GetMask("line"));
            if (colliders.Length > 0)
            {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                mat.SetColor("_Color", Color.red);
                mat.SetPass(0);
                Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(colliders[0].GetComponent<BoxCollider>().center, Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f)));
                //Graphics.DrawMeshNow(mesh, line.GetPosition(line.positionCount/2), Quaternion.identity);
            }
        }

        private bool judgePoint = true;

        GameObject DrawOneLine()
        {
            if (isClick())
            {
                if (judgePoint)
                    FirstPoint();
                else
                    RestPoint();
            }
            if (isLoosen())
                return FinishPaint();
            return null;
        }

        private GameObject FinishPaint()
        {
            GameObject output;
            strokeModel.ClearCurr(out output,selectModel.Target);
            if (selectModel.Target != null)
                selectModel.ClearSelect();
            judgePoint = true;
            return output;
        }

        private void RestPoint()
        {
            if (selectModel.Target != null)
            {
                Ray controllerRay = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                bool bHit = Physics.Raycast(controllerRay, out hitInfo, dist * 10, LayerMask.GetMask(SelectedLayer));
                if (bHit)
                    strokeModel.Draw(hitInfo.point, selectModel.Target);
            }
            else
                strokeModel.Draw(ControllerPosition);
        }

        private void FirstPoint()
        {
            if (hit)
            {
                SelectAndHighlight();
                strokeModel.Draw(TargetPosition, selectModel.Target);
            }
            else
                strokeModel.Draw(ControllerPosition);
            judgePoint = false;
        }

        public void Delete()
        {
            if (hit)
                DeleteLine(TargetPosition);
            else
                DeleteLine(ControllerPosition);
        }



        public void DeleteLine(Vector3 position)
        {
            if (singleClick())
            {
                Collider[] colliders = Physics.OverlapSphere(position, deleteDistance, LayerMask.GetMask("line"));
                if (colliders.Length>0)
                {
                    UndoRedoManager.Insert(new DeleteCommand(colliders[0].gameObject));
                    colliders[0].gameObject.SetActive(false);
                }
            }

        }


        GameObject CreateOneSurface()
        {
            if (global.surfaceMode == SurfaceMode.ONESTROKE)
                return OneStroke();
            else if (global.surfaceMode == SurfaceMode.SPHERE)
                return CreateSurface(PrimitiveType.Sphere,1);
            else if (global.surfaceMode == SurfaceMode.PLANE)
                return CreateSurface(PrimitiveType.Plane,0.1f);
            else if(global.surfaceMode == SurfaceMode.ROTATE)
                return CreateByRotate();
            return null;
        }


        GameObject OneStroke()
        {
            if (meshModel.hasBothLines())
            {
                GameObject newSurface = meshModel.CreateSurface();
                meshModel.ClearAllLine();
                UndoRedoManager.Insert(new DrawCommand(newSurface));
                return newSurface;
            }
            GameObject newLine = DrawOneLine();
            if (newLine != null)
                meshModel.addLine(newLine);
            return null;
        }


        GameObject CreateByRotate()
        {
            if (meshModel.hasBothLines())
            {
                if (isClick())
                {
                    meshModel.rotateTime+=Time.deltaTime;
                    meshModel.RotateSubline();
                    return null;
                }
                if (Loosen()&&meshModel.rotateTime!=0)
                {
                    GameObject newSurface = meshModel.CreateSurfaceByRotate();
                    meshModel.ClearAllLine();
                    UndoRedoManager.Insert(new DrawCommand(newSurface));
                    return newSurface;
                }
            }
            GameObject newLine = DrawOneLine();
            if (newLine != null)
                meshModel.addLine(newLine);
            return null;
        }

        GameObject CreateSurface(PrimitiveType type,float size)
        {
            if (isClick())
            {
                meshModel.CreatePrimitive(ControllerPosition,type,size);
                return null;
            }
            if (Loosen())
            {
                GameObject surface;
                meshModel.ClearAndGetSurface(out surface);
                surface.transform.localScale = new Vector3(size,size,size);
                UndoRedoManager.Insert(new DrawCommand(surface));
                return surface;
            }
            return null;
        }


        void SelectAndHighlight()
        {
            if (hitObject != null && selectModel.Target==null)
            {
                selectModel.SetTarget(hitObject);
                selectModel.SetMaterial();

            }

        }

        void PointerHighlight()
        {
            selectModel.SetMaterial();
            if (hitObject!=null)
            {
                selectModel.HighlightGameObject(hitObject);
            }
        }


        void HandleUndoRedo()
        {
            if (maxUndoStored != UndoRedoManager.maxUndoStored) { UndoRedoManager.maxUndoStored = maxUndoStored; }

            if (Input.GetKey(ActionKey))
            {
                if (Input.GetKeyDown(UndoAction))
                    UndoRedoManager.Undo();
                else if (Input.GetKeyDown(RedoAction))
                    UndoRedoManager.Redo();
            }
        }

        


        void UpdateLineInfo()
        {
            lineInfo.width = slider.value;
            lineInfo.color = colorBar.color;
        }

        void ControllerFollow()
        {
            ControllerPosition = transform.position + transform.forward * dist;
            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit,dist, LayerMask.GetMask(SelectedLayer,HiddenLayer));
            if (bHit)
            {
                hitObject = hit.transform.gameObject;
                TargetPosition = hit.point;
                this.hit = true;
            }
            else
            {
                hitObject = null;
                //TargetPosition = transform.position+transform.forward*dist;
                this.hit = false;
            }
        }

        void MouseFollow()
        {

            //获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；

            screenPosition = Camera.main.WorldToScreenPoint(transform.position);

            //获取鼠标在场景中坐标

            mousePositionOnScreen = Input.mousePosition;


            Ray ray = Camera.main.ScreenPointToRay(mousePositionOnScreen);
            RaycastHit hit;//

            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask(SelectedLayer)))
            {
                mousePositionInWorld = hit.point;
                this.hit = true;
            }
            else
            {
                this.hit = false;
            }

        }


        bool isClick()
        {
            if (useVR)
                if (squeeze.axis > 0.3) {
                    return true;
                }
                else {
                    return false;
                }
            else
                return Input.GetMouseButton(0);
        }


        bool singleClick()
        {
            if (useVR)
                return InteractUI.GetStateDown(pose.inputSource);
            else
                return Input.GetMouseButtonDown(0);
        }


        bool isLoosen()
        {
            if (useVR)
                if (squeeze.axis < 0.1) {
                    return true;
                }
                else {
                    return false;
                }
            else
                return !Input.GetMouseButton(0);
        }

        bool Loosen()
        {
            if (useVR)
                return InteractUI.GetStateUp(pose.inputSource);
            else
                return Input.GetMouseButtonUp(0);
        }
    }   
}