using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace NURBS
{
    [CustomEditor(typeof(SurfaceCpsGenerator))]
    public class SurfaceCpsGeneratorEditor : Editor
    {
        protected SurfaceCpsGenerator generator => (SurfaceCpsGenerator) target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Generate Cps"))
            {
                generator.GenerateSurfaceCpsData();
                if (!Directory.Exists(generator.DataPath)) Directory.CreateDirectory(generator.DataPath);
                var path = generator.DataPath + "/" + generator.DataName + ".asset";
                CreateOrUpdate(generator.surfaceCpsData, path);
            }
        }

        void CreateOrUpdate(Object newAsset, string assetPath)
        {
            var oldAsset = AssetDatabase.LoadAssetAtPath<SurfaceCpsData>(assetPath);
            if (ReferenceEquals(oldAsset, null))
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