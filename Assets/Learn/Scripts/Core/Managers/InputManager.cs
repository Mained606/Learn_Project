using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Assets/Learn/Scripts/Core/Managers/InputManager.cs
public class InputManager : Singleton<InputManager>, IInputReader, PlayerControls.IPlayerActions
{
    // New Input System에서 자동 생성된 클래스
    private PlayerControls controls;

    // 이동 상태 값 (IInputReader 구현)
    public Vector2 Move { get; private set; }

    // 단발 입력 이벤트들 (IInputReader 구현)
    public event Action OnInteractEvent;
    public event Action OnInventoryToggleEvent;
    public event Action OnDialogueNextEvent;
    public event Action OnAttackEvent;

    protected override void Awake()
    {
        base.Awake();
        InitializeInput();
    }

    private void InitializeInput()
    {
        // 입력 액션 인스턴스 생성
        controls = new PlayerControls();

        // 콜백 등록 (자동 생성된 IPlayerActions 인터페이스 사용)
        controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            InitializeInput();
        }

        controls.Player.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
        }
    }

    #region New Input System 콜백 구현부 (절대 이름 변경 금지)

    // Move (벡터 상태)
    void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        HandleMove(context);
    }

    // Interact (단발)
    void PlayerControls.IPlayerActions.OnInteract(InputAction.CallbackContext context)
    {
        HandleInteract(context);
    }

    // // InventoryToggle (단발)
    // void PlayerControls.IPlayerActions.OnInventoryToggle(InputAction.CallbackContext context)
    // {
    //     HandleInventoryToggle(context);
    // }
    //
    // // DialogueNext (단발)
    // void PlayerControls.IPlayerActions.OnDialogueNext(InputAction.CallbackContext context)
    // {
    //     HandleDialogueNext(context);
    // }
    //
    // // Attack (단발)
    // void PlayerControls.IPlayerActions.OnAttack(InputAction.CallbackContext context)
    // {
    //     HandleAttack(context);
    // }

    #endregion

    #region 내부 핸들러 → 이벤트 3단계 구조

    // 1) Move는 performed/canceled 둘 다 상태 업데이트
    private void HandleMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            Move = Vector2.zero;
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        // 단발 입력은 performed 시점에서만 이벤트 발행
        if (!context.performed) return;
        OnInteractEvent?.Invoke();
    }

    private void HandleInventoryToggle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnInventoryToggleEvent?.Invoke();
    }

    private void HandleDialogueNext(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnDialogueNextEvent?.Invoke();
    }

    private void HandleAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OnAttackEvent?.Invoke();
    }

    #endregion
}
