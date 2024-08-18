using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    float hAxis;
    float vAxis;
    bool wDown; // walkDown의 약자
    bool jDown; // jumpDown의 약자

    bool isJump; // 무한 점프를 막기위한 변수
    bool isDodge; //
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput(){
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move(){
        moveVec = new Vector3(hAxis,0,vAxis).normalized;
        if(isDodge){
            moveVec = dodgeVec;
        }
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn(){        
        transform.LookAt(transform.position + moveVec);
    }

    void Jump(){
        if(jDown && !isJump && moveVec == Vector3.zero && !isDodge){
            rigid.AddForce(Vector3.up * 9.81f,ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", isJump);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge(){
        if(jDown && !isJump && moveVec != Vector3.zero && !isDodge){
            dodgeVec = moveVec;
            speed *=2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut(){
        speed *= 0.5f;
        isDodge=false;
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Floor")){
            isJump=false;
            anim.SetBool("isJump", isJump);
        }
    }
}
