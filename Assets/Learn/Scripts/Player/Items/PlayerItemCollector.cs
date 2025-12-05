using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    // 현재 트리거 범위 안에 있는 상호작용 가능 오브젝트(주로 ItemPickup)
    private IInteractable currentItem;

    private void OnTriggerEnter(Collider other)
    {
        // 트리거 영역에 들어온 오브젝트 중 IInteractable 을 가진 오브젝트만 대상
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentItem = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentItem == null) return;

        // 나가는 오브젝트가 현재 아이템과 같은 경우에만 해제
        if (other.TryGetComponent(out IInteractable interactable) && ReferenceEquals(interactable, currentItem))
        {
            currentItem = null;
        }
    }
    
    // 트리거 범위 안에 아이템이 있으면 E키 입력시 Interact 호출
    public void TryPickup()
    {
        if (currentItem == null) return;

        currentItem.Interact(gameObject);
        currentItem = null;
    }
}