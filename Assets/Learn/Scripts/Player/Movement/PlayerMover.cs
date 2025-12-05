using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    private Rigidbody rb;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    // 입력 시스템에서 전달받는 이동 입력 값
    private Vector2 moveInput;

    // 상태머신, 컷신, 대화 등에서 이동을 잠그고 싶을 때 사용
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // 상태머신(Idle/Move)에서 매 프레임 이동 입력을 갱신
    public void SetMoveInput(Vector2 moveInput)
    {
        this.moveInput = moveInput;
    }

    private void FixedUpdate()
    {
        // 이동이 잠긴 상태면 수평 속도만 0으로 고정
        if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        // 물리 회전 제거
        rb.angularVelocity = Vector3.zero;

        // 입력 벡터를 3D 월드 벡터로 변환 (XZ 평면 이동)
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // 대각선 이동 시 속도 증가 방지
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }
}