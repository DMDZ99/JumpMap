using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]    // ����� �����ָ� Ÿ��Ʋ�� ����� ���� �������ϰ�
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;   // �����Է�(WASD)
    public LayerMask groundLayerMask;   // �ٴ� ���̾�

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;       // ī�޶� ����X�� ȸ����
    public float lookSensitivity;   // ���콺 �ΰ���
    private Vector2 mouseDelta;

    public bool canLook = true;

    public Action inventory;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // Ŀ�� ��带 ������ (�Ⱥ��̰�)
    }

    private void FixedUpdate()      // ���������ϴ� ���� FixedUpdate���� �ϴ°� ����.
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;    // ���� ��, WASD�� Vector2���̴�. �̰����� ������ ������ ���°� ���� Vector3��
        dir *= moveSpeed;               // �� ���⿡ moveSpeed ��ŭ ���ؼ� �̵�
        dir.y = _rigidbody.velocity.y;  // y �ʱ�ȭ

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);   // camCurXRot���� minXLook���� �۾����� minXLook ��ȯ,  maxXLook���� Ŀ����  maxXLook ��ȯ �ϴ� Clamp�Լ� ���
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);  // local�� �� ���� : �� ��ü���ƴ϶� �÷��̾��� ���� ���ư����� ��ȭȸ��

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // ���콺 �¿��̵� X �ΰ�����ŭ Y���� ȸ�� = ���� �������ִ�
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)       // Ű �������� Started = Ű�������� �ѹ���, Performed = �ڴ����� ���
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled) // Ű ������
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse); // Impulse : ���������� ���� Ȯ �� �� �ְ���
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]     // å��ٸ�ó�� 4��     ���ڸ���̳�
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) // �������� 0.1f��ŭ ���� �׶��尡 ������
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
