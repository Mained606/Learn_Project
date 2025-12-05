using UnityEngine;

/// <summary>
/// 상호작용 가능한 모든 오브젝트가 구현해야 하는 인터페이스
/// NPC, 오브젝트, 아이템 픽업 등
/// </summary>
public interface IInteractable
{
    // 상호작용 UI에 표시할 문구
    string InteractionPrompt { get; }

    // 상호작용 시도자를 넘겨줌
    void Interact(GameObject interactor);
}