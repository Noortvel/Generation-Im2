#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OceanMeshGenerator))]
[CanEditMultipleObjects]
public class OceanMeshGeneratorEditor : Editor
{
    private OceanMeshGenerator generator;
    private void OnEnable()
    {
        generator = target as OceanMeshGenerator;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Calculate Mesh"))
        {
            generator.CalculateMesh();
        }
    }

}

[ExecuteInEditMode]
public class OceanMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private int x_lenght = 0;
    [SerializeField]
    private int z_lenght = 0;


    public void CalculateMesh()
    {
        var mesh = GetComponent<MeshFilter>();
        var coliderMesh = GetComponent<MeshCollider>();
        var genmesh = GeneratePlane(x_lenght, z_lenght);



        genmesh.RecalculateBounds();
        genmesh.name = "OceanGen";
        mesh.sharedMesh = genmesh;
        //coliderMesh.sharedMesh = genmesh;
        mesh.transform.localScale = new Vector3(mesh.transform.localScale.x, -Mathf.Abs(mesh.transform.localScale.y), mesh.transform.localScale.z);
    }
    private static Mesh Quad1x1(Vector3 origin)
    {
        Vector3 length = new Vector3(1, 0, 0);
        Vector3 width = new Vector3(0, 0, 1);
        var normal = (Vector3.Cross(length, width)).normalized;
        Mesh mesh = new Mesh
        {
            vertices = new[] { origin, origin + length, origin + length + width, origin + width },
            normals = new[] { normal, normal, normal, normal },
            uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) },
            triangles = new[] { 0, 1, 2, 0, 2, 3 }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
    private static Mesh GeneratePlane(int len, int width)
    {
        Mesh mesh = new Mesh();
        CombineInstance[] combine = new CombineInstance[len * width];
        int i = 0;
        for (int x = 0; x < len; x++)
        {
            for (int y = 0; y < width; y++)
            {
                combine[i].mesh = Quad1x1(new Vector3(x, 0, y));
                i++;
            }
        }
        mesh.CombineMeshes(combine, true, false);

        return mesh;
    }

}

#endif
