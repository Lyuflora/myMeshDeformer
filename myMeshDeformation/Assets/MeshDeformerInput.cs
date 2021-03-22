using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f;
    public float forceOffset = 0.1f;

    void Update()
    {
        if (Input.GetMouseButton(0))    // 触发 deformation 条件
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);   // Ray 的数据结构是两个 Vec3，分别是 origin, direction
        //  这里是从 camera 到 a screen point.

        RaycastHit hit;
        // hit 是储存Raycast相关信息的数据结构
        if (Physics.Raycast(inputRay, out hit)) // out 可以让函数传递出多个返回值
        {           
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();  // 挂载在 与Ray碰撞的 mesh 上的 deformer
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;  // 在一个很接近表面的位置
                deformer.AddDeformingForce(point, force);
            }
        }
    }
}
