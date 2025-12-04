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
    public GameObject playerObject;

    [Header("Movement details")] 
    public float CurrentSpeed { get; private set; }
    public float JumpForce;

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
    
    [Header("Mining")]
    public bool CanMine = true;
    
    private NetworkVariable<Vector2> _networkPosition = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> _networkIsFacingRight = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private Vector2 _lerpStartPos;
    private Vector2 _lerpTargetPos;
    private float _lerpTime;
    
    [Header("Network Optimization")]
    [SerializeField] private float positionUpdateThreshold = 0.05f; // 이 거리 이상 움직여야 위치 전송
    [SerializeField] private float lerpDuration = 0.05f;
    private Vector2 _lastSentPosition;
    private bool _lastSentIsFacingRight;
    
    [Header("Laser Synchronization")]
    private NetworkVariable<bool> _isLaserActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> _laserEndPoint = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> _laserRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Vector2 _lerpLaserTargetPos;
    private float _lerpLaserTime;
    private bool _wasLaserActiveLastFrame = false; 

    [SerializeField] private bool IsTest = false;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Owner가 아닌 클라이언트에서는 물리 시뮬레이션을 비활성화하여
        // 네트워크로부터 받은 위치로만 움직이게 함
        if (!IsOwner && rb != null)
        {
            rb.isKinematic = true;
        }
        else if (IsOwner)
        {
            _lastSentPosition = transform.position;
            _lastSentIsFacingRight = _isFacingRight;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        Dust = GetComponentInChildren<ParticleSystem>();

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
        /*if (Condition.CheckIsDead())
        {
            _playerStateMachine.ChangeState(DeathState);
            return;
        }

        if (IsTest)
        {
            if (isknocked) return;
            ProcessKeyboardInput();
            _playerStateMachine.UpdateActiveState();
            return;
        }
        
        if (IsOwner)
        {
            if (ChatManager.instance.IsChatting || InventoryManager.instance.isInvenOpen || MissionManager.instance.IsMissionPanelOpen)
            {
                _playerStateMachine.ChangeState(IdleState);
                return;
            }
            
            if (!isknocked)
            {
                ProcessKeyboardInput();
                _playerStateMachine.UpdateActiveState();
            }

            UpdateNetworkVariablesIfNeeded();
        }
        else
        {
            // 네트워크 위치 값이 변경되면 새로운 보간 시작
            if (_lerpTargetPos != _networkPosition.Value)
            {
                _lerpStartPos = transform.position;
                _lerpTargetPos = _networkPosition.Value;
                _lerpTime = 0;
            }

            // 보간 진행
            if (_lerpTime < lerpDuration)
            {
                _lerpTime += Time.deltaTime;
                transform.position = Vector2.Lerp(_lerpStartPos, _lerpTargetPos, _lerpTime / lerpDuration);
            }
            else
            {
                transform.position = _lerpTargetPos; // 보간이 끝나면 목표 위치로 정확히 이동
            }
            
            // 바라보는 방향 동기화
            if (_isFacingRight != _networkIsFacingRight.Value)
            {
                FlipVisualsOnly();
            }
            
            // 네트워크로 받은 레이저 활성 상태(_isLaserActive)가 이전 프레임과 다를 경우에만 파티클을 제어합니다.
            if (_isLaserActive.Value != _wasLaserActiveLastFrame)
            {
                if (_isLaserActive.Value)
                {
                    // 레이저와 파티클을 켭니다.
                    Laser.EnableLaser();
                }
                else
                {
                    // 레이저와 파티클을 끕니다.
                    Laser.DisableLaser();
                }
                // 현재 상태를 '이전 상태'로 기억하여 다음 프레임에서 중복 호출을 방지합니다.
                _wasLaserActiveLastFrame = _isLaserActive.Value;
            }

            // 레이저가 활성화 상태일 때만 매 프레임 위치와 각도를 업데이트합니다.
            if (_isLaserActive.Value)
            {
                if (_lerpLaserTargetPos != _laserEndPoint.Value)
                {
                    _lerpLaserTargetPos = _laserEndPoint.Value;
                    _lerpLaserTime = 0;
                }
                
                Vector2 interpolatedEndPoint;
                if (_lerpLaserTime < lerpDuration)
                {
                    _lerpLaserTime += Time.deltaTime;
                    interpolatedEndPoint = Vector2.Lerp(Laser.lineRenderer.GetPosition(1), _lerpLaserTargetPos, _lerpLaserTime / lerpDuration);
                }
                else
                {
                    interpolatedEndPoint = _lerpLaserTargetPos;
                }
                
                // 파티클의 위치/회전도 이 메서드 안에서 함께 갱신됩니다.
                Laser.UpdateLaserVisuals(interpolatedEndPoint, _laserRotation.Value);
            }
        }*/
    }

    private void UpdateNetworkVariablesIfNeeded()
    {
        // 위치 동기화: 마지막으로 보낸 위치와 현재 위치의 거리가 threshold보다 클 때만 전송
        if (Vector2.Distance(_lastSentPosition, transform.position) > positionUpdateThreshold)
        {
            _networkPosition.Value = transform.position;
            _lastSentPosition = transform.position;
        }

        // 방향 동기화: 마지막으로 보낸 방향과 현재 방향이 다를 때만 전송
        if (_lastSentIsFacingRight != _isFacingRight)
        {
            _networkIsFacingRight.Value = _isFacingRight;
            _lastSentIsFacingRight = _isFacingRight;
        }
    }
    
    public void UpdateLaserState(bool isActive, Vector2 endPoint, Quaternion rotation)
    {
        if (!IsOwner) return;

        _isLaserActive.Value = isActive;
        if (isActive)
        {
            _laserEndPoint.Value = endPoint;
            _laserRotation.Value = rotation;
        }
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
    
    public void SetMiningAnimationByDirection(Vector2 aimDirection)
    {
        if (Anim == null) return;

        // 플레이어가 바라보는 방향 벡터를 구합니다.
        Vector2 forwardDirection = _isFacingRight ? Vector2.right : Vector2.left;

        // 플레이어의 정면을 기준으로 한 로컬 조준 각도를 계산합니다. (-180 ~ 180)
        float localAngle = Vector2.SignedAngle(forwardDirection, aimDirection);

        // 정면 180도(-90 ~ 90) 범위 외의 값은 클램핑하여 뒤로 채굴하는 애니메이션을 방지합니다.
        localAngle = Mathf.Clamp(localAngle, -90f, 90f);
        
        float adjustedAngle;
        
        // localAngle의 범위(-90 ~ 90)를 애니메이션 인덱스(0 ~ 18)로 변환합니다.
        // 1. 각도에 90을 더해 범위를 (0 ~ 180)으로 조정합니다.
        if (_isFacingRight)
        {
            // 오른쪽을 볼 때: 위(+90도)가 18번, 아래(-90도)가 0번이 되도록 계산
            adjustedAngle = localAngle + 90f;
        }
        else
        {
            // 왼쪽을 볼 때: 위(+90도)가 0번, 아래(-90도)가 18번이 되도록 순서를 반전시켜 계산
            adjustedAngle = -localAngle + 90f;
        }

        // 2. (0 ~ 180) 범위를 18개 구간으로 나눈 값(10)으로 나누어 인덱스를 계산합니다.
        // RoundToInt를 사용하여 각 구간의 중앙값을 기준으로 인덱스를 결정합니다.
        int animationIndex = Mathf.RoundToInt(adjustedAngle / 10f);

        // 3. 계산된 인덱스를 애니메이터의 "miningAngle" 파라미터에 전달합니다.
        Anim.SetFloat("miningAngle", animationIndex);
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
            PlayDustEffectServerRpc();

        Vector2 currentScale = playerObject.transform.localScale;
        currentScale.x *= -1;
        playerObject.transform.localScale = currentScale;
        _isFacingRight = !_isFacingRight;
        FacingDirection *= -1;
    }
    
    /// <summary>
    /// 텔레포트를 시작하는 공개 메서드입니다. Owner 클라이언트에서만 호출해야 합니다.
    /// </summary>
    /// <param name="newPosition">텔레포트할 목표 위치</param>
    public void Teleport(Vector2 newPosition)
    {
        // Owner가 아니면 텔레포트를 요청할 수 없습니다.
        if (!IsOwner) return;

        // 서버에 텔레포트를 요청합니다.
        TeleportServerRpc(newPosition);
    }

    /// <summary>
    /// 클라이언트의 요청을 받아 서버에서 실행됩니다.
    /// 서버가 모든 클라이언트에게 텔레포트를 명령합니다.
    /// </summary>
    [ServerRpc]
    private void TeleportServerRpc(Vector2 newPosition)
    {
        // 1. 서버의 네트워크 변수(_networkPosition)를 즉시 갱신합니다.
        //    이렇게 하면 나중에 접속하는 클라이언트도 올바른 위치에서 시작합니다.
        _networkPosition.Value = newPosition;

        // 2. 모든 클라이언트(요청한 클라이언트 포함)에게 텔레포트를 실행하라는 ClientRpc를 호출합니다.
        TeleportClientRpc(newPosition);
    }

    /// <summary>
    /// 서버의 명령을 받아 모든 클라이언트에서 실행됩니다.
    /// 플레이어의 위치를 즉시 변경하고 Lerp 관련 변수를 초기화합니다.
    /// </summary>
    [ClientRpc]
    private void TeleportClientRpc(Vector2 newPosition)
    {
        // 1. 물리적 위치를 즉시 목표 위치로 변경합니다.
        transform.position = newPosition;

        // 2. Lerp 로직을 무력화하기 위해 보간 시작점과 목표점을 모두 새로운 위치로 설정합니다.
        //    이렇게 하면 다음 Update 프레임에서 Lerp가 작동하더라도 (newPosition에서 newPosition으로) 이동하게 되어 제자리에 머물게 됩니다.
        _lerpStartPos = newPosition;
        _lerpTargetPos = newPosition;
    }
    
    // 클라이언트가 서버에게 파티클 재생을 요청하는 RPC입니다.
    [ServerRpc]
    private void PlayDustEffectServerRpc()
    {
        // 서버가 모든 클라이언트에게 파티클 재생을 명령합니다.
        PlayDustEffectClientRpc();
    }

    // 서버의 명령을 받아 모든 클라이언트에서 실행되는 RPC입니다.
    [ClientRpc]
    private void PlayDustEffectClientRpc()
    {
        // 모든 클라이언트에서 Dust 파티클을 재생합니다.
        if (Dust != null)
        {
            Dust.Play();
        }
    }
    
    // Non-Owner 클라이언트를 위한 시각적 Flip (파티클 재생 없음)
    private void FlipVisualsOnly()
    {
        _isFacingRight = _networkIsFacingRight.Value;
        FacingDirection = _isFacingRight ? 1 : -1;

        Vector3 currentScale = playerObject.transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * FacingDirection * -1; // 초기 방향에 맞게 조정
        playerObject.transform.localScale = currentScale;
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