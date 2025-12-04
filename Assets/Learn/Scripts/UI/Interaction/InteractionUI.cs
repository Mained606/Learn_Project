using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private PlayerInteractor interactor;

    private void Update()
    {
        if (interactor.CurrentTarget != null)
        {
            promptText.text = interactor.CurrentTarget.InteractionPrompt;
            promptText.enabled = true;
        }
        else
        {
            promptText.enabled = false;
        }
    }
}