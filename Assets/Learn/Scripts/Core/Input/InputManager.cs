using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// New Input System 기반 입력 처리 모듈.
/// IInputReader를 구현하여 다른 시스템이 이 클래스를 직접 참조하지 않아도 되도록 함.
/// 모듈형 프레임워크에서도 재사용 가능.
/// </summary>
public class InputManager : MonoBehaviour, IInputReader, PlayerControls.IPlayerActions
{
    private PlayerControls controls;

    // ============ 입력 상태 ============
    public Vector2 Move { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool InventoryToggled { get; private set; }
    public bool DialogueNextPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();


    // ============ 인풋 콜백 메서드 ============
    // 무브
    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // 상호작용
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractPressed = true;
    }

    // Invenry 토글
    public void OnInventoryToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
            InventoryToggled = true;
    }

    // Dialogue Next
    public void OnDialogueNext(InputAction.CallbackContext context)
    {
        if (context.performed)
            DialogueNextPressed = true;
    }

    // Attack
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackPressed = true;
    }

    // ============ 매 프레임 입력 상태 초기화 ============

    private void LateUpdate()
    {
        // 버튼 입력은 1프레임짜리 신호이므로 매 프레임 초기화
        InteractPressed = false;
        InventoryToggled = false;
        DialogueNextPressed = false;
        AttackPressed = false;
    }
}
