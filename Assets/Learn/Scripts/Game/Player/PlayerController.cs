using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IInputReader _input;        // 입력 인터페이스
    private PlayerMover _mover;         // 이동 처리
    private PlayerInteractor _interactor; // 상호작용
    private PlayerStats _stats;         // 스탯

    private void Awake()
    {
        _input = FindFirstObjectByType<InputManager>(); // 싱글톤을 쓰지 않고 분리된 구조
        _mover = GetComponent<PlayerMover>();
        _interactor = GetComponent<PlayerInteractor>();
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        _mover.SetMoveInput(_input.Move);

        if (_input.InteractPressed)
            _interactor.TryInteract();
    }
}