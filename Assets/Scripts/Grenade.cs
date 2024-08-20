using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion(){
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // 센터, 반지름, 쏘는방향 (무관), 구체를 위로보내는것이 아니기에 0, 레이어 마스크
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,15f,Vector3.up,0f,LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits){
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject,5f);
    }
}
