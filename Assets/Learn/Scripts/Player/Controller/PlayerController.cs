using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public IInputReader InputReader { get; private set; }
    public PlayerMover PlayerMover { get; private set; }
    public PlayerInteractor PlayerInteractor { get; private set; }
    public PlayerItemCollector PlayerItemCollector { get; private set; }
    public PlayerStats PlayerStats { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        InputReader = InputManager.Instance;
        PlayerMover = GetComponent<PlayerMover>();
        PlayerInteractor = GetComponent<PlayerInteractor>();
        PlayerItemCollector = GetComponent<PlayerItemCollector>();
        PlayerStats = GetComponent<PlayerStats>();
        StateMachine = GetComponent<PlayerStateMachine>();

        if (InputReader == null)
            Debug.LogError("[PlayerController] InputManager Instance를 찾을 수 없습니다.");
        if (PlayerMover == null)
            Debug.LogError("[PlayerController] PlayerMover 컴포넌트가 없습니다.");
        if (PlayerInteractor == null)
            Debug.LogWarning("[PlayerController] PlayerInteractor 컴포넌트가 없습니다. 레이 기반 상호작용이 비활성화됩니다.");
        if (PlayerItemCollector == null)
            Debug.LogWarning("[PlayerController] PlayerItemCollector 컴포넌트가 없습니다. 트리거 기반 아이템 줍기가 비활성화됩니다.");
        if (StateMachine == null)
            Debug.LogError("[PlayerController] PlayerStateMachine 컴포넌트가 없습니다.");
    }

    private void OnEnable()
    {
        if (InputReader == null) return;

        InputReader.OnInteractEvent += HandleInteract;
        InputReader.OnInventoryToggleEvent += HandleInventoryToggle;
        InputReader.OnDialogueNextEvent += HandleDialogueNext;
        InputReader.OnAttackEvent += HandleAttack;
    }

    private void OnDisable()
    {
        if (InputReader == null) return;

        InputReader.OnInteractEvent -= HandleInteract;
        InputReader.OnInventoryToggleEvent -= HandleInventoryToggle;
        InputReader.OnDialogueNextEvent -= HandleDialogueNext;
        InputReader.OnAttackEvent -= HandleAttack;
    }

    private void Start()
    {
        if (StateMachine != null)
        {
            StateMachine.ChangeState(StateMachine.IdleState);
        }
    }

    /// <summary>
    /// E 키 입력:
    /// 1) 먼저 레이 기반 상호작용 시도 (NPC, 장치, 레이 위의 ItemPickup)
    /// 2) 실패 시 트리거 기반 아이템 줍기 시도 (발 주변 ItemPickup)
    /// </summary>
    private void HandleInteract()
    {
        bool interacted = false;

        if (PlayerInteractor != null)
        {
            interacted = PlayerInteractor.TryInteract();
        }

        // 레이 기반 상호작용에 실패한 경우에만 바닥 아이템 줍기 시도
        if (!interacted && PlayerItemCollector != null)
        {
            PlayerItemCollector.TryPickup();
        }
    }

    private void HandleInventoryToggle()
    {
        // 인풋 컨텍스트 전환만 먼저 처리
        InputManager.Instance.SetContext(InputContext.Inventory);

        // 나중에 인벤토리 상태로 전환 예정
        // StateMachine.ChangeState(StateMachine.InventoryState);
    }

    private void HandleDialogueNext()
    {
        if (InputManager.Instance.CurrentContext == InputContext.Dialogue)
        {
            // 나중에 DialogueState 에 메시지 전달 예정
            Debug.Log("[PlayerController] 다이얼로그 다음 대사 요청");
        }
    }

    private void HandleAttack()
    {
        // 나중에 CombatState 추가 후 처리
        Debug.Log("[PlayerController] 공격 입력 감지");
    }
}
