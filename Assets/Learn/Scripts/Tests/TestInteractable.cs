using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Press E to Interact";

    public void Interact(GameObject interactor)
    {
        Debug.Log($"{interactor.name} 이(가) 상호작용 성공!");
    }
}