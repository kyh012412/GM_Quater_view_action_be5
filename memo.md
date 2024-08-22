### 3D 쿼터뷰 액션게임 - 플레이어 이동 [B38 + Asset]

#### 준비하기

1. 에셋 다운로드

#### 지형 만들기

1. 하이라키에 Cube 생성(Floor)
   1. 위치 리셋
   2. 스케일 x100 y1 z100
2. 만약에 흰색이 아닌 큐브가 생성됐을 시
   1. window > rendering > lighting
   2. generate lighting으로 빛 생성
3. 새로운 cube 생성(Wall)
   1. 스케일 x110 y 10 z 10
   2. position z -55
4. 벽을 복사하여 사방을 막아준다.
5. _메모 중요_
   1. 3d Object는 Mesh와 MeshRenderer로 구성이 되어 있음
   2. Mesh는 실제로 폴리곤에 의해 그려진 데이터
   3. 그것을 우리눈에 보여주는 것이 Mesh Renderer
   4. 보이지 않을 3d 오브젝트는 Mesh Renderer만 끄면된다.
6. Assets/Materials 폴더를 만듬
   1. 폴더 내에서 Material 생성(Floor)
   2. inspector에 albedo의 동그라미를 눌러서 Tile을 선택
7. 기존의 존재하는 3d object에 4. Floor Material 적용
8. 다시 Floor Material의 inspector로 와서
   1. 중간쯤 Tiling 1,1값을 10,10으로 변경
   2. albedo 우측에 색상을 눌러서 변경 가능 616874

#### 플레이어 만들기

1. 프리펩 : 게임 오브젝트를 에셋으로 보관된 형태
2. Player를 하이라키에 드랍
   1. pos y 1.24
3. Player에 필요한 컴포넌트
   1. 콜라이더 (캡슐 콜라이더)
      1. 위치도 맞지않음
      2. 크기도 맞지않음
      3. center y 2
      4. radius 1.3
      5. height 4
   2. rigidbody
   3. Script
4. Assets/Scripts에 Player.cs 생성후 넣어주기

#### 기본 이동 구현

1. Player.cs
2. 코드
3. rigidbody rotation x,z를 얼려준다.
4. 벽과 부딪힐경우 물리충돌이여서
   1. fixedUpdated 연산이관여하고
   2. update와 타이밍이 좋지않을경우 물리 충돌을 무시하는 경우가 발생
5. Rigidbody 의 속성 Collision Detection을 Continous로 바꿔준다.
   1. (CPU를 조금 더 쓰지만 물리 충돌을 잘 계산하게됨)

#### 애니메이션

1. Assets/Animations 폴더를 만들어준다.
   1. 컨트롤러를 만들어준다.(Player)
2. 이 Player controller를 Player이 이하인 Mesh Object에 넣어준다.
3. 다운받은 에셋/Models 에 FBX라는 파일로 모델들이있음
   1. .FBX : 각종 정보들이 구분되어 저장되는 3D 포맷
4. 맨 끝에 Simple Player를 전개해보면 여러 애니메이션이 있다.
   1. Idle/walk/run animation을 드래그해서 넣어준다.
   2. transition을 연결해주고
   3. 파라미터는 isWalk와 isRun을 사용
5. Transition
   1. (뛰는것이 디폴트일때)
   2. Idle > Walk 에 컨디션 추가
   3. has exit Time은 체크해제
   4. transition duration은 0.1
   5. 역방향도 동일
   6. Walk > Run 의 컨디션 isWalk true 일때
   7. Run > Walk 의 컨디션 isWalk false 일때
   8. Walk > Idle 의 컨디션 isRun false 일때
   9. Idle > Walk의 컨디션 (2가지동시에)
      1. isWalk true
      2. isRun true
6. Player.cs 연결
   1. 자식요소에 animator가 있으므로 getcomponentinchildren 사용
7. InputManager로 가서 새로운 것추가
   1. Walk 추가
   2. Positive Button - left shift (띄어쓰기 중요)
   3. Alt Positive Button - (공백)
8. 코드
9. 테스트

#### 기본 회전 구현

1. _transform.LookAt_ : 지정된 벡터를 향해서 회전시켜주는 함수
   1. 매개변수로 벡터 하나만 넣는것을 사용하면 편한듯 (보통 현위치 + 이동방향)

#### 카메라 이동

1. 카메라 좌표
   1. position 0 21 -11
   2. rotation 60 0 0
2. 카메라에 넣을 Follow.cs
   1. offset : 상대적 거리를 담을 변수(일정값 유지)
   2. target 연결
3. 테스트

#### 느낌 살리기

1. 빈 객체 생성 후 (WorldSpace) 1. 바닥과 벽을 넣어서 묶어준다. 2. rotation y 45
   player.cs

```cs
public class Player : MonoBehaviour
{
	public float speed;

	float hAxis;
	float vAxis;
	bool wDown; // walkDown의 약자

	Vector3 moveVec;
	Animator anim;

	void Awake()
	{
		anim = GetComponentInChildren<Animator>();
	}

	void Update()
	{
		hAxis = Input.GetAxisRaw("Horizontal");
		vAxis = Input.GetAxisRaw("Vertical");
		wDown = Input.GetButton("Walk");
		moveVec = new Vector3(hAxis,0,vAxis).normalized;

		transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
		anim.SetBool("isRun", moveVec != Vector3.zero);
		anim.SetBool("isWalk", wDown);

		transform.LookAt(transform.position + moveVec);
	}
}
```

Follow.cs

```cs
public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Awake()
    {
        offset = transform.position-target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;
    }
}
```

### 3D 쿼터뷰 액션게임 - 플레이어 점프와 회피 [B39]

#### 코드정리

1. `bool isJump;`변수를 사용하여 무한 점프를 방지
2. 착지시 재활용할수 있게 값 수정
3. 애니메이터 추가
4. any state에서 나가는것들은 trigger 사용 (doJump, doDodge)
   1. 착지를 잘 제어하기위해 isJump(bool)파라미터도 추가
5. any state -> dodge
   1. 나가는시간 체크해제
   2. transition duration 0
   3. 컨디션 doDodge
6. dodge -> exit
   1. 나가는 시간 체크
   2. transition duration 0.1
7. any state -> jump
   1. 나가는시간 체크해제
   2. transition duration 0
   3. 컨디션 doJump
8. jump -> land
   1. 나가는시간 체크해제
   2. transition duration 0
   3. 컨디션 isJump false일때
9. land -> exit
   1. 나가는 시간체크
   2. transition duration 0.1
10. 테스트
11. edit >project settings > physics 에서 중력설정

#### 지형 물리 편집

1. WorldSpace 객체에서 Inspector 창에서 static을 체크
   1. 자식들도 같이 바꿀지 물어보면 yes
2. _static으로 돌리는 이유_
   1. Player > rigidbody에서 > collision detection의 값을 continuous로 했는데(벽뚫을 더 잘 막기위하여)
   2. continous를 효과적으로 쓰려면 이 객체와 충돌하는물체가 static이여야함
3. - 충돌할때 유니티가 빨리 계산하기 위해서는 둘다 rigidbody를 가지고 있는것이 좋음
4. WorldSpace의 자식객체들에 rigidbody 추가
   1. use gravity를 끄고
   2. is kinematic을 켠다.
      1. 코드상으로 직접움직여주지않으면 움직이지 않는다.
