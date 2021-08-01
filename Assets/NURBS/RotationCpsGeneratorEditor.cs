using System.IO;
using UnityEditor;
using UnityEngine;

namespace NURBS
{
    [CustomEditor(typeof(RotationCpsGenerator))]
    public class RotationCpsGeneratorEditor : Editor
    {
        private RotationCpsGenerator Generator => (RotationCpsGenerator) target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Generate Cps"))
            {
                Generator.GenerateSurfaceCpsData();
                if (!Directory.Exists(Generator.DataPath)) Directory.CreateDirectory(Generator.DataPath);
                var path = Generator.DataPath + "/" + Generator.DataName + ".asset";
                CreateOrUpdate(Generator.surfaceCpsData, path);
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