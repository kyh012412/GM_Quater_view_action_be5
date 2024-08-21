using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Start()
    {
        isLook = true;
    }

    void Update()
    {
        if(isDead){
            StopAllCoroutines();
            return;
        }
        if(isLook){ // 바라보고 있는 중이면 //플레이어가 가는방향을 예측
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            lookVec = new Vector3(h,0,v) * 5f;
            transform.LookAt(target.position + lookVec);
        }else{
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think(){
        yield return new WaitForSeconds(0.1f); // 난이도 조절 시 이 값을 조정

        int ranAction = Random.Range(0,5);

        switch (ranAction) {
            case 0:
            case 1:
                // 미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                // 돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                // 점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }

        yield return null;
    }

    IEnumerator MissileShot(){
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile,missilePortA.position,missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile,missilePortA.position,missilePortA.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator RockShot(){
        isLook = false;
        anim.SetTrigger("doBigShot");

        Instantiate(bullet,transform.position,transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt(){
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
