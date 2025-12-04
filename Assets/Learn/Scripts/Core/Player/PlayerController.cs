using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IInputReader input;        // 입력 인터페이스
    private PlayerMover mover;         // 이동 처리
    private PlayerInteractor interactor; // 상호작용
    private PlayerItemCollector itemCollector; // 바닥 아이템 상호작용
    private PlayerStats stats;         // 스탯

    private void Awake()
    {
        input = FindFirstObjectByType<InputManager>(); // 싱글톤을 쓰지 않고 분리된 구조
        mover = GetComponent<PlayerMover>();
        interactor = GetComponent<PlayerInteractor>();
        itemCollector = GetComponent<PlayerItemCollector>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // 움직임 키 입력 인식
        mover.SetMoveInput(input.Move);

        // 상호작용(E)
        if (input.InteractPressed)
        {
            // 우선순위: Raycast → Item
            if (interactor.CurrentTarget != null)
                interactor.TryInteract();
            else
                itemCollector.TryPickup();
        }
    }
}