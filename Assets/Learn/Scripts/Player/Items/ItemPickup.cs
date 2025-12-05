using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("아이템 기본 정보")]
    [SerializeField] private string itemName;

    // 바닥 아이템임을 명확히 구분하기 위한 태그 강제
    private void Reset()
    {
        gameObject.tag = "Item";
    }

    // UI 프롬프트
    public string InteractionPrompt => $"{itemName} 줍기 [E]";

    public void Interact(GameObject interactor)
    {
        Pickup(interactor);
    }

    private void Pickup(GameObject interactor)
    {
        Debug.Log($"{interactor.name} 이(가) {itemName} 을(를) 획득");

        // 인벤토리 시스템이 준비되면 여기로 전달
        // interactor.GetComponent<PlayerInventory>()?.AddItem(itemData);

        Destroy(gameObject);
    }
}