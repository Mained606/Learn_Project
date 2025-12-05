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

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        // 물리 회전 방지
        rb.angularVelocity = Vector3.zero;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // Unity6 물리 버그 대응
        if (moveInput == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
}