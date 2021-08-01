using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;
using CommandUndoRedo;
using UnityEngine.UI;
using Valve.VR;
using System;
using Leap.Unity.Interaction;

namespace VRPainting {
    public class VRHandController : VRController {

        public SteamVR_Action_Boolean LeftPress = SteamVR_Input.GetBooleanAction("SnapTurnLeft");

        public SteamVR_Action_Boolean RightPress = SteamVR_Input.GetBooleanAction("SnapTurnRight");

        public SteamVR_Action_Boolean UpPress = SteamVR_Input.GetBooleanAction("SnapTurnUp");

        public SteamVR_Action_Boolean menu = SteamVR_Input.GetBooleanAction("menu");

        StrokeModel strokeModel;

        SelectModel selectModel;

        MeshModel meshModel;

        private bool hit = false;      //是否碰到平面

        string HiddenLayer = "hidden_surface";

        string SelectedLayer = "selected_surface";

        string VirtualLayer = "virtual_surface";

        // Start is called before the first frame update
        void Start() {
            lineInfo = new LineInfo();
            global = GameObject.Find("Canvas").GetComponent<GlobalState>();
            selectModel = gameObject.AddComponent<SelectModel>();
            strokeModel = gameObject.AddComponent<StrokeModel>();
            meshModel = gameObject.AddComponent<MeshModel>();
        }

        void Update() {
            HandleUndoRedo();
            //MouseFollow();
            ControllerFollow();
            UpdateLineInfo();
            PointerHighlight();
            SetTransparent();

            if (global.action == ActionState.PAINT) {
                DrawOneLine();
            }
            else if (global.action == ActionState.UI) {

            }
            else if (global.action == ActionState.FREE) {

                //selectDemo.SelectAndHighlight(new Ray(transform.position, transform.forward),dist);
                //selectDemo.SelectAndHighlight(Camera.main.ScreenPointToRay(Input.mousePosition));
            }
            else if (global.action == ActionState.DELETE) {
                DeleteLine(ControllerPosition);
            }
        }

        private void SetTransparent() {
            if(menu.GetStateDown(pose.inputSource))
                selectModel.transparent = !selectModel.transparent;
            if (UpPress.GetStateDown(pose.inputSource)) {
                Debug.Log("ahhhhhhhh");
                if (hitObject != null) {
                    InteractionBehaviour IB = hitObject.GetComponent<InteractionBehaviour>();
                    ManipulationHand MH= hitObject.GetComponent<ManipulationHand>();
                    IB.enabled = !IB.enabled;
                    MH.enabled = !MH.enabled;
                }
            }
        }

        //private void OnRenderObject() {
        //    if (global.action == ActionState.DELETE) {
        //        if (hit) {
        //            showNearLine(TargetPosition);
        //        }
        //        else {
        //            showNearLine(ControllerPosition);
        //        }
        //    }

        //}

        void showNearLine(Vector3 position) {
            Collider[] colliders = Physics.OverlapSphere(ControllerPosition, deleteDistance, LayerMask.GetMask("line"));
            if (colliders.Length > 0) {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                mat.SetColor("_Color", Color.red);
                mat.SetPass(0);
                Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(colliders[0].GetComponent<BoxCollider>().center, Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f)));
                //Graphics.DrawMeshNow(mesh, line.GetPosition(line.positionCount/2), Quaternion.identity);
            }
        }

        private bool judgePoint = true;

        GameObject DrawOneLine() {
            if (isClick()) {
                if (judgePoint)
                    FirstPoint();
                else
                    RestPoint();
            }
            if (isLoosen())
                return FinishPaint();
            return null;
        }

        private GameObject FinishPaint() {
            GameObject output;
            strokeModel.ClearCurr(out output, selectModel.Target);
            if (selectModel.Target != null)
                selectModel.ClearSelect();
            judgePoint = true;
            return output;
        }

        private void RestPoint() {
            if (selectModel.Target != null) {
                Ray controllerRay = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                bool bHit = Physics.Raycast(controllerRay, out hitInfo, dist, LayerMask.GetMask(HiddenLayer));
                if (bHit)
                    strokeModel.Draw(hitInfo.point, selectModel.Target);
            }
            else
                strokeModel.Draw(ControllerPosition);
        }

        private void FirstPoint() {
            if (hit) {
                if (hitObject.layer == LayerMask.NameToLayer(VirtualLayer)) {
                    GameObject.Find("Draw Surface").GetComponent<DrawSurface>().FinishSurface();
                }
                SelectAndHighlight();
                
                strokeModel.Draw(TargetPosition, selectModel.Target);
            }
            else
                strokeModel.Draw(ControllerPosition);
            judgePoint = false;
        }

        public void Delete() {
            if (hit)
                DeleteLine(TargetPosition);
            else
                DeleteLine(ControllerPosition);
        }



        public void DeleteLine(Vector3 position) {
            if (singleClick()) {
                Collider[] colliders = Physics.OverlapSphere(position, deleteDistance, LayerMask.GetMask("line"));
                if (colliders.Length > 0) {
                    UndoRedoManager.Insert(new DeleteCommand(colliders[0].gameObject));
                    colliders[0].gameObject.SetActive(false);
                }
            }

        }



        void SelectAndHighlight() {
            if (hitObject != null && selectModel.Target == null) {
                selectModel.SetTarget(hitObject);
                selectModel.SetMaterial();

            }

        }


        void PointerHighlight() {
            selectModel.SetMaterial();
            if (hitObject != null) {
                selectModel.HighlightGameObject(hitObject);
            }
        }


        void HandleUndoRedo() {
            if (maxUndoStored != UndoRedoManager.maxUndoStored) { UndoRedoManager.maxUndoStored = maxUndoStored; }

            if (LeftPress.GetStateDown(pose.inputSource)) {
                UndoRedoManager.Undo();
            }
            if (RightPress.GetStateDown(pose.inputSource)) {
                UndoRedoManager.Redo();
            }
        }




        void UpdateLineInfo() {
            lineInfo.width = slider.value;
            lineInfo.color = colorBar.color;
        }

        void ControllerFollow() {
            ControllerPosition = transform.position + transform.forward * dist;
            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit, dist,LayerMask.GetMask(HiddenLayer,VirtualLayer));
            if (bHit) {
                hitObject = hit.transform.gameObject;
                TargetPosition = hit.point;
                this.hit = true;
            }
            else {
                hitObject = null;
                //TargetPosition = transform.position+transform.forward*dist;
                this.hit = false;
            }
        }

        void MouseFollow() {

            //获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；

            screenPosition = Camera.main.WorldToScreenPoint(transform.position);

            //获取鼠标在场景中坐标

            mousePositionOnScreen = Input.mousePosition;


            Ray ray = Camera.main.ScreenPointToRay(mousePositionOnScreen);
            RaycastHit hit;//

            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask(SelectedLayer))) {
                mousePositionInWorld = hit.point;
                this.hit = true;
            }
            else {
                this.hit = false;
            }

        }


        bool isClick() {
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


        bool singleClick() {
            if (useVR)
                return InteractUI.GetStateDown(pose.inputSource);
            else
                return Input.GetMouseButtonDown(0);
        }


        bool isLoosen() {
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

        bool Loosen() {
            if (useVR)
                return InteractUI.GetStateUp(pose.inputSource);
            else
                return Input.GetMouseButtonUp(0);
        }
    }
}