5. Materials내에 Physics Material추가(Wall)
   1. Dynamic Friction 0
   2. Static Friction 0
   3. Friction Combine 값은 Minimum (마찰력 최소를 위해)
   4. Wall에 넣어준다.(Box Collider 내의 Material에서 확인가능)

#### 회피 구현

1. Dodge 메서드 구현(Jump copy후 개조)
2. bool 변수 isDodge추가
3. dodge 애니메이션 스피드 3
4. 테스트
   player.cs

```cs
bool jDown; // jumpDown의 약자
bool isJump; // 무한 점프를 막기위한 변수
bool isDodge; //

Vector3 dodgeVec;
Rigidbody rigid;

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
```

### 3D 쿼터뷰 액션게임 - 아이템 만들기 [B40]

#### 아이템 준비

1. 받은 에셋의 prefab에서 Weapon Hammer를 하이라키에 드래그드랍
2. 그 내부의 Mesh Object의 값을 조절
   1. position y 1.5
   2. rotation z 30

#### 라이트 이펙트

1. Weapon Hammer 내에 빈객체 추가(Light)
   1. Light 컴포넌트 추가
2. Light
   1. 의 pos y 0.9
   2. type 속성이 있는데. 기본적인 (Point) 사용
   3. intensity 빛의 세기 10
   4. Range 빔의 범위 4
   5. 빛의 색 ff7100
   6. shadow 값은 별개로 주지 않음

#### 파티클 이펙트

1. 빈객체추가 (Particle)
   1. Particle System 이라는 컴포넌트 추가
   2. 분홍색(Magenta)이 피어오르는데 이유는
      1. 재질을 못 찾을때 이렇게 보이게 된다.
   3. 맨끝쯤에 Render에 Material이 있음
      1. Default-Line 선택해서 연결
   4. Emission : 파티클 입자 출력양 (초당 출력양?) 15로 설정
   5. shape : 발사하는 각도의 모양
      1. cone 외에도 구체 반구체 모양등 다양한 모양이 있음
      2. cone에 발사각을 rotation x의 값을 위로 쏘기위해 -90으로해준다.
   6. color over lifetime : 시간에 따른 색상변화
      1. gradient editor를 열고
      2. 좌하단앵커 클릭후 컬러값을 설정
      3. 56.2% 지점에 앵커를 추가해주고 컬러를 fff500으로 해준다.
      4. 상단의 앵커는 알파값을제어
      5. 우상단의앵커의 알파값은 0으로 해준다.
   7. size over lifetime : 시간에 따른 크기변화
      1. 커브그래프를 조정해준다.
      2. 시계 열그래프로 왼쪽이 초기 우측이 후기이다.
   8. limit velocity over lifetime : 시간에 따른 속도제한
      1. drag : 저항 값을 1로 해준다.
   9. 이쁘게 잘 만들기 위해서는 Start LifeTime과 Start Speed값을 잘 조정해야함
   10. Start Lifelife타임 우측에 화살표를 누르면 Random between two constants를 선택할수있다.
       1. 2~4사용

#### 로직 구현

1. Weapon Hammer 내에 RigidBody 추가
2. Sphere Collider(첫번째) 추가
   1. center y 1
3. Sphere Collider(두번째) 추가
   1. radius 5
   2. is trigger 체크
4. Item.cs
   1. enum : 열거형 타입 (타입 이름 지정 필요)
5. Hammer 내에 item.cs 추가
   1. type weapon value 0
6. Weapon HandGun 모델을 하이라키에 드랍
7. HandGun > Mesh Object
   1. pos y 1.5
   2. rotation z 25
   3. scale 1.3
   4. Light 객체추가
8. Hand Gun > Light
   1. pos y 0.8
   2. 컬러 98d200
   3. range 4
   4. intensity 10
9. Hand Gun > particle
   1. renderer > material
      1. default-line
   2. shape Rotation x -90
   3. emission 7
   4. color over lifetime 좌측하단앵커 c7f61d
      1. 63.5% 하단앵커 ffe91a
      2. 우상단앵커 알파 0
   5. size over lifetime 시작1 끝0
   6. limit velocity over lifetime
      1. drag 0.7
   7. start lifetime 2-4
   8. start speed 3-5
10. HandGun 내에 컴포넌트 추가 rigidbody
11. HandGun 내에 컴포넌트 추가 sphere collider 1
    1. center y 1
12. HandGun 내에 컴포넌트 추가 sphere collider 2
    1. radius 5
    2. is trigger 체크
13. HandGun 내에 item.cs 추가
    1. type weapon value 1
14. _똑같이 만들기 참조_
    1. object 기울이기 높이 높여주기
    2. light 컴포넌트 복사후 생상변경
    3. particle 컴포넌트 복사후 색상변경
    4. 부모객체 sphere 컴포넌트 복사
    5. item은 particle radius만드라드게해주기
15. _// weapon들 똑같이 만들기_ 2. submachinegun 3. granade 4. Item Heart 5. item ammo 6. coin gold 7. coin silver 8. coin bronze
16. (아이템 회전 효과 주기)

#### 프리펩 저장

1. tag생성 (Item, Weapon)
2. weapon에는 weapon(3개)
3. 나머지는 Item tag를 넣어준다. (grenade도 아이템태그)
4. 각자 Item.cs가 있는데 올바른 type과 value를 지정해준다.
5. Assets/Prefabs 폴더를 만들어 준다.
6. Prefab화 하면서 original prefab을 눌러준다. 1. Prefab화 후 position을 0 0 0 으로 초기화
   item.cs

```cs
public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
```

### 3D 쿼터뷰 액션게임 - 드랍 무기 입수와 교체 [B41]

#### 오브젝트 감지

1. nearObject 추가
2. ontriggerStay와 ontriggerExit 사용

#### 무기 입수

1. Interaction 버튼추가 (e)
2. player inspector로가서 Has Weapons의 배열크기를 3으로 지정해주기

#### 무기 장착

1. Player > Mesh Object > Bone_Body > Bone_Shoulder_R > Bone_Arm_R > RightHand 이하에 cyclinder 추가(Weapon Point)
   1. 실린더가 외부에서 보이지않게 위치 조정
   2. pos -0.13 3.25 -2.45
   3. scale 4
   4. 캡슐 콜라이더 제거
   5. 메쉬렌더 비활성화
   6. 이하에 받은 에셋 > 프리펩 내에 무기 3개를 넣어주고 확인하기
      1. 비활성화까지
2. Player의 weapons 배열에 위 3개의 객체를 넣어준다.

#### 무기 교체

1. 스크립트
2. bool 변수와 InputManager의 인풋추가
3. equipWeapon 변수추가
4. animator 내에서 파라미터 doSwap 추가
5. any state > swap
   1. 컨디션 doSwap
   2. 나가는시간 없고
   3. duration time 0
6. Swap > exit
   1. 컨디션x
   2. 나가는시간 체크
   3. duration time 0.1
7. isSwap 변수를 두어 중첩액션을 막는다.
   player.cs

