using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HollowCylinderSmooth : MonoBehaviour
{
    [Header("锅壁参数")]
    public int segments = 64;
    public float radius = 0.5f;
    public float thickness = 0.05f;
    public float height = 0.1f;

    void Start()
    {
        GenerateHollowCylinder();
    }

    void GenerateHollowCylinder()
    {
        Mesh mesh = new Mesh();
        mesh.name = "SmoothHollowCylinder";

        int vertCount = segments * 2 * 2; // 2 rings (outer + inner), each with bottom + top

        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        int[] triangles = new int[segments * 12]; // 2 sides (outer + inner), 2 triangles per quad

        float angleStep = 2 * Mathf.PI / segments;

        // 构建顶点 & 法线
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 dir = new Vector3(cos, 0, sin);

            // 外圈
            Vector3 upOffset = Vector3.up * 0f; // bottom at Y = 0
            Vector3 heightOffset = Vector3.up * height;

            vertices[i * 2] = (radius + thickness) * dir + upOffset;
            vertices[i * 2 + 1] = (radius + thickness) * dir + heightOffset;
            normals[i * 2] = dir;
            normals[i * 2 + 1] = dir;

            // 内圈（偏移 segments*2）
            vertices[segments * 2 + i * 2] = radius * dir + upOffset;                 // bottom inner
            vertices[segments * 2 + i * 2 + 1] = radius * dir + Vector3.up * height; // top inner
            normals[segments * 2 + i * 2] = -dir;
            normals[segments * 2 + i * 2 + 1] = -dir;
        }

        // 构建三角面
        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            // 外圈面
            int i0 = i * 2;
            int i1 = next * 2;

            triangles[triIndex++] = i0;
            triangles[triIndex++] = i0 + 1;
            triangles[triIndex++] = i1 + 1;

            triangles[triIndex++] = i0;
            triangles[triIndex++] = i1 + 1;
            triangles[triIndex++] = i1;

            // 内圈面（翻转）
            int i2 = segments * 2 + i * 2;
            int i3 = segments * 2 + next * 2;

            triangles[triIndex++] = i2;
            triangles[triIndex++] = i3 + 1;
            triangles[triIndex++] = i2 + 1;

            triangles[triIndex++] = i2;
            triangles[triIndex++] = i3;
            triangles[triIndex++] = i3 + 1;
        }

        // 应用到 Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        GetComponent<MeshFilter>().mesh = mesh;

        // 设置 MeshCollider
        var collider = GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = true;
        
        Destroy(GetComponent<MeshCollider>());
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            GameObject wall = new GameObject("PanWall" + i);
            wall.transform.parent = transform;
            wall.transform.localPosition = new Vector3(x, height/2, z); // 半高位置
            wall.transform.localRotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
            wall.transform.localScale = new Vector3(0.05f, height, 0.05f);

            wall.AddComponent<BoxCollider>();
        }
    }
}
