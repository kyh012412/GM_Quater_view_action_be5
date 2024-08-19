using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;

    public int maxHasGrenades;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;


    float hAxis;
    float vAxis;

    bool wDown; // walkDown의 약자
    bool jDown; // jumpDown의 약자
    bool fDown; // FireDown의 약자
    bool rDown; // ReloadDown의 약자
    bool iDown; // interaction
    bool sDown1; // swapDown
    bool sDown2; // swapDown
    bool sDown3; // swapDown

    bool isJump; // 무한 점프를 막기위한 변수
    bool isDodge; // 회피 중인지
    bool isSwap; // 스왑중인지
    bool isReload; // 재장전중인지
    bool isFireReady = true; // (공격 가능한 상태)
    
    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

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
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }


    void GetInput(){
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1"); // 기본적인 마우스 좌클릭
        rDown = Input.GetButtonDown("Reload"); 
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move(){
        moveVec = new Vector3(hAxis,0,vAxis).normalized;

        if(isDodge) moveVec = dodgeVec;

        if(isSwap || isReload || !isFireReady){ 
            // Debug.Log("zerofy" + isSwap + isFireReady);            
            moveVec = Vector3.zero; 
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn(){
        // #1 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // #2 마우스에 의한 회전
        if(fDown){
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)){
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y=0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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

    void Attack(){
        if(equipWeapon == null){
            return;
        }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap){
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type ==Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload(){
        if(equipWeapon == null) return;

        if(equipWeapon.type == Weapon.Type.Melee) return;

        if(ammo == 0) return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isReload){
            anim.SetTrigger("doReload");
            isReload=true;

            Invoke("ReloadOut",3f);
        }
    }

    void ReloadOut(){
        // 코드 수정 필요 재장전시 무조건 maxAmmo가 빠지는것에 대한 
        int reAmmo = ammo <equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo;
        ammo -= reAmmo;
        isReload=false;
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
                equipWeapon.gameObject.SetActive(false);
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeaponIndex = weaponIndex;
            equipWeapon.gameObject.SetActive(true);

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
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item")){
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    ammo += item.value;
                    ammo = Mathf.Min(ammo,maxAmmo);
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    coin = Mathf.Min(coin,maxCoin);
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    health = Mathf.Min(coin,maxHealth);
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    hasGrenades = Mathf.Min(hasGrenades,maxHasGrenades);
                    break;
            }
            Destroy(other.gameObject);
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