```cs
public GameObject[] weapons;

public bool[] hasWeapons;

bool iDown; // interaction
bool sDown1; // swapDown
bool sDown2; // swapDown
bool sDown3; // swapDown

bool isDodge; // 회피 중인지
bool isSwap; // 스왑중인지

GameObject nearObject;
GameObject equipWeapon;
int equipWeaponIndex = -1;

void Update()
{
	//...
	Swap();
	Interaction();
}

void GetInput(){
	//..
	iDown = Input.GetButtonDown("Interaction");
	sDown1 = Input.GetButtonDown("Swap1");
	sDown2 = Input.GetButtonDown("Swap2");
	sDown3 = Input.GetButtonDown("Swap3");
}

void Move(){
	//...
	if(isSwap) moveVec = Vector3.zero;
	transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
	//...
}
void Dodge(){

	if(jDown && !isJump && moveVec != Vector3.zero && !isDodge && !isSwap){
		//...
	}

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
```

### 3D 쿼터뷰 액션게임 - 아이템 먹기 & 공전물체 만들기 [B42]

#### 변수 생성

1. 각종 제약사항 변수 생성
2. inspector에서 값 정의

#### 아이템 입수

1. OnTriggerEnter를 사용해서
2. Item tag일시 분기처리

#### 공전 물체 만들기

1. 플레이어내에 gameobject\[\] 변수추가
2. 하이라키에 빈객체추가 (Grenade Group)
   1. pos y 1.5
3. Grenade Group 내에 빈객체 4개추가
   1. 각자마다 x,z방향으로 음또는 양의 방향중 한방향으로만 2만큼 의 거리두게 조정
   2. 각 방향마다 받은에셋 > Prefab의 grenade를 자식객체로 넣어준다.
   3. grenade객체들을 rotation z 30
4. 플레이어 주변의 grenade는 드랍 grenade와 다르게 보이게하기위하여
5. 받은에셋 > materials 에서 Weapon grenade _orbit_ materials를 사용
   1. 각각의 grenade아래에 mesh Object에 이 material을 넣는다.
   2. mesh object내에 light 컴포넌트추가
   3. range 2 intensity 1 컬러 9f4fea
6. particle 추가
   1. Emission 내에
      1. rate over time이 아닌 rate over distance를 사용 10
         1. 시간이 아닌 움직인 거리에 따라서 이펙트가 발생
   2. Mesh Object(에 보면)
      1. simulation space - local인 값을 world로 교체

#### 공전 구현

1. Orbit.cs (추후 넣는위치 Group의 하위 객체인 4개)
2. RotateAround : 타겟 주위를 회전하는 함수
3. player inspector에 grenades에 4를 주고 방향 아래의 grenade를 넣어준다.
   player.cs

```cs
public int hasGrenades;
public int ammo;
public int coin;
public int health;

public int maxHasGrenades;
public int maxAmmo;
public int maxCoin;
public int maxHealth;

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
```

Orbit.cs

```cs
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
```

### 3D 쿼터뷰 액션게임 - 코루틴으로 근접공격 구현하기 [B43]

#### 변수 생성

1. Weapon.cs (이 스크립트는 player 내부에 넣은 3개의 아이템에 넣어줘야함)
2. inspector로와서
3. Hammer
   1. melee 25 0.4
   2. box collider 추가
      1. center y 2.5
      2. size 3.5 3 2
      3. is trigger 체크
   3. 추가후 본인을 등록
   4. 태그 melee

#### 근접 공격 효과

1. Weapon point > Weapon Hammer 내에 객체 추가(Effect)
2. 내에 _Trail renderer_ 라는 컴포넌트 추가
   1. 잔상을 그려주는 컴포넌트
   2. magenta색(분홍색) 이 나오므로 material을 추가해준다.
      1. default line
   3. 그래프에서 우클릭 add key
      1. 우측 아래로 드래그
   4. Time 0.5
   5. Min Vertex Distance 0.1인데 이값을 1.5까지 키우면 각진 모양이 된다.
   6. color
      1. 좌하단 앵커값 ff9700
      2. 중간 하단 (61.8)앵커값 fff100
      3. 우상단 알파값 0
   7. Effect 객체의 y값 증가 2.2
3. Weapon Hammer에 이하의 Effect 객체등록
4. Effect 내의
   1. Trail renderer 컴포넌트 disabled
5. Weapon Hammer의 box collider disabled

#### 공격 로직 (코루틴)

1. 기본의 방식
   1. use() 메인 루틴 -> Swing() 서브 루틴 -> use() 메인 루틴 -> Swing() 서브 루틴
2. 코루틴 방식
   1. use() 메인루틴 + Swing() 코루틴이라고 부름(Co-op)
3. _코루틴_ 함수 : 메인루틴 + 코루틴 (동시실행)
4. *yield 결과*를 전달하는 키워드
5. 예
   ```cs
   IEnumerator Swing(){
   	// 1번 구역 실행
   	yield return null; // 1번구역 실행 후 1프레임 대기
   	// 2번 구역 실행
   }
   ```
6. yield 키워드를 여러 개 사용하여 시간차 로직 작성 가능
7. 1프레임이 아닌 다른 시간도 대기 가능하다.
8. `yield break;`로 코루틴문 탈출 가능
9. 시작하는 방법은 StartCoroutine을 사용한다.
10. 외부에서 중단할때는 StopCoroutine을 사용한다.
11. 코루틴을 호출하기전에 로직이 꼬이지 않게 미리 코루틴을 종료후 시작할수있다.

#### 공격 실행

1. equipWeapon의 자료형을 Weapon으로 변경
2. player animator에 prameter 추가
3. transition 추가
   Weapon.cs

```cs
public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea; // 근접 공격의 범위
    public TrailRenderer trailEffect; // 휘두룰 때 효과

    public void Use(){
        if(type == Type.Melee){
            StopCoroutine(Swing());
            StartCoroutine(Swing());
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
}
```

Player.cs

```cs
bool fDown; // FireDown의 약자
bool isFireReady = true; // 재장전완료 (공격 가능한 상태)
Weapon equipWeapon;
float fireDelay;

void Update()
{
	//...
	Attack();
}

void GetInput(){
	//...
	fDown = Input.GetButtonDown("Fire1"); // 기본적인 마우스 좌클릭
}

//move 와 fire을 동시에 못하게 수정
void Move(){
	moveVec = new Vector3(hAxis,0,vAxis).normalized;

	if(isDodge) moveVec = dodgeVec;

	if(isSwap || !isFireReady) moveVec = Vector3.zero;

	transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

	anim.SetBool("isRun", moveVec != Vector3.zero);
	anim.SetBool("isWalk", wDown);
}
void Attack(){
	if(equipWeapon == null){
		return;
	}

	fireDelay += Time.deltaTime;
	isFireReady = equipWeapon.rate < fireDelay;

	if(fDown && isFireReady && !isDodge && !isSwap){
		equipWeapon.Use();
		anim.SetTrigger("doSwing");
		fireDelay = 0;
	}
}

void Swap(){
	//...
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
```

### 3D 쿼터뷰 액션게임 - 원거리공격 구현 [B44]

#### 총알, 탄피 만들기

