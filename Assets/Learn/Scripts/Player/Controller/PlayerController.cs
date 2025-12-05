using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public IInputReader inputReader { get; private set; }
    public PlayerMover playerMover { get; private set; }
    public PlayerInteractor playerInteractor { get; private set; }
    public PlayerItemCollector playerItemCollector { get; private set; }
    public PlayerStats playerStats { get; private set; }

    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        inputReader = InputManager.Instance;
        playerMover = GetComponent<PlayerMover>();
        playerInteractor = GetComponent<PlayerInteractor>();
        playerItemCollector = GetComponent<PlayerItemCollector>();
        playerStats = GetComponent<PlayerStats>();

        stateMachine = GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        inputReader.OnInteractEvent += HandleInteract;
        inputReader.OnInventoryToggleEvent += HandleInventoryToggle;
        inputReader.OnDialogueNextEvent += HandleDialogueNext;
        inputReader.OnAttackEvent += HandleAttack;
    }

    private void OnDisable()
    {
        inputReader.OnInteractEvent -= HandleInteract;
        inputReader.OnInventoryToggleEvent -= HandleInventoryToggle;
        inputReader.OnDialogueNextEvent -= HandleDialogueNext;
        inputReader.OnAttackEvent -= HandleAttack;
    }

    private void Start()
    {
        stateMachine.ChangeState(new PlayerIdleState(stateMachine, this));
    }

    // -------------------------
    // 단발 입력 처리
    // -------------------------

    private void HandleInteract()
    {
        // 1) 먼저 레이 기반 상호작용을 시도한다.
        if (playerInteractor != null && playerInteractor.TryInteract())
            return; // 상호작용 성공 → 여기서 종료

        // 2) 레이 상호작용 실패 → 바닥 아이템 줍기 시도
        if (playerItemCollector != null)
            playerItemCollector.TryPickup();
    }


    private void HandleInventoryToggle()
    {
        InputManager.Instance.SetContext(InputContext.Inventory);

        // 인벤토리 상태로 전환
        // stateMachine.ChangeState(new PlayerInventoryState(stateMachine, this));
    }

    private void HandleDialogueNext()
    {
        // Dialogue 상태가 아닐 수도 있으므로 컨텍스트 확인
        if (InputManager.Instance.CurrentContext == InputContext.Dialogue)
        {
            // 대사 넘기기 처리
        }
    }

    private void HandleAttack()
    {
        // 나중에 CombatState 추가 예정
    }
}
