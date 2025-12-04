using FMOD.Studio;
using UnityEngine;
using Unity.Netcode;

public class Player : Entity
{
    public ParticleSystem Dust { get; private set; }
    
    public Player_IdleState IdleState { get; private set; }
    public Player_WalkState WalkState { get; private set; }
    public Player_RunState RunState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DeathState DeathState { get; private set; }

    private Player_StateMachine _playerStateMachine;
    public Player_Condition Condition { get; private set; }
    public Player_Stats Stats { get; private set; }
    public GameObject PlayerObject;

    [Header("Movement details")] 
    public float CurrentSpeed { get; private set; }

    [Range(0, 1)] public float inAirMoveMultiplier = .7f;
    private bool _isFacingRight = false;
    public int FacingDirection { get; private set; } = -1;

    private KeyCode _lastKey = KeyCode.None;
    public float MoveInput { get; private set; } = 0f;

    [Header("Collision detection")] 
    [SerializeField] private Transform groundCheck;

    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.1f);
    [SerializeField] private LayerMask whatIsGround;
    public bool IsGroundDetected { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        Dust = GetComponentInChildren<ParticleSystem>();
        Condition = GetComponent<Player_Condition>();
        Stats = GetComponent<Player_Stats>();

        _playerStateMachine = new Player_StateMachine();

        IdleState = new Player_IdleState(this, _playerStateMachine, "idle");
        WalkState = new Player_WalkState(this, _playerStateMachine, "walk");
        RunState = new Player_RunState(this, _playerStateMachine, "run");
        JumpState = new Player_JumpState(this, _playerStateMachine, "jumpFall");
        FallState = new Player_FallState(this, _playerStateMachine, "jumpFall");
        DeathState = new Player_DeathState(this, _playerStateMachine, "death");
    }

    private void Start()
    {
        _playerStateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (isknocked) return;
        ProcessKeyboardInput();
        
        _playerStateMachine.UpdateActiveState();
    }
    
    private void FixedUpdate()
    {
        HandleCollisionDetection();
        
        if (isknocked)
            return;

        _playerStateMachine.FiexedUpdateActiveState();
    }

    private void ProcessKeyboardInput()
    {
        KeyCode leftKey = KeyManager.instance.GetKeyCodeByName("Move Left");
        KeyCode rightKey = KeyManager.instance.GetKeyCodeByName("Move Right");

        // 마지막으로 누른 키 저장
        if (Input.GetKeyDown(leftKey)) _lastKey = leftKey;
        if (Input.GetKeyDown(rightKey)) _lastKey = rightKey;

        // 현재 눌려 있는 키 확인
        bool isLeftHeld = Input.GetKey(leftKey);
        bool isRightHeld = Input.GetKey(rightKey);

        MoveInput = 0;

        if (isLeftHeld && isRightHeld)
        {
            // 둘 다 눌린 경우는 마지막 누른 키 우선
            if (_lastKey == leftKey)
                MoveInput = -1;
            else if (_lastKey == rightKey)
                MoveInput = 1;
        }
        else if (isLeftHeld)
        {
            MoveInput = -1;
        }
        else if (isRightHeld)
        {
            MoveInput = 1;
        }
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isknocked)
            return;
        
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        CheckAndFlip(xVelocity);
    }

    public void CheckAndFlip(float xDirection)
    {
        if (xDirection > 0 && !_isFacingRight)
            Flip();
        else if (xDirection < 0 && _isFacingRight)
            Flip();
    }

    private void Flip()
    {
        if (IsGroundDetected)
            Dust.Play();

        Vector2 currentScale = PlayerObject.transform.localScale;
        currentScale.x *= -1;
        PlayerObject.transform.localScale = currentScale;
        _isFacingRight = !_isFacingRight;
        FacingDirection *= -1;
    }

    private void HandleCollisionDetection()
    {
        IsGroundDetected = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, whatIsGround);
    }

    public void SetMoveSpeed(float speed)
    {
        CurrentSpeed = speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}