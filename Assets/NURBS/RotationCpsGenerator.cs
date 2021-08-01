using System;
using System.Collections.Generic;
using UnityEngine;

namespace NURBS
{
    public class RotationCpsGenerator : MonoBehaviour
    {
        [SerializeField] protected List<CP> cps;
        [SerializeField] protected Vector3 axis;
        [SerializeField] protected int orderX = 3;
        [SerializeField] protected int orderY = 3;
        [SerializeField] protected bool loopX = false;
        [SerializeField] protected float rotationAngle;
        [SerializeField] protected string dataPath = "Assets/Surface/CpsData";
        [SerializeField] protected string dataName = "cpsData";
        public SurfaceCpsData surfaceCpsData { get; private set; }
        public string DataPath => dataPath;

        public string DataName => dataName;

        // Start is called before the first frame update
        public void GenerateSurfaceCpsData()
        {
            surfaceCpsData = ScriptableObject.CreateInstance<SurfaceCpsData>();
            surfaceCpsData.orderX = orderX;
            surfaceCpsData.orderY = orderY;
            surfaceCpsData.loopX = loopX;
            surfaceCpsData.loopY = Mathf.Abs(Mathf.Abs(rotationAngle) - 360f) < 1e-5f;
            surfaceCpsData.size.x = 10;
            surfaceCpsData.size.y = 10;
            surfaceCpsData.count.x = cps.Count;
            surfaceCpsData.count.y = Mathf.CeilToInt(rotationAngle / 45f) + 1;
            surfaceCpsData.cps = new List<CP>();
            var t = rotationAngle / (surfaceCpsData.count.y - 1);
            for (var y = 0; y < surfaceCpsData.count.y; y++)
            for (var x = 0; x < surfaceCpsData.count.x; x++)
            {
                var newPos = Quaternion.AngleAxis(y * t, axis) * cps[x].pos;
                surfaceCpsData.cps.Add(new CP(newPos, cps[x].weight));
            }
        }
    }
}