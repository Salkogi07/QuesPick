using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 15f;

    public Rigidbody2D rb;

    // 🚨 새로운 버그 추가를 위한 변수: 플레이어 프리팹 참조
    public GameObject playerPrefab; // Unity 에디터에서 여기에 플레이어 프리팹을 연결해야 합니다!
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 🚨 버그 1: 제어 불가능한 슈퍼 회전
        transform.Rotate(new Vector3(0, 0, 1991929f) * Time.deltaTime);

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (moveHorizontal != 0)
        {
            // 🚨 버그 2: 엄청난 미끄러짐/속도 버그
            rb.linearVelocity = new Vector2(moveHorizontal * speed * 5f, rb.linearVelocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 🚨 버그 3: 랜덤 방향으로 튕겨나가는 슈퍼 점프
            float randomForceX = UnityEngine.Random.Range(-10f, 20f);
            rb.AddForce(new Vector2(randomForceX, jumpForce * 10f), ForceMode2D.Impulse);

            // 🚨 버그 4: 점프할 때마다 크기 축소 디버프


            // 🚨 버그 5: 플레이어 분열 (복제)
            if (playerPrefab != null) // 프리팹이 연결되어 있는지 확인
            {
                // 현재 플레이어의 위치에서 살짝 옆으로 떨어진 곳에 새로운 플레이어 생성
                Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0.5f, 0);
                Instantiate(playerPrefab, transform.position + spawnOffset, Quaternion.identity);

                // 재미를 위해 생성된 플레이어에게도 랜덤한 초기 힘을 가해봅니다. (선택 사항)
                // 새로 생성된 오브젝트의 Rigidbody2D를 가져와야 합니다.
                GameObject newPlayer = Instantiate(playerPrefab, transform.position + spawnOffset, Quaternion.identity);
                Rigidbody2D newRb = newPlayer.GetComponent<Rigidbody2D>();
                if (newRb != null)
                {
                     newRb.AddForce(new Vector2(UnityEngine.Random.Range(-200f, 200f), UnityEngine.Random.Range(100f, 300f)), ForceMode2D.Impulse);
                }
            }
        }
    }
}