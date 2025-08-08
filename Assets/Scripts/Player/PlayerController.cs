using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]    // 헤더를 적어주면 타이틀이 생기는 느낌 구분편하게
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;   // 현재입력(WASD)
    public LayerMask groundLayerMask;   // 바닥 레이어

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;       // 카메라 현재X축 회전값
    public float lookSensitivity;   // 마우스 민감도
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
        Cursor.lockState = CursorLockMode.Locked;   // 커서 모드를 락모드로 (안보이게)
    }

    private void FixedUpdate()      // 물리연산하는 것은 FixedUpdate에서 하는게 좋다.
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
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;    // 투입 즉, WASD는 Vector2값이다. 이걸통해 앞으로 옆으로 가는걸 만듬 Vector3로
        dir *= moveSpeed;               // 그 방향에 moveSpeed 만큼 곱해서 이동
        dir.y = _rigidbody.velocity.y;  // y 초기화

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);   // camCurXRot값이 minXLook보다 작아지면 minXLook 반환,  maxXLook보다 커지면  maxXLook 반환 하는 Clamp함수 사용
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);  // local을 쓴 이유 : 씬 전체가아니라 플레이어의 고개가 돌아가야함 상화회전

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // 마우스 좌우이동 X 민감도만큼 Y값을 회전 = 고개를 돌릴수있다
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)       // 키 눌렸을때 Started = 키눌렀을때 한번만, Performed = 꾹누르면 계속
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled) // 키 뗐을때
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
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse); // Impulse : 순간적으로 힘을 확 줄 수 있게함
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]     // 책상다리처럼 4개     십자모양이네
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) // 레이저를 0.1f만큼 쏴서 그라운드가 있으면
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
