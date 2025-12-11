using UnityEngine;

public class Auto_chase_simulation : MonoBehaviour
{
    // public Transform player;          // 플레이어 Transform
    // private LineRenderer lineRenderer;
    //
    // void Start()
    // {
    //     lineRenderer = GetComponent<LineRenderer>();
    //     lineRenderer.positionCount = 2;
    // }
    //
    // void Update()
    // {
    //     if (player != null)
    //     {
    //         // 적 위치
    //         lineRenderer.SetPosition(0, transform.position);
    //
    //         // 플레이어 위치
    //         lineRenderer.SetPosition(1, player.position);
    //     }
    // }
    public Transform player;
    public LineRenderer lineRenderer;

    public int steps = 30;                 // 시뮬레이션 포인트 수
    public float timeStep = 0.1f;          // 시뮬레이션 간격
    public float moveSpeed = 3f;           // 적 이동 속도
    public float gravity = -9.8f;          // 중력

    void Start()
    {
        lineRenderer.positionCount = steps;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 startPos = transform.position;
        Vector2 dir = (player.position - transform.position).normalized;

        Vector2 velocity = dir * moveSpeed;   // 초기 이동 속도

        Vector2 currentPos = startPos;

        for (int i = 0; i < steps; i++)
        {
            // 현재 위치 저장
            lineRenderer.SetPosition(i, currentPos);

            // 위치 업데이트
            currentPos += velocity * timeStep;

            // 중력 적용
            velocity.y += gravity * timeStep;
        }
    }
}
