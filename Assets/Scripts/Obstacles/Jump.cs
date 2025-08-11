using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpVelocity = 10f;    // ���� �ӵ� 10f

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();    // �浹�� �÷��̾� ������Ʈ���� Rigidbody ������Ʈ ������
            if (playerRb != null)
            {
                Vector3 velocity = playerRb.velocity;       // ���� �ӵ� ����
                velocity.y = jumpVelocity;  // y�� �������� �����ӵ� ��
                playerRb.velocity = velocity;
            }
        }
    }
}
