using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NURBS
{
    public class SurfaceCpsGenerator : MonoBehaviour
    {
        [SerializeField] protected List<CP> splineCps1;
        [SerializeField] protected List<CP> splineCps2;
        [SerializeField] protected int orderX = 3;
        [SerializeField] protected int orderY = 3;
        [SerializeField] protected bool loopX = false;
        [SerializeField] protected bool loopY = false;
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
            surfaceCpsData.loopY = loopY;
            surfaceCpsData.size.x = 10;
            surfaceCpsData.size.y = 10;
            surfaceCpsData.count.x = splineCps1.Count;
            surfaceCpsData.count.y = splineCps2.Count;
            surfaceCpsData.cps = new List<CP>();
            for (var y = 0; y < splineCps2.Count; y++)
            for (var x = 0; x < splineCps1.Count; x++)
            {
                // surfaceCpsData.cps[x + y * splineCps1.Count] = new CP(splineCps1[x].pos + splineCps2[y].pos,
                //     splineCps1[x].weight * splineCps2[y].weight);
                surfaceCpsData.cps.Add(new CP(splineCps1[x].pos + splineCps2[y].pos,
                    splineCps1[x].weight * splineCps2[y].weight));
            }
        }
    }
}