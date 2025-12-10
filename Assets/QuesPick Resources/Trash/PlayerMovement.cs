using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // --- 설정 가능한 변수 ---

    [Header("움직임 설정")]
    public float moveSpeed = 5f; // 좌우 이동 속도

    [Header("점프 설정")]
    public float jumpForce = 10f; // 점프 시 가해지는 힘
    public LayerMask groundLayer; // 지면으로 인식할 레이어 (Inspector에서 설정)
    public Transform groundCheck; // 지면 체크를 위한 Transform 위치
    public float checkRadius = 0.2f; // 지면 체크 범위 반경

    // --- 내부 변수 ---

    private Rigidbody2D rb; // Rigidbody2D 컴포넌트 참조
    private float moveInput; // 사용자의 좌우 입력 값 ( -1f ~ 1f )
    private bool isGrounded; // 현재 지면에 닿아있는지 여부

    // --- Unity 기본 함수 ---

    void Start()
    {
        // Rigidbody2D 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D 컴포넌트가 없으면 경고 메시지를 출력하고 스크립트를 비활성화합니다.
        if (rb == null)
        {
            Debug.LogError("PlayerMovement 스크립트는 Rigidbody2D 컴포넌트를 필요로 합니다.");
            enabled = false;
        }
    }

    void Update()
    {
        // 1. 좌우 입력 감지
        // Input.GetAxisRaw("Horizontal")은 기본적으로 A/D 키 또는 왼쪽/오른쪽 화살표 키 입력을 반환합니다.
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프 입력 감지
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // FixedUpdate는 물리 계산을 처리하기에 적합합니다.

        // 1. 지면 체크
        // Physics2D.OverlapCircle을 사용하여 groundCheck 위치 주변에 groundLayer가 있는지 확인합니다.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        

        // 2. 좌우 이동
        // 현재 Rigidbody2D의 수직 속도(rb.velocity.y)는 유지하고 수평 속도(rb.velocity.x)만 업데이트합니다.
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- 커스텀 함수 ---

    private void Jump()
    {
        // 수직 방향으로 힘을 가해 점프합니다.
        // 기존의 수직 속도를 무시하고 새로운 힘을 적용합니다.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // 또는 AddForce를 사용할 수도 있습니다:
        // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}