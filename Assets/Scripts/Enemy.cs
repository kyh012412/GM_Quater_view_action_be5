using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    Rigidbody rigid; // awake에서 초기화
    BoxCollider boxCollider; // awake에서 초기화
    Material mat; // 태초의 mesh renderer가 가지고 있는 material
    NavMeshAgent nav;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart",2f);
    }

    void Update()
    {
        if(isChase)
            nav.SetDestination(target.position);
        
    }

    void FreezeVelocity(){
        if(!isChase) return;
        rigid.velocity =Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    void ChaseStart(){
        isChase = true;
        anim.SetBool("isWalk",true);
    }

    // void StopToWall(){
    //     Debug.DrawRay(transform.position,transform.forward *5,Color.green);
    //     // isBorder = Physics.Raycast(transform.position,transform.forward,5,LayerMask.GetMask("Wall"));
    // }

    void FixedUpdate()
    {
        FreezeVelocity();
        // StopToWall();
    }

    void OnEnable()
    {
        curHealth = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("something in");
        if(other.CompareTag("Melee")){
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Debug.Log("Melee : " + curHealth);

            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }else if(other.CompareTag("Bullet")){
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            
            Debug.Log("Range : " + curHealth);
            
            Vector3 reactVec = transform.position - other.transform.position;            
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }
    IEnumerator OnDamage(Vector3 reactVec){
        yield return OnDamage(reactVec,false);
    }

    IEnumerator OnDamage(Vector3 reactVec,bool isGrenade){
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;

        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if(curHealth > 0){
            rigid.AddForce(reactVec * Random.Range(0,2),ForceMode.Impulse);
            mat.color = Color.white;
        }else{
            mat.color = Color.gray;
            gameObject.layer = 14; //넘버 그대로
            anim.SetTrigger("doDie");
            isChase=false;
            nav.enabled=false; // 이 옵션을 써야 y축 액션이 동작함

            if(isGrenade){
                reactVec += Vector3.up * 3;
                rigid.freezeRotation = false;

                rigid.AddForce(reactVec * 3,ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15,ForceMode.Impulse);
            }else{
                rigid.AddForce(reactVec * 5,ForceMode.Impulse);
            }

            Destroy(gameObject,4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos){
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;        
        StartCoroutine(OnDamage(reactVec,true));
    }
}
