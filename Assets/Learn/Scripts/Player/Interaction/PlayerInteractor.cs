using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask interactLayer;

    private IInteractable currentTarget;

    public IInteractable CurrentTarget => currentTarget;

    private void Update()
    {
        ScanTarget();
    }

    private void ScanTarget()
    {
        currentTarget = null;

        Ray ray = new Ray(transform.position + Vector3.up * 0.8f, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentTarget = interactable;
            }
        }
    }

    public bool TryInteract()
    {
        if (currentTarget == null)
            return false;

        currentTarget.Interact(gameObject);
        return true;
    }
}