using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private Collider _currentTarget;

    private void OnTriggerEnter(Collider other)
    {
        _currentTarget = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentTarget == other)
            _currentTarget = null;
    }

    public void TryInteract()
    {
        if (_currentTarget == null)
            return;

        // 대상에게 상호작용 가능한 컴포넌트가 있는지 확인
        if (_currentTarget.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact();
        }
    }
}