1. 하이라키에 빈객체 추가 (Bullet HandGun)
   1. Trail Renderer 컴포넌트 추가
   2. rigidbody 추가
   3. sphere collider 추가
      1. radious 0.3
2. Trail Renderer
   1. material 넣어주기
   2. time 0.3
   3. Min Vertex Distance 0
   4. width 0.3~0
   5. color a9de14(10.3) ~ ffc200(61.8) / 우상단 알파 50
3. Bullet HandGun를 복사(Bullet SubMachineGun)
   1. trail renderder
      1. time 0.1
      2. color 변경
4. 탄피는 받은에셋 > prefab > Bullet Case가 있음 하이라키에 드랍
   1. 그 이하의 Mesh Object scale 0.5
5. Bullet Case에 rigidbody 와 box collider를 넣어준다.
   1. box collider size 0.45 0.3 0.3
6. Bullet.cs 생성
7. Bullet HandGun, Bullet SubMachineGun, Bullet Case 에 Bullet.cs를 넣어준다.
   1. 20, 7, 0 넣어주고 prefab화
   2. 그후 위치를 0,0,0으로 초기화

#### 발사 구현

1. Player animator에 shot animation을 추가해준다.
2. Player 객체 밑에 있는 무기의 Weapon 에가서 inspector에 올바른 타입을 잡아준다.
3. Weapon.cs 에 변수추가
4. Player 객체 내에 빈객체를 추가하여준다.(Bullet Pos)
   1. position 1 2.5 1.75
5. Weapon Pos > Weapon HandGun > Case Pos라는 객체를만들어준다.
   1. position 0 0.8 -0.1
6. SubMachineGun에도 같은 원리로 Case Pos를 만들어준다.
   1. position 0 0.7 -0.2
7. 이제 inspector를 통해 bullet.cs의 변수를 넣어 줘야 한다.
   1. player 밑에 있는 Bullet pos를 연결해주고
   2. Case Pos는 각각연결해주며
   3. prefab은 우리 Assets/prefab의 있는것을 사용한다.
8. 벽에 Wall tag를 넣어준다.

#### 재장전 구현

1. Weapon.cs 변수추가
2. animator state 연결과 파라미터 추가
3. reload 제어
4. player내에 inspector에서 기본 maxAmmo와 curAmmo 조정

#### 마우스 회전

1. player.cs 에 `public Camera followCamera` 변수 추가후 main카메라를 넣어준다.
2. _Camera.ScreenPointToRay() : 스크린에서 월드로 Ray를 쏘는 함수_
3. _out : return처럼 반환값을 주어진 변수에 저장하는 키워드_
4. _RayCast 관련 짧은 예제_

```cs
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
```

Bullet.cs

```cs
public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Floor")){
            Destroy(gameObject,3);
        }else if(other.gameObject.CompareTag("Wall")){
            Destroy(gameObject);
        }
    }
}
```

Player.cs

```cs
public Camera followCamera;
bool rDown; // ReloadDown의 약자
bool isReload; // 재장전중인지

bool isFireReady = true; // (공격 가능한 상태)
void Update()
{
	//...
	Reload();
}

void GetInput(){
//...
	fDown = Input.GetButton("Fire1"); // 기본적인 마우스 좌클릭
	rDown = Input.GetButtonDown("Reload");
}

void Move(){
	moveVec = new Vector3(hAxis,0,vAxis).normalized;

	if(isDodge) moveVec = dodgeVec;

	if(isSwap || isReload || !isFireReady){ 
		moveVec = Vector3.zero;
	}
	//...
}

void Turn(){        // #1 키보드에 의한 회전
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
```

### 3D 쿼터뷰 액션게임 - 플레이어 물리문제 고치기 [B45]

#### 자동회전 방지

1. _Rigidbody.angularVelocity : 물리 회전속도_
2. FreezeRotation 메서드 생성 후 FixedUpdate에서 호출

#### 충돌 레이어 설정

1. 레이어 정의
   1. Floor 8
   2. Player 9
   3. PlayerBullet 10
   4. BulletCase 11
   5. Wall 12
2. 각 객체에 레이어 넣어주기
3. edit > project settings > physics 아래에
   1. layer collision matrix
4. BulletCase는 Floor와 BulletCase랑만 상호작용 (나머지 해제)
5. PlayerBullet은 Player와 비상호작용
6. 상호작용하지않게되면 collision event나 trigger등이 발동하지않음

#### 벽 관통 방지

1. `StopToWall()` 메서드 생성
2. isBorder로 Move 메서드 일부제어

#### 아이템 충돌 제거

1. Item.cs 수정
2. GetComponent로 컴포넌트를 가져오는데
3. Sphere collider가 2개잇으므로
4. 제일 위에있는것을 가져오게됨
5. inspector상의 radius가 0.5인 collider가 상단에오도록 조정
6. 9개의 아이템을 이렇게 만들어준다.

player.cs

```cs
bool isBorder;

void FreezeRotation(){
	rigid.angularVelocity =Vector3.zero;
}

void StopToWall(){
	Debug.DrawRay(transform.position,transform.forward *5,Color.green);
	isBorder = Physics.Raycast(transform.position,transform.forward,5,LayerMask.GetMask("Wall"));
}

void FixedUpdate()
{
	FreezeRotation();
	StopToWall();
}

void Move(){
	//...
	if(!isBorder)
		transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

	anim.SetBool("isRun", moveVec != Vector3.zero);
	anim.SetBool("isWalk", wDown);
}
```

Item.cs

```cs
Rigidbody rigid;
SphereCollider sphereCollider;

void Awake()
{
	rigid = GetComponent<Rigidbody>();
	sphereCollider = GetComponent<SphereCollider>();
}

void OnCollisionEnter(Collision other)
{
	if(other.gameObject.CompareTag("Floor")){
		rigid.isKinematic = true;
		sphereCollider.enabled = false;
	}
}
```

### 3D 쿼터뷰 액션게임 - 피격 테스터 만들기 [B46]

#### 오브젝트 생성

1. 하이라키에 Cube 추가 (Test Enemy)
   1. Scale 5 3 5
   2. rigidbody 추가
      1. freeze rotation x, z 체크

#### 충돌 이벤트

1. Bullet에
   1. Bullet tag를 추가
   2. collider에 is trigger가 되있는지 확인
2. Weapon에서 melee box collider가 활성화된시간을 0.3에서 0.5로 늘리기

#### 피격 로직

1. Material을 가져오는법
   1. Material은 meshrenderer가 가지고 있다.
   2. 이하의 방법을 사용해서 초기화
      ```cs
      Material mat; //
      void Awake()
      {
      	mat = GetComponent<MeshRenderer>().material;
      }
      ```
2. OnDamage 메서드 내에서 분기처리 및 render처리
3. layer 추가
   1. Enemy 13
   2. EnemyDead 14
4. 충돌 설정
5. Enemy는 모든 태그에 충돌
6. EnemyDead는 Floor와 Wall과 (EnemyDead)와만 충돌
7. Test Enemy의 layer를 Enemy로 설정

#### 후처리 로직

1. 벡터를 계산하여 코루틴 호출시 첨부를해주고
2. 코루틴내에서 addforce를 더하여준다.
   Enemy.cs

