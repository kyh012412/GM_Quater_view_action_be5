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

###
