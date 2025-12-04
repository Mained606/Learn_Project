# Learn_Project

이 프로젝트는 Unity 6 기반으로 모듈식 게임 시스템 프레임워크를 구축하기 위한 학습용 프로젝트입니다.
각 시스템은 다른 게임에서도 재사용 가능하도록 독립성, 확장성, 유지보수성과 같은 요소들을 적용하고 있습니다.

## 프로젝트 개요
- **개발 기간**:
  - 2025.12.04 ~ 진행중

- **개발 인원** : 1명
- **개발 환경** :
  - Unity 6000.2.6f2
  - Rider IDE
  - Git / Github
 
## 주요 기능

**Core**
  - Input System
    - InputManager
    - IInputReader
    - PlayerControls (New Input System 기반)
  
  - Event System (시스템 간 직접 참조를 제거하기 위한 이벤트 메시징 구조)
    - GameEventBus
    - IGameEvent

**Player**
  - PlayerController
    - 입력 -> 이동 / 상호작용 / 스탯으로 전달하는 허브 역할
      
  - Movemen System
    - PlayerMover
    - Rigidbody 기반 이동 (추후 캐릭터 컨트롤러로 변경 예정)
    - FixedUpdate에서 물리 이동 처리
    
  - Stats System
    - PlayerStats
    - 체력 / 속도 등 추후 확장 가능한 구조
    
  - Interaction System (Raycast 기반 인식)
    - PlayerInteractor
    - IInteractable
    
  - Item Collector (Trigger 기반 인식 바닥 아이템 습득용)
    - PlayerItemCollector
    - ItemPickup

**Game System (추가 개발 예정)**
  - Inventory System
    - 슬롯 관리, 드래그 & 드롭, 소비 / 장착 시스템
    
  - Skill System
    - SO기반 데이터 + 런타임 객체 분리 구조
    - Active / Passive / Buff 공통 구조 설계
    
  - Quest System
    - 조건 검사기 / 보상 / 퀘스트 로그
    
  - Dialogue & NPC System
    - 선택형 대화 / NPC 상호작용 / 퀘스트 연결

## 프로젝트 구조
```
Assets/
└─ Learn/
    ├─ Scripts/
    │    ├─ Core/
    │    │     ├─ Input/
    │    │     │     ├─ InputManager.cs
    │    │     │     ├─ IInputReader.cs
    │    │     │     └─ PlayerControls.inputactions
    │    │     │
    │    │     ├─ Events/
    │    │     │     ├─ GameEventBus.cs
    │    │     │     └─ IGameEvent.cs
    │    │     │
    │    │     ├─ Utils/
    │    │     │     ├─ Extensions/
    │    │     │     └─ Helpers/
    │    │     │
    │    │     └─ Managers/
    │    │           ├─ GameManager.cs
    │    │           ├─ UIManager.cs
    │    │           └─ AudioManager.cs
    │    │
    │    ├─ Player/
    │    │     ├─ Controller/
    │    │     │     └─ PlayerController.cs
    │    │     │
    │    │     ├─ Movement/
    │    │     │     └─ PlayerMover.cs
    │    │     │
    │    │     ├─ Stats/
    │    │     │     └─ PlayerStats.cs
    │    │     │
    │    │     ├─ Interaction/
    │    │     │     ├─ PlayerInteractor.cs
    │    │     │     ├─ IInteractable.cs
    │    │     │     └─ TestInteractable.cs
    │    │     │
    │    │     ├─ Items/
    │    │     │     ├─ PlayerItemCollector.cs
    │    │     │     ├─ ItemPickup.cs
    │    │     │     └─ Runtime/
    │    │     │
    │    │     └─ Skills/
    │    │           ├─ Runtime/
    │    │           ├─ Effects/
    │    │           └─ Interfaces/
    │    │
    │    ├─ NPC/
    │    │     ├─ NPCController.cs
    │    │     ├─ NPCInteraction.cs
    │    │     └─ DialogueTrigger.cs
    │    │
    │    ├─ Inventory/
    │    │     ├─ UI/
    │    │     │     ├─ InventorySlot.cs
    │    │     │     ├─ SlotDragHandler.cs
    │    │     │     └─ InventoryPanel.cs
    │    │     │
    │    │     ├─ Systems/
    │    │     │     └─ InventorySystem.cs
    │    │     │
    │    │     └─ Data/
    │    │           └─ ItemData.cs
    │    │
    │    ├─ Dialogue/
    │    │     ├─ DialogueManager.cs
    │    │     ├─ DialogueUI.cs
    │    │     └─ DialogueNode.cs
    │    │
    │    ├─ Quest/
    │    │     ├─ QuestSystem.cs
    │    │     ├─ QuestCondition.cs
    │    │     └─ QuestData.cs
    │    │
    │    └─ UI/
    │          ├─ HUD/
    │          ├─ Windows/
    │          └─ Common/
    │
    ├─ ScriptableObjects/
    │    ├─ Items/
    │    ├─ Skills/
    │    ├─ Dialogue/
    │    ├─ Quests/
    │    └─ NPC/
    │
    ├─ Prefabs/
    │    ├─ Player/
    │    ├─ Items/
    │    ├─ NPC/
    │    ├─ UI/
    │    └─ World/
    │
    ├─ Materials/
    ├─ Animations/
    ├─ Audio/
    ├─ Textures/
    ├─ Scenes/
    └─ Tests/
```
