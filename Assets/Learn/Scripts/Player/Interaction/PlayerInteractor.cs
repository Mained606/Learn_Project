using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("레이 설정")]
    [SerializeField] private float interactDistance = 3f;      // 상호작용 가능 거리
    [SerializeField] private LayerMask interactLayerMask;      // 상호작용 대상 레이어 (NPC, 오브젝트, ItemPickup 등)

    [Header("디버그")]
    [SerializeField] private IInteractable currentTarget;

    public IInteractable CurrentTarget => currentTarget;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[PlayerInteractor] 메인 카메라를 찾을 수 없습니다. Ray 방향이 올바르지 않을 수 있습니다.");
        }
    }

    private void Update()
    {
        UpdateCurrentTarget();
    }
    
    /// 매 프레임 레이캐스트로 현재 바라보고 있는 IInteractable 을 갱신
    private void UpdateCurrentTarget()
    {
        currentTarget = null;

        // 플레이어 기준으로 레이 쏘기
        Vector3 origin = transform.position + Vector3.up * 0.2f;   // 플레이어 머리 근처
        Vector3 direction = transform.forward;                      // 플레이어가 바라보는 방향

        Debug.DrawRay(origin, direction * interactDistance, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactDistance, interactLayerMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentTarget = interactable;
            }
        }
    }
    
    // E 키 입력 시 호출 현재 타겟이 있으면 상호작용을 수행하고 true, 없으면 false 반환
    public bool TryInteract()
    {
        if (currentTarget == null) return false;

        currentTarget.Interact(gameObject);
        return true;
    }
}