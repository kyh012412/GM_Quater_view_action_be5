using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;
    float vAxis;

    bool wDown; // walkDown의 약자
    bool jDown; // jumpDown의 약자
    bool iDown; // interaction
    bool sDown1; // swapDown
    bool sDown2; // swapDown
    bool sDown3; // swapDown

    bool isJump; // 무한 점프를 막기위한 변수
    bool isDodge; // 회피 중인지
    bool isSwap; // 스왑중인지
    
    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;

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
        Swap();
        Interaction();
    }


    void GetInput(){
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move(){
        moveVec = new Vector3(hAxis,0,vAxis).normalized;
        if(isDodge){
            moveVec = dodgeVec;
        }
        if(isSwap) moveVec = Vector3.zero;
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn(){        
        transform.LookAt(transform.position + moveVec);
    }

    void Jump(){
        if(jDown && !isJump && moveVec == Vector3.zero && !isDodge && !isSwap){
            rigid.AddForce(Vector3.up * 9.81f,ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", isJump);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge(){
        if(jDown && !isJump && moveVec != Vector3.zero && !isDodge && !isSwap){
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

    
    void Swap(){
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;


        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isSwap){
            if(equipWeapon != null)
                equipWeapon.SetActive(false);
            equipWeapon = weapons[weaponIndex];
            equipWeaponIndex = weaponIndex;
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut",0.4f);
        }
    }
    void SwapOut(){
        isSwap =false;

    }

    void Interaction(){
        // 상호작용의 조건
        if(iDown && nearObject !=null && !isJump && !isDodge){

            // 상호작용하는 물체의 태그에 다른 분기처리
            if(nearObject.CompareTag("Weapon")){

                // 기존에 같은 무기 소유를 했는지 체크
                Item item = nearObject.GetComponent<Item>();

                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                // 근처의 오브젝트 삭제
                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Floor")){
            isJump=false;
            anim.SetBool("isJump", isJump);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Weapon")){
            nearObject = other.gameObject;
        }
        // Debug.Log(nearObject);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Weapon"))
            nearObject = null;
    }
}
