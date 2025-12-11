using UnityEngine;

public class Player : Entity
{
    [Header("Components")]
    public ParticleSystem Dust { get; private set; }
    public Player_Condition Condition { get; private set; }
    public Player_Stats Stats { get; private set; }
    public CapsuleCollider2D Collider { get; private set; }
    public GameObject PlayerObject;
    
    // State Machine
    public Player_StateMachine StateMachine { get; private set; }
    public Player_IdleState IdleState { get; private set; }
    public Player_WalkState WalkState { get; private set; }
    public Player_RunState RunState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DeathState DeathState { get; private set; }
    public Player_CrouchIdleState CrouchIdleState { get; private set; }
    public Player_CrouchMoveState CrouchMoveState { get; private set; }

    [Header("Movement Settings")] 
    public float CurrentSpeed { get; private set; }
    [Range(0, 1)] public float inAirMoveMultiplier = 0.7f;
    public int FacingDirection { get; private set; } = -1;
    private bool _isFacingRight = false;
    
    [Header("Crouch Settings")]
    public Vector2 CrouchColliderSize = new Vector2(0.8f, 0.9f);
    public Vector2 CrouchColliderOffset = new Vector2(0f, -0.45f);
    [HideInInspector] public Vector2 OriginalColliderSize;
    [HideInInspector] public Vector2 OriginalColliderOffset;
    
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float ceilingCheckRadius = 0.2f;

    [Header("Jump Settings & Timers")]
    public float JumpBufferTime = 0.2f;
    public float CoyoteTime = 0.2f; 
    public float CoyoteTimeCounter { get; set; }
    public float MaxFallSpeed = 20f; 
    public float JumpBufferCounter { get; set; }

    [Header("Collision Info")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.1f);
    [SerializeField] private LayerMask whatIsGround;
    public bool IsGroundDetected { get; private set; }
    public bool IsCeilingDetected { get; private set; }

    // --- [추가됨] 경사면(Slope) 관련 변수 시작 ---
    [Header("Slope Settings")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    private float slopeDownAngle;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerp;
    public bool IsOnSlope { get; private set; }
    public bool CanWalkOnSlope { get; private set; }
    // --- [추가됨] 경사면 관련 변수 끝 ---

    // Input Variables
    public float MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsJumpReleased { get; private set; }
    public bool IsSprintHeld { get; private set; }
    public bool IsCrouchHeld { get; private set; }

    private KeyCode _lastXKey = KeyCode.None;

    protected override void Awake()
    {
        base.Awake();
        
        Dust = GetComponentInChildren<ParticleSystem>();
        Condition = GetComponent<Player_Condition>();
        Stats = GetComponent<Player_Stats>();
        Collider = GetComponent<CapsuleCollider2D>();
        
        if (Collider != null)
        {
            OriginalColliderSize = Collider.size;
            OriginalColliderOffset = Collider.offset;
        }

        StateMachine = new Player_StateMachine();

        IdleState = new Player_IdleState(this, StateMachine, "idle");
        WalkState = new Player_WalkState(this, StateMachine, "walk");
        RunState = new Player_RunState(this, StateMachine, "run");
        JumpState = new Player_JumpState(this, StateMachine, "jumpFall");
        FallState = new Player_FallState(this, StateMachine, "jumpFall");
        DeathState = new Player_DeathState(this, StateMachine, "death");
        CrouchIdleState = new Player_CrouchIdleState(this, StateMachine, "crouchIdle");
        CrouchMoveState = new Player_CrouchMoveState(this, StateMachine, "crouchMove");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (isknocked) return;

        HandleInput();
        UpdateJumpTimers();
        
        StateMachine.UpdateActiveState();
    }
    
    private void FixedUpdate()
    {
        CheckCollision();
        SlopeCheck(); // [추가됨] 물리 업데이트 시 경사면 체크 수행
        
        if (isknocked) return;
        
        StateMachine.FiexedUpdateActiveState();
    }

    private void HandleInput()
    {
        KeyCode leftKey = KeyManager.instance.GetKeyCodeByName("Move Left");
        KeyCode rightKey = KeyManager.instance.GetKeyCodeByName("Move Right");

        if (Input.GetKeyDown(leftKey)) _lastXKey = leftKey;
        if (Input.GetKeyDown(rightKey)) _lastXKey = rightKey;

        bool isLeftHeld = Input.GetKey(leftKey);
        bool isRightHeld = Input.GetKey(rightKey);

        MoveInput = 0;
        if (isLeftHeld && isRightHeld)
            MoveInput = (_lastXKey == leftKey) ? -1 : 1;
        else if (isLeftHeld)
            MoveInput = -1;
        else if (isRightHeld)
            MoveInput = 1;

        IsJumpPressed = Input.GetKeyDown(KeyManager.instance.GetKeyCodeByName("Jump"));
        IsJumpReleased = Input.GetKeyUp(KeyManager.instance.GetKeyCodeByName("Jump"));
        IsSprintHeld = Input.GetKey(KeyManager.instance.GetKeyCodeByName("Sprint"));
        IsCrouchHeld = Input.GetKey(KeyManager.instance.GetKeyCodeByName("Crouch")); 
    }

    private void UpdateJumpTimers()
    {
        if (IsGroundDetected)
            CoyoteTimeCounter = CoyoteTime;
        else
            CoyoteTimeCounter -= Time.deltaTime;

        if (IsJumpPressed)
            JumpBufferCounter = JumpBufferTime;
        else
            JumpBufferCounter -= Time.deltaTime;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isknocked) return;
        
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        CheckAndFlip(xVelocity);
    }

