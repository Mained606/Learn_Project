using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 5f;

    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        this.moveInput = moveInput;
    }

    private void FixedUpdate()
    {
        // 물리 회전 제거
        rb.angularVelocity = Vector3.zero;

        // 입력을 저장해 둔 _moveInput 사용
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // 대각선 이동 시 속도 증가 방지 Normalize
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }

}