```cs
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    Rigidbody rigid; // awake에서 초기화
    BoxCollider boxCollider; // awake에서 초기화
    Material mat; // 태초의 mesh renderer가 가지고 있는 material

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
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
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;

        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if(curHealth > 0){
            rigid.AddForce(reactVec * Random.Range(0,2),ForceMode.Impulse);
            mat.color = Color.white;
        }else{
            mat.color = Color.gray;
            rigid.AddForce(reactVec * 5,ForceMode.Impulse);
            gameObject.layer = 14; //넘버 그대로
            Destroy(gameObject,4);
        }
    }
}
```

Bullets.cs

```cs
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Floor")){ // Bullet Case를 위한 로직
            Destroy(gameObject,3);
        }else if(other.gameObject.CompareTag("Wall")){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag){ // 탄알 제거 로직
            case "Floor":
            case "Wall":
                Destroy(gameObject);
                break;
        }
    }
```

### 3D 쿼터뷰 액션게임 - 수류탄 구현하기 [B47]

#### 오브젝트 생성

1. 새로운 grenade를 위해 받은 에셋 > prefab에서 하이라키로 grenade 드랍하여 생성(Throw Grenade)
2. grenade
   1. 받은에셋 > particles 에 grenade explosion를 greande 자식 객체로 넣어준다.
      1. 비활성화
   2. rigidbody와
   3. sphere collider 추가
      1. 반지름 0.7
      2. 새로운 physics material을 만들어서 추가한다.(Grenade)
         1. 1,1,1
3. Mesh Object에 trail renderer 컴포넌트 추가 5. material default-line 6. 좌하단 컬러 ac1bde 7. 60 FF7AA8
4. grenade layer player bullet
5. 이 완성된것을 prefab화 한다.
   1. 위치 초기화

#### 수류탄 투척

1. Player.cs 내에 grenadeObj를 만들어주고 연결

#### 수류탄 폭팔

1. Grenade.cs
2. 테스트

#### 수류탄 피격

1. _원형 다중 raycast 하는법_

```cs
	// 센터, 반지름, 쏘는방향 (무관), 구체를 위로보내는것이 아니기에 0, 레이어 마스크

	RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,15f,Vector3.up,0f,LayerMask.GetMask("Enemy"));
```

Grenade.cs

```cs
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
```

Enemy.cs

```cs
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
            if(isGrenade){
                reactVec += Vector3.up * 3;
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 3,ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15,ForceMode.Impulse);
            }else{
                rigid.AddForce(reactVec * 5,ForceMode.Impulse);
            }
            gameObject.layer = 14; //넘버 그대로
            Destroy(gameObject,4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos){
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec,true));
    }
```

Player.cs

```cs
    public GameObject grenadeObj;
    bool fDown2; // FireDown의 약자

    void Update()
    {
	    //...
        Grenade();
    }
    void GetInput(){
        fDown2 = Input.GetButtonDown("Fire2"); // 마우스 우클릭
    }
   
    void Grenade(){
        if(hasGrenades <= 0) return;
        if(fDown2 && !isReload && !isSwap){
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)){
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y=0;
                transform.LookAt(transform.position + nextVec);
                GameObject instantGrenade = Instantiate(grenadeObj,transform.position,transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();

                nextVec.y=10;
                rigidGrenade.AddForce(nextVec,ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back *10,ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
```

### 3D 쿼터뷰 액션게임 - 목표를 추적하는 AI 만들기 [B48]

#### 오브젝트 생성

1. 하이라키에 받은에셋 > prefab Enemy A를 넣어준다.
   1. rigidbody
      1. freeze roation x z
   2. box collider
      1. center 0 1.4 0
      2. size 2.5 2.5 2.5
   3. Enemy.cs
      1. maxHealth 50
      2. Materail은 이하의 객체에 잇으므로 GetComponentInchildren으로 변경
   4. 태그 Enemy layer Enemy

#### _Navigation AI_

1. Enemy A에 다음 컴포넌트 추가
   1. _NavMeshAgent : Navigation을 사용하는 인공지능 컴포넌트_
2. target 초기화
3. 테스트 / 에러
   1. NavAgent를 쓰기위해서는 NavMesh가 필요
4. _NavMesh : NavAgent가 경로를 그리기 위한 바탕 (Mesh)_
5. unity > window > ai > Navigation
   1. Bake 탭으로 이동
   2. 우하단 Bake 버튼 클릭
      1. 파랗게된 부분이 적을 따라다니는 데 필요한 바탕
   3. NavMesh는 Static 오브젝트만 Bake 가능

#### 애니메이션

1. Assets/Animations에 Enemy A.controller를 만들어준다.
   1. 이파일을 Enemy A아래있는 Mesh Object에 넣어준다.
2. 받은에셋 > model > enemy A 에서 모든 animation을 가져와준다.
3. 파라미터 isWalk, isAttack, doDie
4. any state > doDie
   1. 나가는시간 체크해제
   2. duration 0
5. 나머지 transition도 동일
   Enemy.cs

```cs
public Transform target;

public bool isChase;
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

void FixedUpdate()
{
	FreezeVelocity();
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
```

### 3D 쿼터뷰 액션게임 - 다양한 몬스터 만들기 [B49]

#### 플레이어 피격

1. Player.cs
2. 코루틴 OnDamage()와 bool isDamage를 만든다.
3. 코드
4. 하이라키에 빈객체 생성(EnemyBullet)
   1. box collider
      1. is collider 체크
   2. tag EnemyBullet
   3. layer EnemyBullet
   4. Bullet.cs
5. 테스트

#### 몬스터 이동 보정

1. Enemy.cs

#### 일반형 몬스터

1. EnemyBullet을 Enemy A내로 넣어준다.
   1. 근접공격 범위로 사용될 자리에 놓아준다.
   2. position 0 1.2 2.63
   3. box collider 컴포넌트 비활성화
2. `public BoxCollider meleeArea;`와 함께 할당
3. `public bool isAttack;` 선언
4. player 객체의 layer가 Player인지 확인

#### 돌격형 몬스터

1. 받은 에셋 > Prefab > Enemy B를 꺼내 하이라키에 넣어준다.
   1. Enemy A로부터 컴포넌트롤 copy pastes 해온다.
      1. Enemy.cs 에 Melee Area만 신경써서 별개의 객체를 복사해온후 연결해준다.
   2. Max Health는 80으로 준다.
   3. Mesh Object에 Animator를 만들어준다.
      1. Enemy A를 복사하여 Enemy B로 해준뒤 이것을 사용
2. Enemy A와 B에 차이를 주기 위해서
3. Nav Mesh Agent내의 속성값을 조정해준다.
4. A
   1. speed 10
   2. Angular Speed(회전속도) 360
   3. Acceleration(가속도) 30
5. B
   1. 10, 360, 50
6. 테스트 / 문제발견
   1. 공격범위 콜라이더의 tag와 layer가 EnemyBullet으로되어있어서 이값이 벽에 닿으면 사라지며 에러를 발생시킴

#### 원거리형 몬스터

1. Enemy C를 넣어주고 기본적인 설정을한다.
   1. 컴포넌트 애니메이션
   2. Box collider
      1. center 0 2 0
      2. size 3 3 3
   3. Nav Mesh Agent
      1. speed 5
      2. Angular Speed 480
      3. Acceleration 60
   4. Enemy.cs
      1. type c
      2. 200
