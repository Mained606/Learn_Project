using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>, IInputReader
{
    public Vector2 Move { get; private set; }

    public event Action OnInteractEvent;
    public event Action OnInventoryToggleEvent;
    public event Action OnDialogueNextEvent;
    public event Action OnAttackEvent;

    private PlayerControls controls;

    // 현재 입력 컨텍스트
    public InputContext CurrentContext { get; private set; } = InputContext.Gameplay;

    protected override void Awake()
    {
        base.Awake();
        
        controls = new PlayerControls();

        // Move = 상태 값
        controls.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => Move = Vector2.zero;


        // 단발 입력들
        // 인터랙트는 인벤토리 컨텍스트에서도 허용
        controls.Player.Interact.performed += ctx => TryFire(OnInteractEvent, false, true);
        // 인벤토리 토글은 컨텍스트와 무관하게 항상 허용
        controls.Player.InventoryToggle.performed += ctx =>
        {
            Debug.Log("[InputManager] InventoryToggle 입력 감지");
            TryFire(OnInventoryToggleEvent, true);
        };
        
        // 아직 키 바인딩 전이라 주석 처리함
        // controls.Player.DialogueNext.performed += ctx => TryFire(OnDialogueNextEvent);
        // controls.Player.Attack.performed += ctx => TryFire(OnAttackEvent);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // 입력 컨텍스트 변경
    public void SetContext(InputContext context)
    {
        CurrentContext = context;
    }

    // 컨텍스트에 따라 단발 입력 차단
    private void TryFire(System.Action action, bool ignoreContext = false, bool allowInventoryContext = false)
    {
        if (action == null) return;

        if (ignoreContext)
        {
            action?.Invoke();
            return;
        }

        // 입력 허용 조건
        switch (CurrentContext)
        {
            case InputContext.Gameplay:
                action?.Invoke();
                break;

            case InputContext.Inventory:
                if (allowInventoryContext)
                    action?.Invoke();
                break;

            case InputContext.UI:
            case InputContext.Dialogue:
            case InputContext.Locked:
                // 차단 -> 아무것도 실행 안 함
                break;
        }
    }
}
