using UnityEngine;

/// <summary>
/// 입력값을 읽어오는 인터페이스. 모듈 간 의존성을 최소화하기 위함.
/// 이 인터페이스 덕분에 InputManager를 다른 프로젝트에 그대로 복붙할 수 있음.
/// </summary>
public interface IInputReader
{
    Vector2 Move { get; }
    bool InteractPressed { get; }
    bool InventoryToggled { get; }
    bool DialogueNextPressed { get; }
    bool AttackPressed { get; }
}
