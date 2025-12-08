using UnityEngine;

/// <summary>
/// UI 전역 매니저. 입력 이벤트를 구독해 UI 패널 토글을 처리한다.
/// 인벤토리 외 추가 패널도 여기서 일괄 관리 가능.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [Header("UI 패널")]
    [SerializeField] private GameObject inventoryPanel;
    private InputManager inputManager;

    private bool inventoryOpen;

    protected override void Awake()
    {
        base.Awake();
        if (inventoryPanel != null)
        {
            inventoryOpen = inventoryPanel.activeSelf;
        }
    }

    private void OnEnable()
    {
        inputManager = InputManager.Instance;
        if (inputManager != null)
            inputManager.OnInventoryToggleEvent += HandleInventoryToggle;
    }

    private void OnDisable()
    {
        if (inputManager != null)
            inputManager.OnInventoryToggleEvent -= HandleInventoryToggle;
    }

    private void HandleInventoryToggle()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(inventoryOpen);
        }

        inputManager?.SetContext(inventoryOpen ? InputContext.Inventory : InputContext.Gameplay);
    }
}