    // --- [추가됨] 경사면 이동을 고려한 속도 설정 함수 ---
    public void SetGroundVelocity(float moveInput, float speed)
    {
        if (isknocked) return;

        CurrentSpeed = speed;
        float targetX = moveInput * speed;

        // 경사면에 있고, 걸을 수 있는 각도이며, 땅에 붙어있을 때
        if (IsGroundDetected && IsOnSlope && CanWalkOnSlope)
        {
            // 경사면 벡터에 맞춰 속도 계산 (수직항력의 수직 벡터 이용)
            // 원본 공식: newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            // 여기서 -moveInput을 곱하는 이유는 slopeNormalPerp의 방향과 입력 방향의 관계 때문입니다.
            float slopeX = speed * slopeNormalPerp.x * -moveInput;
            float slopeY = speed * slopeNormalPerp.y * -moveInput;
            
            rb.linearVelocity = new Vector2(slopeX, slopeY);
        }
        else
        {
            // 평지이거나 공중 등 일반적인 경우
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        }
        
        CheckAndFlip(targetX);
    }

    public void SetMoveSpeed(float speed) => CurrentSpeed = speed;

    private void CheckAndFlip(float xVelocity)
    {
        // 0.01f 같은 작은 값 노이즈 방지
        if (Mathf.Abs(xVelocity) > 0.1f)
        {
            if (xVelocity > 0 && !_isFacingRight) Flip();
            else if (xVelocity < 0 && _isFacingRight) Flip();
        }
    }

    private void Flip()
    {
        if (IsGroundDetected) Dust.Play();

        Vector2 currentScale = PlayerObject.transform.localScale;
        currentScale.x *= -1;
        PlayerObject.transform.localScale = currentScale;
        
        _isFacingRight = !_isFacingRight;
        FacingDirection *= -1;
    }

    private void CheckCollision()
    {
        // Ground Check (OverlapBox)
        if(groundCheck != null)
            IsGroundDetected = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, whatIsGround);
        
        if(ceilingCheck != null)
            IsCeilingDetected = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround);
    }

    // --- [추가됨] 경사면 체크 로직 (원본 PlayerController의 기능 이식) ---
    private void SlopeCheck()
    {
        // 콜라이더 하단 중앙 위치 계산 (Crouch 상태 등 고려하여 transform.position 대신 Collider bounds 사용 권장하거나 오프셋 조정)
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, Collider.size.y / 2 + Collider.offset.y));
        
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            IsOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {      
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            // 법선 벡터의 수직 벡터(경사면을 따라가는 벡터) 계산
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != 0) // 각도가 있으면 경사면으로 간주
            {
                IsOnSlope = true;
            }                       
            
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        // 경사 각도가 너무 가파른지 체크
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            CanWalkOnSlope = false;
        }
        else
        {
            CanWalkOnSlope = true;
        }

        // 마찰력 적용 (경사면에서 미끄러짐 방지)
        // 이동 입력이 없고, 경사면에 서 있고, 걸을 수 있는 각도라면 -> 마찰력 높임
        if (IsGroundDetected && IsOnSlope && CanWalkOnSlope && MoveInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
        
        if(ceilingCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }
}