using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo; // 현재무기 최대탄창의 수
    public int curAmmo; // 현재 남은 총알

    public BoxCollider meleeArea; // 근접 공격의 범위
    public TrailRenderer trailEffect; // 휘두룰 때 효과
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use(){
        if(type == Type.Melee){
            StopCoroutine(Swing());
            StartCoroutine(Swing());
        }else if(type == Type.Range && curAmmo > 0){
            curAmmo--;
            StartCoroutine(Shot());
        }
    }

    IEnumerator Swing(){
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot(){
        // #1 총알 발사
        GameObject instantBullet = Instantiate(bullet,bulletPos.position,bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50f;

        // #2 탄피 배출
        GameObject instantCase = Instantiate(bulletCase,bulletPos.position,bulletPos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

        //힘을 받을 방향
        Vector3 caseVec = bulletCasePos.forward*Random.Range(-3,-2) +Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseVec,ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up*10,ForceMode.Impulse); // 탄피 회전힘






        yield return null;
    }
}