2. 받은 에셋 > prefab에 missile을 하이라키에 놓고
3. mesh object
   1. pos의 높이를 3으로 조정
   2. rotation y 90
   3. Missile.cs
4. Missile 객체내에 Effect 객체추가(Effect)
   1. particle system
      1. pos 0 3 1
   2. Renderer - material 넣어주기
   3. Start lifetime 0.6
   4. Start speed 15
   5. start size 0.7
   6. emission - Rate over time 30
   7. shape - angle 14
   8. Radius - 0.5 (시작되는 반지름의크기)
   9. Color over lifetime
      1. ff8a00 ~ yellow
   10. Size over lifetime 점점 감소하는크기
5. Missile에
   1. rigidbody 추가
      1. use gravity 체크해제
   2. box collider 추가
      1. center 0 3 0
      2. size 1 1 1
      3. is trigger 체크
   3. Bullet.cs
      1. 15
   4. tag EnemyBullet, layer EnemyBullet
   5. prefab화
      1. z축(양의 방향)이 정면임
      2. Mesh object rotation y = -90
      3. effect의 shape의 rotation y 180
      4. position z 1> -1
6. Enemy C도 기존과 같이 복사해서 애니메이터를 넣어준뒤
   1. 각각의 animation을 바꿔준다.
   2. 대체품은 쿼드 에셋 > models > EnemyC에 있다.
7. EnemyC도 태그와 레이어를 설정해준다.
8. Enemy와 EnemyBullet이 충돌하지 않도록 설정해준다.
9. 점검 1. Enemy A,B,C먼저 태그와 레이어설정 2. 그후 Enemy A,B 이하의 EnemyBullet을 태그와 레이어를 변경해준다. 3. Missile도 태그와 레이어에 EnemyBullet를 맞춰준다.
   Missile.cs

```cs
public class Missile : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.right*30 * Time.deltaTime);
    }
}
```

Bullet.cs

```cs
	void OnTriggerEnter(Collider other)
	{
		switch(other.gameObject.tag){ // 탄 제거 로직
			case "Floor":
			case "Wall":
				if(!isMelee)
					Destroy(gameObject);
				break;
		}
	}
```

Enemy.cs

```cs
	public enum Type {A,B,C};

	public Type enemyType;
	public BoxCollider meleeArea;

	public GameObject bullet;
	public bool isAttack;

	void FixedUpdate()
	{
		FreezeVelocity();
		Targeting();// 가까이에 있는 물체 감지
	}

	void Targeting(){// 가까이에 있는 물체 감지
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
```

Player.cs

```cs
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Item")){ //Item먹었을때
			//...
		}else if(other.CompareTag("EnemyBullet") ) { // 피격을 위한 분기 처리 //|| other.CompareTag("Enemy")
			if(!isDamage){
				Bullet enemyBullet = other.GetComponent<Bullet>();
				health -= enemyBullet.damage;
				if(other.GetComponent<Rigidbody>() != null){
					Destroy(other.gameObject);
				}

				StartCoroutine(OnDamage());
			}
		}
	}

	IEnumerator OnDamage(){
		isDamage = true;
		foreach(MeshRenderer mesh in meshs){
			mesh.material.color = Color.yellow;
		}

		yield return new WaitForSeconds(1f);
		isDamage = false;
		foreach(MeshRenderer mesh in meshs){
			mesh.material.color = Color.white;
		}
	}
```

### 3D 쿼터뷰 액션게임 - 다양한 패턴을 구사하는 보스 만들기 [B50]

#### 보스 기본 세팅

1. 쿼드에셋 > prefab Enemy D를 하이라키에 드랍
2. Mesh Object에 Animator 추가
3. 쿼드에셋 > 모델 > EnemyD에서 필요한 state를 가져와준다.
4. 파라미터 doDie, doBigShot, doShot, doTaunt 사용
5. Enemy D
   1. Scale 3 3 3
      1. Mesh Object의 Scale 1 1 1
   2. rigidbody
      1. freeze rotation x,z
   3. box collider
      1. center 0 2 0
      2. size 3.7 3.7 3.7
   4. nav mesh agent
      1. speed 40
      2. angular Speed 0
      3. acceleration 60
6. Enemy D > 빈객체 추가 (Missile Port A)
   1. pos 2 2 1
   2. Missile Port B도 유사하게 만들어준다. (대칭)
7. Enemy D > 빈객체 추가(Melee Area)
   1. box collider
      1. center 0 0.5 0
      2. size 3.5 0.5 3.5
      3. is trigger 체크
      4. 비활성화
   2. Bullet.cs 추가
      1. 25 , is melee true
   3. 전체가 Enemy Enemy
   4. Melee Area만 EnemyBullet으로 수정

#### 투사체 만들기

1. 쿼드 에셋 > prefab에서 Missile Boss,Boss Rock를 하이라키에 드랍
2. Missile Boss > Mesh Object
   1. pos y 4
   2. scale 0.4
   3. rotation y 90
   4. Missile.cs추가
3. Missile Boss > Effect
   1. position 0 4 -1
   2. Particle system
   3. material - default-line
   4. shape에서 방향조정
      1. angle 10
      2. radius 0.6
   5. color over lifetime
   6. size over lifetime
   7. start lifetime 1
   8. start speed 15
   9. start size 0.9
   10. simulate space world
   11. Emission - rate over time 30
4. Missile Boss
   1. rigidbody 추가
      1. use gravity 비활성화
   2. box collider
      1. center 0 4 0
      2. size 2.5 2.5 3.5
      3. is trigger 체크
   3. 태그와 layer설정
   4. nav mesh agent
      1. speed 35
      2. angular speed 360
      3. acceleration 30
5. BossMissile.cs 에서 Bullet을 상속받아서 코드완성

#### 투사체(바위) 만들기

1. Boss Rock객체에
   1. rigidbody 추가
      1. mass 10
      2. angular drag (회전저항) 0
      3. freeze rotation y,z
   2. sphere collider
      1. center 0 5 0
      2. radius 4
   3. BossRock.cs
      1. damage 30
   4. tag와 layer 설정
2. bullet.cs에
   1. bool 변수 isRock추가
3. Boss Rock 객체에 2번째 sphere collider 추가
   1. 그전과 같은 크기로하고 is trigger체크
4. Prefab화
   1. 포지션 초기화

#### 보스 로직 준비

1. mat 을 MeshRenderer\[\] meshs로 변경
2. Enemy.cs 를 상속받는 Boss.cs생성3034
3. Enemy D로 와서 인스펙터에서 연결
   1. bullet은 BossRock 사용
4. 테스트
   1. boss가 플레이어 이동방향을 에측해서 그곳을 바라보면 ok

#### 패턴 로직

1. 문제 발생
2. Enemy에서 private인것은 상속되지않았음
3. 일부 필요한 값들은 public으로 전환
4. _주의 : Awake() 함수는 자식 스크립트만 단독 실행!_
   1. Enemy.cs의 Awake는 Boss.cs에서 실행되지 않음
   2. Boss쪽 awake에 앞코드를 복사해서 넣어준다.
5. 테스트

