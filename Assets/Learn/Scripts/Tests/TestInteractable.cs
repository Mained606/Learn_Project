using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Press E to Interact";

    public void Interact()
    {
        Debug.Log("상호작용 성공!");
    }
}