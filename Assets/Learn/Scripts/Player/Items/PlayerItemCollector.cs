using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [Header("수집 로그 출력 여부")]
    [SerializeField] private bool logPickup = true;

    // 트리거 범위 내 현재 포커스된 바닥 아이템
    private ItemPickup currentPickup;

    private void OnTriggerEnter(Collider other)
    {
        // 트리거로 감지된 바닥 아이템만 포커스
        if (other.TryGetComponent(out ItemPickup pickup))
        {
            currentPickup = pickup;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (currentPickup == null) return;

        if (other.TryGetComponent(out ItemPickup pickup) && ReferenceEquals(pickup, currentPickup))
        {
            currentPickup = null;
        }
    }
    
    /// <summary>
    /// E 키 입력 시 호출. 트리거 안에 있는 바닥 아이템을 수동으로 수집.
    /// </summary>
    public bool TryPickup()
    {
        if (currentPickup == null) return false;

        currentPickup.Collect(gameObject);

        if (logPickup)
        {
            Debug.Log("[PlayerItemCollector] 트리거 아이템 수집 완료");
        }

        currentPickup = null;
        return true;
    }
}
