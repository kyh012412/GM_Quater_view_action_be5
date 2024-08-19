using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; // 공전의 중심대상
    public float orbitSpeed;
    Vector3 offset;
    
    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        // 코드 수정 가능
        transform.position = target.position+offset;
        transform.RotateAround(target.position,Vector3.up,orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;
    }
}
