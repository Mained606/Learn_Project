using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IInputReader inputReader;
    private PlayerMover playerMover;
    private PlayerInteractor playerInteractor;
    private PlayerStats playerStats;

    private void Awake()
    {
        inputReader = InputManager.Instance;
        if (inputReader == null)
        {
            Debug.LogError("[PlayerController] InputManager Instance를 찾을 수 없습니다.");
        }

        playerMover = GetComponent<PlayerMover>();
        if (playerMover == null)
        {
            Debug.LogError("[PlayerController] PlayerMover 컴포넌트가 없습니다.");
        }

        playerInteractor = GetComponent<PlayerInteractor>();
        if (playerInteractor == null)
        {
            Debug.LogWarning("[PlayerController] PlayerInteractor 컴포넌트가 없습니다. 상호작용 기능이 비활성화됩니다.");
        }

        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerController] PlayerStats 컴포넌트가 없습니다. 스탯 시스템은 선택 사항입니다.");
        }
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnInteractEvent += OnInteractInput;
            inputReader.OnInventoryToggleEvent += OnInventoryToggle;
            inputReader.OnDialogueNextEvent += OnDialogueNext;
            inputReader.OnAttackEvent += OnAttackInput;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnInteractEvent -= OnInteractInput;
            inputReader.OnInventoryToggleEvent -= OnInventoryToggle;
            inputReader.OnDialogueNextEvent -= OnDialogueNext;
            inputReader.OnAttackEvent -= OnAttackInput;
        }
    }

    private void Update()
    {
        if (playerMover != null && inputReader != null)
        {
            playerMover.SetMoveInput(inputReader.Move);
        }
    }

    private void OnInteractInput()
    {
        if (playerInteractor != null)
        {
            playerInteractor.TryInteract();
        }
    }

    private void OnInventoryToggle()
    {
        Debug.Log("[PlayerController] 인벤토리 토글 입력 감지됨");
    }

    private void OnDialogueNext()
    {
        Debug.Log("[PlayerController] 다이얼로그 다음 입력 감지됨");
    }

    private void OnAttackInput()
    {
        Debug.Log("[PlayerController] 공격 입력 감지됨");
    }
}
