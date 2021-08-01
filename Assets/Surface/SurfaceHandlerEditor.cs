using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NURBS
{
    [CustomEditor(typeof(SurfaceHandler))]
    public class SurfaceHandlerEditor : Editor
    {
        protected int selectedId = -1;
        protected int orderX;
        protected int orderY;
        protected List<Vector3> segments = new List<Vector3>();
        protected SurfaceHandler handler => (SurfaceHandler) target;
        protected Vector3 hpos => handler.transform.position;
        protected SurfaceCpsData data => handler.Data;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Bake Mesh"))
            {
                if (!Directory.Exists(handler.BakePath)) Directory.CreateDirectory(handler.BakePath);
                var path = handler.BakePath + "/" + handler.BakeName + ".asset";
                CreateOrUpdate(handler.mesh, path);
            }

            if (GUILayout.Button("Pan"))
            {
                var l1 = new List<CP>
                {
                    new CP(new Vector3(0f, 0f, 0f), 1f),
                    new CP(new Vector3(0f, 1f, 0f), 1f),
                    new CP(new Vector3(0f, 2f, 0f), 1f),
                    new CP(new Vector3(0f, 3f, 0f), 1f)
                };
                var l2 = new List<CP>
                {
                    new CP(new Vector3(2f, 0f, 0f), 1f),
                    new CP(new Vector3(1.414f, 0f, 1.414f), 1f),
                    new CP(new Vector3(0f, 0f, 2f), 1f),
                    new CP(new Vector3(-1.414f, 0f, 1.414f), 1f),
                    new CP(new Vector3(-2f, 0f, 0f), 1f)
                };
                //Pipeline.PanAndRender(l1, l2, 1, 2, false, false);
            }

            if (GUILayout.Button("Rotate"))
            {
                var l = new List<CP>
                {
                    new CP(new Vector3(1f, 0f, 0f), 1f),
                    new CP(new Vector3(1f, 1f, 0f), 1f),
                    new CP(new Vector3(1f, 2f, 0f), 1f),
                    new CP(new Vector3(1f, 3f, 0f), 1f)
                };
                ;
                //Pipeline.RotateAndRender(l, new Vector3(0f, 1f, 0f), 360, 2, 2, false);
            }
        }

        public void OnSceneGUI()
        {
            if (!Application.isPlaying) return;
            if (segments.Count == 0) UpdateSegments();
            var cps = handler.Data.cps;
            if (handler.Data.orderX != orderX || handler.Data.orderY != orderY)
            {
                handler.Reset();
                orderX = data.orderX;
                orderY = data.orderY;
            }

            for (var i = 0; i < cps.Count; i++)
            {
                var cp = cps[i];
                handler.surface.UpdateCP(data.Convert(i), new CP(hpos + cp.pos, cp.weight));
            }

            for (var i = 0; i < cps.Count; i++)
            {
                var wp = handler.transform.TransformPoint(cps[i].pos);
                var sz = HandleUtility.GetHandleSize(wp) * 0.1f;
                if (Handles.Button(wp, Quaternion.identity, sz, sz, Handles.CubeHandleCap))
                {
                    selectedId = i;
                    Repaint();
                }
            }

            if (selectedId > -1)
            {
                var cp = cps[selectedId];
                var wp = handler.transform.TransformPoint(cp.pos);
                EditorGUI.BeginChangeCheck();
                var pos = Handles.DoPositionHandle(wp, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    cp.pos = handler.transform.InverseTransformPoint(pos);
                    cps[selectedId] = cp;
                    handler.UpdateMesh();
                    UpdateSegments();
                    EditorUtility.SetDirty(handler.Data);
                }
            }

            Handles.color = Color.blue;
            Handles.DrawLines(segments.ToArray());
        }

        private void UpdateSegments()
        {
            segments.Clear();
            for (var x = 0; x < data.count.x; x++)
            for (var y = 0; y < data.count.y - 1; y++)
            {
                segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                segments.Add(hpos + data.cps[data.Convert(x, y + 1)].pos);
            }

            for (var y = 0; y < data.count.y; y++)
            for (var x = 0; x < data.count.x - 1; x++)
            {
                segments.Add(hpos + data.cps[data.Convert(x, y)].pos);
                segments.Add(hpos + data.cps[data.Convert(x + 1, y)].pos);
            }
        }

        private static void CreateOrUpdate(Object newAsset, string assetPath)
        {
            var oldAsset = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (oldAsset == null)
            {
                AssetDatabase.CreateAsset(newAsset, assetPath);
            }
            else
            {
                EditorUtility.CopySerializedIfDifferent(newAsset, oldAsset);
                AssetDatabase.SaveAssets();
            }
        }
    }
}