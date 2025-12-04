using UnityEngine;

public class Player : Entity
{
    [Header("Components")]
    public ParticleSystem Dust { get; private set; }
    public Player_Condition Condition { get; private set; }
    public Player_Stats Stats { get; private set; }
    public GameObject PlayerObject;
    
    // State Machine
    public Player_StateMachine StateMachine { get; private set; }
    public Player_IdleState IdleState { get; private set; }
    public Player_WalkState WalkState { get; private set; }
    public Player_RunState RunState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DeathState DeathState { get; private set; }

    [Header("Movement Settings")] 
    public float CurrentSpeed { get; private set; }
    [Range(0, 1)] public float inAirMoveMultiplier = 0.7f;
    public int FacingDirection { get; private set; } = -1;
    private bool _isFacingRight = false;

    [Header("Jump Settings & Timers")]
    public float JumpBufferTime = 0.2f;
    
    // [코요테 타임 설정]
    public float CoyoteTime = 0.2f; // 플랫폼에서 떨어진 뒤 점프 가능한 시간
    public float CoyoteTimeCounter { get; set; }

    // [낙하 속도 제한 설정]
    public float MaxFallSpeed = 20f; // 최대 낙하 속도

    public float JumpBufferCounter { get; set; }

    [Header("Collision Info")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.1f);
    [SerializeField] private LayerMask whatIsGround;
    public bool IsGroundDetected { get; private set; }

    // Input Variables
    public float MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsJumpReleased { get; private set; }
    public bool IsSprintHeld { get; private set; }

    private KeyCode _lastXKey = KeyCode.None;

    protected override void Awake()
    {
        base.Awake();
        
        Dust = GetComponentInChildren<ParticleSystem>();
        Condition = GetComponent<Player_Condition>();
        Stats = GetComponent<Player_Stats>();

        StateMachine = new Player_StateMachine();

        IdleState = new Player_IdleState(this, StateMachine, "idle");
        WalkState = new Player_WalkState(this, StateMachine, "walk");
        RunState = new Player_RunState(this, StateMachine, "run");
        JumpState = new Player_JumpState(this, StateMachine, "jumpFall");
        FallState = new Player_FallState(this, StateMachine, "jumpFall");
        DeathState = new Player_DeathState(this, StateMachine, "death");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (isknocked) return;

        HandleInput();
        UpdateJumpTimers(); // 타이머 갱신 (코요테 타임 계산)
        
        StateMachine.UpdateActiveState();
    }
    
    private void FixedUpdate()
    {
        CheckCollision();
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
    }

    // [코요테 타임 및 점프 버퍼 로직]
    private void UpdateJumpTimers()
    {
        // 땅에 있으면 코요테 타임 충전, 공중에 있으면 시간 감소
        if (IsGroundDetected)
            CoyoteTimeCounter = CoyoteTime;
        else
            CoyoteTimeCounter -= Time.deltaTime;

        // 점프 선입력(Buffer) 처리
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

    public void SetMoveSpeed(float speed) => CurrentSpeed = speed;

    private void CheckAndFlip(float xVelocity)
    {
        if (xVelocity > 0 && !_isFacingRight) Flip();
        else if (xVelocity < 0 && _isFacingRight) Flip();
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
        IsGroundDetected = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if(groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}