#### 로직 점검

1. player가 피격무적상태중이더라도 투사체는 삭제되어야한다.
2. Boss Melee Area
   Bullet.cs

```cs
	public bool isRock;
	void OnCollisionEnter(Collision other)
	{
		if(!isRock && other.gameObject.CompareTag("Floor")){ // Bullet Case를 위한 로직
			Destroy(gameObject,3);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		switch(other.gameObject.tag){ // 탄dkf 제거 로직
			// case "Floor":
			case "Wall":
				if(!isMelee)
					Destroy(gameObject);
				break;
		}
	}
```

Enemy.cs

```cs
	public enum Type { A, B, C, D };
	public bool isDead;
	public Rigidbody rigid; // awake에서 초기화
	public BoxCollider boxCollider; // awake에서 초기화
	public MeshRenderer[] meshs; // 태초의 mesh renderer가 가지고 있는 material
	public NavMeshAgent nav;
	public Animator anim;

	void Awake()
	{
		//...
		meshs = GetComponentsInChildren<MeshRenderer>();

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

	IEnumerator ChangeColor(Color color){
		foreach(MeshRenderer mesh in meshs){
			mesh.material.color = color;
		}

		yield break;
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

			if(isGrenade){
				reactVec += Vector3.up * 3;
				rigid.freezeRotation = false;

				rigid.AddForce(reactVec * 3,ForceMode.Impulse);
				rigid.AddTorque(reactVec * 15,ForceMode.Impulse);
			}else{
				rigid.AddForce(reactVec * 5,ForceMode.Impulse);
			}

			if(enemyType != Type.D)
				Destroy(gameObject,4);
		}
	}
```

Player.cs

```cs
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Item")){ //Item먹었을때
			//...
		}else if(other.CompareTag("EnemyBullet") ) { // 피격을 위한 분기 처리 //|| other.CompareTag("Enemy")
			if(!isDamage){
				Bullet enemyBullet = other.GetComponent<Bullet>();
				health -= enemyBullet.damage;

				bool isBossAtk = other.name == "Boss Melee Area";
				StartCoroutine(OnDamage(isBossAtk));
			}

			if(other.GetComponent<Rigidbody>() != null){
				Destroy(other.gameObject);
			}
		}
	}

	IEnumerator OnDamage(bool isBossAtk){
		isDamage = true;
		foreach(MeshRenderer mesh in meshs){
			mesh.material.color = Color.yellow;
		}

		if(isBossAtk){
			rigid.AddForce(transform.forward* -25,ForceMode.Impulse);
		}

		yield return new WaitForSeconds(1f);
		isDamage = false;
		foreach(MeshRenderer mesh in meshs){
			mesh.material.color = Color.white;
		}
		if(isBossAtk)
			rigid.velocity =Vector3.zero;
	}
```

Boss.cs

```cs
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
```

BossMissile.cs

```cs
public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav =GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }
}
```

BossRock.cs

```cs
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
```

### 3D 쿼터뷰 액션게임 - UI 배치하기 [B51]

#### 캔버스 세팅

1. Canvas 추가
2. EventSystem : UI에서 키입력을 결정해주는 컴포넌트
3. 인게임 - 월드 좌표계 , UI = 스크립 좌표계
4. 월드 좌표 - Transform, 스크린 좌표 - RectTransform
5. GameTab에서 16:9 설정을 해준다.
6. Canvas 컴포넌트 내에 Pixel Perfect를 체크해준다.
7. 어느 해상도에서도 동일한 크기를 만들기 위해서
   1. Canvas Scaler 의
   2. UI Scale Mode -> Scale With Screen Size
   3. 1920 1080

#### 타이틀 메뉴 UI

1. Canvas > Panel 추가 (Menu Panel)
   1. color 383838 알파 100
2. Menu Panel > Image(Title Image)
   1. 소스 이미지 title
   2. set native size
   3. pos y 200
3. Menu Panel > Image (Max Score Image)
   1. 소스 이미지 icon score mini
   2. pos x -170 pos y -150
4. Menu Panel > Text (Max Score Text)
   1. 가로 세로 0 0
   2. pos x 30 pos y -148
   3. 라벨 99999999
   4. 컬러 하얀색
   5. 폰트크기 130
   6. scale 0.5
   7. (쿠키런 볼드체)
5. Menu Panel > Button (Start Button)
   1. 소스 이미지 Panel A
   2. Image type simple
   3. set native size
   4. Image type sliced
   5. 가로 세로 400 150
   6. pos y -350
6. Menu Panel > Button > Text
   1. bottom 15
   2. 폰트 종류변경
   3. 라벨 Game Start
   4. 스케일 0.5
   5. 폰트 크기 130
   6. 중앙정렬 중앙정렬 overflow overflow
7. Menu Panel 비활성화
8. _폰트는 프로젝트에 많이 담으면 안좋음 (무거움)_

#### 인게임 주요 UI

1. Canvas > Panel 추가 (Game Panel)
   1. color 알파값 0
2. Game Panel > 빈 객체 추가(Score Group)
   1. 가로 세로 500 500
   1. 가로 세로 ００
   1. 앵커 좌상단
   1. pos x 20 y -20
3. Score Group > Image 추가 (Score Image)
   1. 소스 이미지 icon score mini
   2. 앵커 좌상단
4. Score Group > Text 추가 (Score Text)
   1. 폰트변경
   2. 스케일 0.5
   3. 폰트크기 100
   4. 라벨 99999999
   5. 좌측정렬 상단정렬
   6. pos x 33
   7. 색상 하얀색
   8. 앵커 좌상단
   9. pos x 60
5. 앵커 : UI 중심과 위치의 방향 기준
6. shift 키 : UI 중심, Alt 키 : UI 위치
7. Score Group(복사) (Status Group)
   1. 앵커 좌하단
   2. pos x 20 y 20
   3. 내부 자식객체들도 앵커 좌하단
   4. Text 위치 보정
   5. 자식을 2쌍더 복사(총3쌍이 되게)
8. 위 부터 icon heart mini, icon ammo mini, icon coin mini
   1. health text
      1. 라벨 100 / 100
   2. ammo text
      1. 라벨 - / 99
   3. coin text
      1. 라벨 1000
9. Score Group 복사 (Stage Group)
   1. 우상단 앵커
   2. Text 조정
      1. 라벨 STAGE 0
   3. 아이콘이 text의 좌측에 오게 끔 조정
   4. icon stage mini
   5. 한 쌍 복사
      1. icon time mini
      2. 라벨 00:00:00
10. Score Group 복사 (Enemy Group)
    1. 모두 복사하여 우하단 앵커 2. 라벨 x 0

#### 장비 UI

1. Game Panel > 빈객체 추가 (Equip Group)
   1. 가로세로 0 0
   2. 앵커 중앙하단
2. Equip Group > Image 추가 (Equip 1)
   1. 소스 이미지 Panel B
   2. set native size
   3. 앵커 중앙 하단
3. Equip Group > Image > Image (Weapon 1 Image)
   1. 소스 이미지 Icon Weapon Hammer
   2. set native size
4. Weapon 1 Image 복사 (Num Image)
   1. 소스 이미지 icon num1
   2. set native size
   3. 앵커 좌상단
   4. pos x 15 y 25
