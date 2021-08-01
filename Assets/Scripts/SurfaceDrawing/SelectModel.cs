using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPainting
{
    public class SelectModel : MonoBehaviour
    {
        public GameObject Target;

        public string planeName;

        public bool transparent = false;

        Material mt; //材质

        public float SELECTED = 0.6f;  // 选中的透明度

        public float MIDDLE = 0.2f;    // 鼠标移到上面透明度

        public float HIDDEN = 0.2f;    // 未选中的透明度

        string HiddenLayer = "hidden_surface";

        string SelectedLayer = "selected_surface";

        private void Start()
        {
            Physics.queriesHitBackfaces = true;
            initMaterials();
        }


        public void SetTarget(GameObject hitObject)
        {
            Target = hitObject;
            planeName = hitObject.name;
            ClearAllLayer();
            hitObject.layer = LayerMask.NameToLayer(SelectedLayer);
        }


        //public void SelectAndHighlight(Ray ray,float dist)
        //{
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(ray, out hitInfo, dist, LayerMask.GetMask(SelectedLayer, HiddenLayer)))
        //    {
        //        if (Input.GetMouseButtonDown(0))
        //        {
        //            //if (Target == hitInfo.transform.gameObject)
        //            //{
        //            //    Target = null;
        //            //    ClearAllLayer();
        //            //    SetMaterial();
        //            //    return;
        //            //}
        //            Target = hitInfo.transform.gameObject;
        //            planeName = hitInfo.transform.gameObject.name;
        //            ClearAllLayer();
        //            hitInfo.transform.gameObject.layer = LayerMask.NameToLayer(SelectedLayer);
        //        }
        //        // 设置material
        //        SetMaterial();
        //        HighlightGameObject(hitInfo.transform.gameObject);
        //    }
        //    else
        //    {
        //        SetMaterial();
        //    }
        //}

        public void HighlightGameObject(GameObject go)
        {
            Material mt =go.GetComponent<MeshRenderer>().material;
            mt.color = new Color(1, 1, 1, Target == go ? SELECTED + MIDDLE : HIDDEN + MIDDLE);
        }

        public void ClearAllLayer()
        {
            foreach (Transform child in GameObject.Find("Draw Surface").transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(HiddenLayer);
            }
        }


        public void ClearSelect()
        {
            Target = null;
            ClearAllLayer();
            SetMaterial();
        }

        public void SetMaterial()
        {
            foreach (Transform child in GameObject.Find("Draw Surface").transform)
            {
                if (child.gameObject.name=="Default")
                {
                    continue;
                }
                Material mat = child.gameObject.GetComponent<MeshRenderer>().material;
                if (transparent) {
                    mat.color = new Color(1, 1, 1, 0);
                    Target = null;
                    continue;
                }
                if (child.gameObject.layer == LayerMask.NameToLayer(HiddenLayer))
                {
                    mat.color = new Color(1, 1, 1, HIDDEN);
                }
                else if (child.gameObject.layer == LayerMask.NameToLayer(SelectedLayer))
                {
                    mat.color = new Color(1, 1, 1, SELECTED);
                }
            }
        }


        void initMaterials()
        {
            foreach (Transform child in GameObject.Find("Draw Surface").transform)
            {
                if (child.gameObject.name != "Default")
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    SetMaterialRenderingMode(mat, RenderingMode.Transparent);
                    mat.color = new Color(1, 1, 1, HIDDEN);
                    child.gameObject.GetComponent<MeshRenderer>().material = mat;
                    child.gameObject.layer = LayerMask.NameToLayer(HiddenLayer);
                }
            }
        }

        public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

    }

    
}


