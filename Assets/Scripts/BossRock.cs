using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot; // 기를 모으고 쏘는 타이밍을 관리할 bool 변수 추가

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer(){
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }
    
    IEnumerator GainPower(){
        while(!isShoot){
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower,ForceMode.Acceleration); // 지속적으로
            yield return new WaitForSeconds(1f/120f);
        }
    }
}
