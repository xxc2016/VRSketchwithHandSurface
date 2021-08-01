using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;

namespace VRPainting
{
    public class GlobalState : MonoBehaviour
    {
        public GameObject canvas;

        public PaintingMode mode = PaintingMode.FREEMODE;

        public ActionState action = ActionState.FREE;

        public SurfaceMode surfaceMode = SurfaceMode.ONESTROKE;

        private GlobalState() { }

        private void Start()
        {
            canvas = GameObject.Find("Canvas");
        }

        public void changeState(ActionState newState)
        {
            action = newState;
        }

        public void onClickPaintBtn()
        {
            action = action==ActionState.PAINT?ActionState.FREE:ActionState.PAINT;       
        }

        public void onClickGizmosBtn()
        {
            action = action == ActionState.GIZMOS ? ActionState.FREE : ActionState.GIZMOS;
        }

        private ActionState lastAction;

        //public void onClickUIBtn()
        //{
        //    if (action == ActionState.UI)
        //    {
        //        action = lastAction;
        //    }
        //    else
        //    {
        //        lastAction = action;
        //        action = ActionState.UI;
        //    }
        //    //canvas.transform.Find("SettingPanel").gameObject.SetActive(_action == ActionState.UI);
        //}

        public void onClickCreateSurfaceBtn()
        {
            action = action == ActionState.SURFACE ? ActionState.FREE : ActionState.SURFACE;
        }

        public void onClickDeleteBtn()
        {
            action = action == ActionState.DELETE ? ActionState.FREE : ActionState.DELETE;
        }


        public void onModeBtnClick(string modeStr)
        {
            PaintingMode currMode = PaintingMode.FREEMODE;
            switch (modeStr)
            {
                case "free":
                    currMode = PaintingMode.FREEMODE;
                    break;
                case "straight":
                    currMode = PaintingMode.STRAIGHTMODE;
                    break;
                case "circle":
                    currMode = PaintingMode.CIRCLEMODE;
                    break;
            }
            mode = currMode;
        }

        public void onSurfaceModeBtnClick(string modeStr)
        {
            SurfaceMode currMode = SurfaceMode.ONESTROKE;
            switch (modeStr)
            {
                case "onestroke":
                    currMode = SurfaceMode.ONESTROKE;
                    break;
                case "sphere":
                    currMode = SurfaceMode.SPHERE;
                    break;
                case "plane":
                    currMode = SurfaceMode.PLANE;
                    break;
                case "rotate":
                    currMode = SurfaceMode.ROTATE;
                    break;
            }
            action = ActionState.SURFACE;
            surfaceMode = currMode;
        }

        public void SetAction(ActionState newAction) {
            action = newAction;
        }

        private List<PaintingMode> ModeList = new List<PaintingMode>() { PaintingMode.FREEMODE, PaintingMode.STRAIGHTMODE, PaintingMode.CIRCLEMODE };

        private int currModeIndex = 0;

        public void nextMode()
        {
            currModeIndex = ModeList.IndexOf(mode);
            int nextIndex = currModeIndex +  1 >= ModeList.Count ? 0 : currModeIndex + 1;
            mode = ModeList[nextIndex];
        }

        public void lastMode()
        {
            currModeIndex = ModeList.IndexOf(mode);
            int lastIndex = currModeIndex - 1 < 0 ? ModeList.Count - 1 : currModeIndex - 1;
            mode = ModeList[lastIndex];
        }
    }
}
