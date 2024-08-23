using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;

    public GameObject[] coins;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public Rigidbody rigid; // awake에서 초기화
    public BoxCollider boxCollider; // awake에서 초기화
    public MeshRenderer[] meshs; // 태초의 mesh renderer가 가지고 있는 material
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart",2f);
    }

    void Update()
    {
        if(nav.enabled && enemyType != Type.D){
            nav.SetDestination(target.position); //목표물 갱신이 안될뿐 목표물의 마지막위치로 이동이 되어버림
            nav.isStopped=!isChase;
        }
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
        Targeting();// 가까이에 있는 물체 감지
    }

    void Targeting(){// 가까이에 있는 물체 감지
        if(!isDead && enemyType != Type.D){
            float targetRadius = 1.5f; // 최초 만드는 원의 반지름
            float targetRange = 3f; // 원을 쏘는 거리

            switch(enemyType){
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,targetRadius,transform.forward,targetRange,LayerMask.GetMask("Player"));
            if(rayHits.Length > 0 && !isAttack){
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack(){
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack",true);

        switch(enemyType){
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
        
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward*20,ForceMode.Impulse);
                meleeArea.enabled =true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);

                GameObject instantBullet = Instantiate(bullet,transform.position,transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                                
                yield return new WaitForSeconds(2f);                
                break;
        }



        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack",false);

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

    IEnumerator ChangeColor(Color color){
        foreach(MeshRenderer mesh in meshs){
            mesh.material.color = color;
        }
        yield break;
    }

    IEnumerator OnDamage(Vector3 reactVec){
        yield return OnDamage(reactVec,false);
    }

    IEnumerator OnDamage(Vector3 reactVec,bool isGrenade){
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;

        ChangeColor(Color.red);
        yield return new WaitForSeconds(0.1f);
        if(curHealth > 0){
            rigid.AddForce(reactVec * Random.Range(0,2),ForceMode.Impulse);
            ChangeColor(Color.white);
        }else{
            ChangeColor(Color.gray);
            gameObject.layer = 14; //넘버 그대로
            isDead = true;
            anim.SetTrigger("doDie");
            isChase=false;
            nav.enabled=false; // 이 옵션을 써야 y축 액션이 동작함

            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0,3);
            Instantiate(coins[ranCoin],transform.position,Quaternion.identity);

            switch(enemyType){
                case Type.A:
                    GameManager.instance.enemyCntA--;
                    break;
                case Type.B:
                    GameManager.instance.enemyCntB--;
                    break;
                case Type.C:
                    GameManager.instance.enemyCntC--;
                    break;
                case Type.D:
                    GameManager.instance.enemyCntD--;
                    break;
            }

            if(isGrenade){
                reactVec += Vector3.up * 3;
                rigid.freezeRotation = false;

                rigid.AddForce(reactVec * 3,ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15,ForceMode.Impulse);
            }else{
                rigid.AddForce(reactVec * 5,ForceMode.Impulse);
            }

            // if(enemyType != Type.D)
            Destroy(gameObject,4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos){
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;        
        StartCoroutine(OnDamage(reactVec,true));
    }
}