5. Equip Group > Image를 4개 추가(복사사용)
   1. Control L
      1. Weapon L Image
      2. Num Image
6. Equip Group
   1. scale 0.7 (optional)
   2. pos y 20

#### 보스 체력 UI

1. Game Panel > 빈객체 추가 (Boss Group)
   1. 가로 세로 0 0
2. Boss Group > Image 추가
   1. 소스 guage back
   2. Image type을 simple로 바꿔준후 set native size 후 다시 sliced로 변경
   3. 가로 600 높이 50
3. Boss Group > Image > Image 추가(Boss Health Image)
   1. 소스 guage front
   2. Image type을 simple로 바꿔준후 set native size 후 다시 sliced로 변경
   3. 가로 600 높이 50
4. Boss Group > Image > Image 추가
   1. 앵커 좌측
   2. 소스 icon Enemy D

### 3D 쿼터뷰 액션게임 - 간단한 상점 만들기 [B52]

#### 상점 꾸미기

1. 하이라키에 빈객체 추가 (Item Shop)
2. 진열대 Item Shop > cube
   1. position 0 1.5 0
   2. scale 5 2 3
   3. box collider 삭제
3. Assets/Materials 내에 material 생성 (ShopTable)
   1. albedo에 Asset/Textures에있는 \_Pattern을 넣어준다.
   2. ItemShop > Cube에 적용
4. 진열 상품을 Goods Group에 만들어서 넣어주기
   1. 각각을 scale을 작게조정하면 편하다
5. Goods Group도 Item Shop이하로 넣어주기
6. 쿼드에셋 > Prefab을 드랍 후 크기조절
7. 쿼드에셋 > Prefab npc luna도 드랍(Luna)
   1. mesh object에 animator 추가(Luna)
8. Item Shop > 빈객체 추가 (Zone)
   1. Particle System
      1. shape cone > donut
      2. rotation x 90
      3. start speed 0
      4. start lifetime 1
      5. start size 0.5
      6. shape
         1. radius 2.5
         2. donut
         3. radius 0
         4. mode - loop
      7. emission
         1. rate over time 60
      8. color over lifetime
         1. 좌하단 ff5d7d~
         2. 52.4 ffe781
   2. sphere collider 추가
      1. is trigger 체크
   3. Shop tag 추가
   4. Shop.cs 추가
9. Item Shop > 빈객체 (Spawn Pos A,B,C)
10. Item Shop 복사 (Weapon Shop)
    1. Luna 대신 Ludo로 교체
    2. Ludo에도 animator를 넣어준다.
11. 쿼드 에셋 > prefab에서 table을 위를 다른 아이템으로 채워준다.
12. Particle 색상을 blue계열로 변경

#### UI 구축하기

1. Canvas > Game Panel > Item Shop Group
   1. pos y -1000
   2. 가로 1000 높이 500
   3. 이미지 컴포넌트 추가
      1. 소스 이미지 Panel A
      2. Color ea7493
2. Item Shop Group > Button (Item button A)
   1. 가로 세로 240 380
3. Item button A > Text (Name Text)
   1. 스케일 0.5
   2. 폰트크기 80
   3. 중앙정렬 overflow
   4. top bottom 30 -30
   5. 라벨 체 력
4. Name Text 복사 (Price Text)
   1. left 50
   2. top bottom 115 -115
   3. 라벨 999,999
   4. 컬러 오렌지
5. Item button A > Image (Item Image)
   1. 소스 이미지 icon heart
   2. pos y 90
   3. 가로 세로 180 180
6. Item button A > Image (Coin Image)
   1. 소스 이미지 icon coin mini
7. Item button A
   1. 앵커 상단 중앙
   2. pos y -30
8. Item Button B,C도 만들어 준다.
9. Item Shop Group > Button 추가 (Exit Button)
   1. 이하 텍스트 삭제
   2. 위치는 좌상단 예정
   3. 소스 이미지 icon close
   4. set native size
   5. Transition에서 color를 지정해준다.
10. Item Shop Group > Image 추가 (Portarit Image)
    1. icon luna
    2. set native size
    3. 구석으로 이동후 30 25
11. Item Shop Group > Text 추가(Talk Text)
    1. 앵커 중앙하단
    2. 폰트 70
    3. 컬러 white
12. Item Shop Group 복사 (Weapon Shop Group)
    1. 캐릭터 ludo로 변경
    2. color blue계열로 변경

#### 상점 출입

1. 각 Shop 객체 내에 Zone 객체에 shop.cs를 넣어준다.
2. Player.cs ontriggerstay와 ontriggerexit 에서 처리
3. ui창에서 닫기버튼 누를때도 처리할수 잇게
4. on Click
   1. 객체 zone
   2. 메서드 Shop.Exit
5. 테스트 / 미리 UI 객체 활성화 해두기

#### 아이템 구입

1. Shop.cs
2. 각각의 배열을 inspector 내에서 초기화
3. Assets/Prefabs에서 가져다가 객체를 넣어줘야 한다.
4. shop.cs에서 buy 메서드구현
5. Shop roup내에 버튼이 click 됐을때 작동으로 연결
6. 테스트 / 정상

#### 액션 제한

1. player.cs 내에 `bool isShop` 변수를 추가해서 제어해준다.
   Shop.cs

```cs
public class Shop : MonoBehaviour
{
	public RectTransform uiGroup;
	public Animator anim;
	public GameObject[] itemObj;
	public int[] itemPrice;
	public Transform[] itemPos;
	public string[] talkData;
	public Text talkText;

	Player enterPlayer;

	public void Enter(Player player){
		enterPlayer = player;
		uiGroup.anchoredPosition = Vector3.zero;
	}

	public void Exit(){
		anim.SetTrigger("doHello");
		uiGroup.anchoredPosition = Vector3.down * 1000;
	}

	public void Buy(int index){
		int price = itemPrice[index];
		if(price > enterPlayer.coin){
			StopCoroutine(Talk());
			StartCoroutine(Talk());
			return;
		}

		enterPlayer.coin -= price;

		Vector3 ranVec = Vector3.right * Random.Range(-3,3) + Vector3.forward * Random.Range(-3,3);
		Instantiate(itemObj[index],itemPos[index].position + ranVec, itemPos[index].rotation);
	}

	IEnumerator Talk(){
		talkText.text =talkData[1];
		yield return new WaitForSeconds(2f);
		talkText.text =talkData[0];
	}
}
```

Player.cs

```cs
	bool isShop;
	void Interaction(){
		// 상호작용의 조건
		if(iDown && nearObject !=null && !isJump && !isDodge){
			// 상호작용하는 물체의 태그에 다른 분기처리
			if(nearObject.CompareTag("Weapon")){
				//...
			}else if(nearObject.CompareTag("Shop")){
				Shop shop = nearObject.GetComponent<Shop>();
				shop.Enter(this);
				isShop=true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Weapon"))
			nearObject = null;
		else if( other.CompareTag("Shop")){
			Shop shop = nearObject.GetComponent<Shop>();
			shop.Exit();
			nearObject = null;
			isShop=false;
		}
	}
	// 이 이외에 isShop 변수를 사용하여 다른 method들을 제어해준다.
```

###
