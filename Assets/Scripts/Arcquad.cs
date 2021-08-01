using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arcquad : MonoBehaviour
{
    public GameObject quad;

//    public float width;
//    public float height;
    private MeshFilter _meshFilter;

//    // Use this for initialization
    void Start()
    {
        _meshFilter = quad.GetComponent<MeshFilter>();
        // var mesh = new Mesh();
        _meshFilter.mesh = CreateArcSurface();

//        CreateQuadMesh(mesh);
//        CreateReticleVertices();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Range(0, 4)] public float threshold;
    public float planeWidth = 1;
    public float planeHeight = 1;
    public int planesSeg = 64;

    private Mesh CreateArcSurface()
    {
        Mesh mesh = new Mesh();

        int segments_count = planesSeg;
        int vertex_count = (segments_count + 1) * 2;

        Vector3[] vertices = new Vector3[vertex_count];

        int vi = 0;

        // 普通平面步
        float widthSetup = planeWidth * 1.0f / segments_count;

        // 半径
        float r = planeWidth * 1.0f / (Mathf.Sin(threshold / 2) * 2);

        // 弧度步
        float angleSetup = threshold / planesSeg;

        // 余角
        float coangle = (Mathf.PI - threshold) / 2;

        // 弓形的高度
        // https://zh.wikipedia.org/wiki/%E5%BC%93%E5%BD%A2
        float h = r - (r * Mathf.Cos(threshold / 2));

        // 弓形高度差值（半径-高度）
        float diff = r - h;

        for (int si = 0; si <= segments_count; si++)
        {
            float x = 0;

            float z = 0;

            if (threshold == 0)
            {
                // 阈值为0时,按照普通平面设置顶点
                x = widthSetup * si;

                vertices[vi++] = new Vector3(-planeWidth / 2 + x, planeHeight / 2, z);

                vertices[vi++] = new Vector3(-planeWidth / 2 + x, -planeHeight / 2, z);
            }
            else
            {
                // 阈值不为0时,根据圆的几何性质计算弧上一点
                // https://zh.wikipedia.org/wiki/%E5%9C%86
                x = r * Mathf.Cos(coangle + angleSetup * si);
                z = r * Mathf.Sin(coangle + angleSetup * si);

                vertices[vi++] = new Vector3(-x, planeHeight / 2, z - diff);
                vertices[vi++] = new Vector3(-x, -planeHeight / 2, z - diff);
            }
        }

        int indices_count = segments_count * 3 * 2;
        int[] indices = new int[indices_count];

        int vert = 0;
        int idx = 0;
        for (int si = 0; si < segments_count; si++)
        {
            indices[idx++] = vert + 1;
            indices[idx++] = vert;
            indices[idx++] = vert + 3;

            indices[idx++] = vert;
            indices[idx++] = vert + 2;
            indices[idx++] = vert + 3;

            vert += 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = indices;

        // // https://answers.unity.com/questions/154324/how-do-uvs-work.html
        Vector2[] uv = new Vector2[vertices.Length];

        float uvSetup = 1.0f / segments_count;

        int iduv = 0;
        for (int i = 0; i < uv.Length; i = i + 2)
        {
            uv[i] = new Vector2(uvSetup * iduv, 1);
            uv[i + 1] = new Vector2(uvSetup * iduv, 0);
            iduv++;
        }

        mesh.uv = uv;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
    
}
