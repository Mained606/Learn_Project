public enum InputContext
{
    Gameplay,   // 일반 게임 플레이
    UI,         // 메뉴, 인벤토리 등 UI 상태
    Dialogue,   // 대화 중
    Inventory,  // 인벤토리 화면
    Locked      // 모든 입력 잠금 (컷신 등)
}