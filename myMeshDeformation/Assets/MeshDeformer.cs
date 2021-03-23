using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    private void Start()
    {
        // 获得初始的 mesh 顶点位置，并储存到 deform 数组中
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length]; ;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
    }
    public void AddDeformingForce(Vector3 point, float force)
    {
        Debug.DrawLine(Camera.main.transform.position, point);  // Debug
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;   // 施力点到 vertices 的向量
        // 下一行设置笔刷
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);  // F 与距离的平方成反比
        float velocity = attenuatedForce * Time.deltaTime;  // dv = F dt （不考虑 m）
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
    void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals(); // 重新计算法线
    }

    public float springForce = 20f;
    public float damping = 5f;
    private bool deformDone = false;
    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];

            //Vector3 nextVelocity = velocity - displacement * springForce * Time.deltaTime;
            //// velocity -= displacement * springForce * Time.deltaTime;
            //if(Vector3.Dot(velocity, nextVelocity) <0.01f)
            //{
            //    velocity = nextVelocity = new Vector3(.0f, .0f, .0f);
            //    deformDone = true;
            //}


        velocity -= displacement * springForce * Time.deltaTime;

        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        
        displacedVertices[i] += velocity * Time.deltaTime;  // 逐渐变形

        if (deformDone)
        {
            // Update original mesh
            originalVertices = displacedVertices;
            deformDone = false;
        }
    }
}
