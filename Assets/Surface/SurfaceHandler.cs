using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace NURBS
{
    public class SurfaceHandler : MonoBehaviour
    {
        [SerializeField] protected SurfaceCpsData data;
        [SerializeField] public Material mat;
        [SerializeField] public Vector2Int division;
        [SerializeField] protected string bakePath = "Assets/Surface/Bakedmesh";
        [SerializeField] protected string bakeName = "bakedMesh";

        public SurfaceCpsData Data
        {
            get => data;
            set => data = value;
        }

        public Surface surface { get; protected set; }
        public Mesh mesh { get; private set; }
        public string BakePath => bakePath;
        public string BakeName => bakeName;

        protected float normalizedT(float t, int count, int order) =>
            Mathf.Clamp((t * (count + 1) + order - 1) / (count + order), 0, 1 - 1e-5f);

        protected MeshRenderer rndr;
        protected MeshFilter fltr;
        protected NativeArray<Vector3> vtcs;

        private void Start()
        {
            mat = new Material(Shader.Find("VR/SpatialMapping/Wireframe"));
            Debug.Log("SurfaceHandler start");
        }

        public void Generate()
        {
            surface = new Surface(data.cps, data.orderX, data.orderY, data.loopX, data.loopY, data.count.x,
                data.count.y);
            for (var y = 0; y < data.count.y; y++) {
                for (var x = 0; x < data.count.x; x++)
                {
                    var i = data.Convert(x, y);
                    surface.UpdateCP(new Vector2Int(x, y), new CP(transform.position + data.cps[i].pos, data.cps[i].weight));
                }
            }
            CreateMesh();
        }


        public void RuinSelf()
        {
            Destroy(this);
        }


        public void OnDestroy()
        {
            Debug.Log("SurfaceHandler destroy");
            surface.Dispose();
            vtcs.Dispose();
        }

        public void Reset()
        {
            Debug.Log("SurfaceHandler reset");
            surface?.Dispose();
            surface = new Surface(data.cps, data.orderX, data.orderY, data.loopX, data.loopY, data.count.x,
                data.count.y);
        }

        public Vector3 GetCurve(float t1, float t2)
        {
            return surface.GetCurve(normalizedT(t1, data.count.x, data.orderX),
                normalizedT(t2, data.count.y, data.orderY));
        }

        private void CreateMesh()
        {
            mesh = new Mesh();
            fltr = gameObject.AddComponent<MeshFilter>();
            rndr = gameObject.AddComponent<MeshRenderer>();
            vtcs = new NativeArray<Vector3>((division.x + 1) * (division.y + 1) * 2, Allocator.Persistent);

            var idcs = new List<int>();
            var lx = division.x + 1;
            var ly = division.y + 1;
            var offsetX = data.loopX ? 1f * data.orderX / (data.orderX * 2 + data.count.x) : 0f;
            var offsetY = data.loopY ? 1f * data.orderY / (data.orderY * 2 + data.count.y) : 0f;
            var dx = (1f - 2 * offsetX) / division.x;
            var dy = (1f - 2 * offsetY) / division.y;

            //用来求平均点
            var vecSum = Vector3.zero;

            for (var iy = 0; iy < ly; iy++)
            for (var ix = 0; ix < lx; ix++)
            {
                var i = ix + iy * lx;
                vtcs[i] = surface.GetCurve(offsetX + ix * dx, offsetY + iy * dy);
                vecSum += vtcs[i];
                if (iy < division.y && ix < division.x)
                {
                    idcs.Add(i);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx);
                    idcs.Add(i + lx);
                    idcs.Add(i + 1);
                    idcs.Add(i + lx + 1);
                }
            }

            // 所有点的平均点
            Vector3 center = vecSum / (lx * ly);
            //double side
            for (var iy = 0; iy < ly; iy++)
            for (var ix = 0; ix < lx; ix++)
            {
                var i = ix + iy * lx + lx * ly;
                vtcs[i] = surface.GetCurve(offsetX + ix * dx, offsetY + iy * dy);
                if (iy < division.y && ix < division.x)
                {
                    idcs.Add(i + lx);
                    idcs.Add(i + lx + 1);
                    idcs.Add(i + 1);
                    idcs.Add(i);
                    idcs.Add(i + lx);
                    idcs.Add(i + 1);
                }
            }

            for (int i = 0; i < vtcs.Length; i++)
            {
                vtcs[i] -= center;
            }
            mesh.SetVertices(vtcs);
            mesh.SetTriangles(idcs.ToArray(), 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            // var uvs = new Vector2[vtcs.Length];
            // for (var i = 0; i < uvs.Length; i++)
            //     uvs[i] = new Vector2(vtcs[i].x, vtcs[i].z);
            // mesh.uv2 = uvs;

            //rndr.material = mat;
            fltr.mesh = mesh;
        }

        [BurstCompile]
        struct UpdateMeshJob : IJobParallelFor
        {
            [WriteOnly] public NativeArray<Vector3> vtcs;
            [ReadOnly] public NativeArray<CP> cps;
            public Vector2Int division;
            public Vector2Int cpsLen;
            public Vector2 invDiv;
            public Vector2 offsetDiv;
            public int orderX;
            public int orderY;

            public void Execute(int id)
            {
                var l = division.x + 1;
                var x = (id % l) * invDiv.x * (1f - 2 * offsetDiv.x) + offsetDiv.x;
                var y = (id / l) * invDiv.y * (1f - 2 * offsetDiv.y) + offsetDiv.y;
                vtcs[id] = NURBSSurface.GetCurve(cps, x, y, orderX, orderY, cpsLen.x, cpsLen.y);
            }
        }


        public void UpdateMesh()
        {
            var job = new UpdateMeshJob
            {
                vtcs = vtcs,
                cps = surface.cps,
                division = division,
                invDiv = new Vector2(1f / division.x, 1f / division.y),
                offsetDiv = new Vector2(data.loopX ? 1f * data.orderX / (data.orderX * 2 + data.count.x) : 0f,
                    data.loopY ? 1f * data.orderY / (data.orderY * 2 + data.count.y) : 0f),
                cpsLen = data.count,
                orderX = data.orderX,
                orderY = data.orderY
            };
            job.Schedule(vtcs.Length, 0).Complete();

            mesh.SetVertices(vtcs);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
    }
}