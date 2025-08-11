using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpVelocity = 10f;    // 점프 속도 10f

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();    // 충돌한 플레이어 오브젝트에서 Rigidbody 컴포넌트 가져옴
            if (playerRb != null)
            {
                Vector3 velocity = playerRb.velocity;       // 현재 속도 저장
                velocity.y = jumpVelocity;  // y축 방향으로 점프속도 줌
                playerRb.velocity = velocity;
            }
        }
    }
}
