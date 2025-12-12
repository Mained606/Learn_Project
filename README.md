# Learn_Project

Unity 6 기반으로 “어느 게임에도 붙여 쓸 수 있는” 모듈식 게임 시스템 프레임워크를 만드는 학습용 프로젝트입니다.  
독립성·확장성·유지보수성을 최우선으로 설계했습니다.

## 프로젝트 개요
- **개발 기간**: 2025.12.04 ~ 진행중
- **개발 인원**: 1명
- **개발 환경**: Unity 6000.2.6f2, Rider IDE, Git/GitHub

## 프로젝트 구조
```
Assets/
└─ Learn/
   ├─ Scripts/
   │  ├─ Core/
   │  │  ├─ Events/              # GameEventBus
   │  │  ├─ Input/               # PlayerControls.inputactions
   │  │  ├─ Interfaces/          # IGameEvent, IInputReader, IInteractable
   │  │  └─ Managers/            # InputManager, UIManager, ItemManager, InventoryManager, IItemDropSpawner, Singleton
   │  ├─ Player/
   │  │  ├─ Controller/          # PlayerController
   │  │  ├─ Interaction/         # PlayerInteractor
   │  │  ├─ Items/               # EquipmentSlot, ItemActionResolver/Runner, ItemData/Definition/Database, ItemPickup, ItemType, PlayerItemCollector
   │  │  ├─ Inventory/           # PlayerInventory
   │  │  ├─ Movement/            # PlayerMover
   │  │  ├─ StateMachine/        # PlayerStateMachine, PlayerIdleState, PlayerMoveState, PlayerState
   │  │  └─ Stats/               # PlayerStats
   │  ├─ Tests/                  # TestInteractable
   │  └─ UI/
   │     ├─ Inventory/           # ContextMenu(동적 버튼), DragAndDrop, InventoryContextMenu/DetailPanel/SlotView/Tooltip/UI
   │     └─ Interaction/         # InteractionUI
   └─ (기타 폴더: Prefabs / ScriptableObjects / Scenes ...)
```

## 주요 기능 (설명 포함)
- **입력 / 플레이어 제어**
  - New Input System으로 입력 모드 전환(InputManager), 키 입력을 한 번 감지해 각 기능으로 전달
  - PlayerController는 입력을 받아 상태머신(Idle/Move)과 이동 모듈(PlayerMover)에게 넘겨주는 “중간 전달자” 역할

- **상호작용**
  - Raycast 상호작용(PlayerInteractor) + Trigger 기반 아이템 수집(PlayerItemCollector)
  - 바닥 아이템은 ItemPickup으로 분리 설계

- **아이템 / 인벤토리**
  - 데이터 분리: ItemData(순수 런타임) / ItemDefinition(SO: heal/attackBonus/EquipmentSlot) / ItemDefinitionDatabase(아이템 ID→Definition 매핑)
    - PlayerInventory와 UI는 Definition을 통해 아이콘/설명 등 클라이언트 리소스를 조회
  - 인벤토리: PlayerInventory가 스택/병합/분할/슬롯 관리, DragHandler+IDragSlot으로 병합/교환 처리
  - 장비: EquipmentSlot별로 한 개만 장착, 같은 부위에 새 장비 장착 시 기존 장비 자동 해제
  - InventoryManager: 여러 인벤토리 등록/조회, 인벤토리 간 스택 이동 뼈대 제공

- **아이템 액션 / 매니저**
  - ItemManager(싱글톤): 아이템 정의 조회, 드롭 스폰(기본 Instantiate 또는 IItemDropSpawner 구현), 글로벌 이벤트 발행
    - definitionProviderOverride 슬롯을 두어 ScriptableObject DB 외에 Addressables/JSON Provider로 교체 가능(비우면 기본 DB 사용)
  - ItemActionRunner: 플레이어별 아이템 효과 적용/장착 상태 추적(정의는 Manager에서 조회)
  - ItemActionResolver: Consume/Equip/Unequip/Drop 시 HP 회복, 공격력 보너스 등 적용

- **UI / UX**
  - InventoryUI/SlotView: 인벤토리 데이터를 슬롯에 바인딩하고 Runner에 액션 요청
  - 동적 컨텍스트 메뉴(Use/Equip/Unequip/Split/Drop), 툴팁/디테일 패널
  - Drag & Drop 프리뷰, 스택 병합/스왑 UX

## 세팅 가이드
- 씬에 `ItemManager` 배치 → DefinitionDatabase, DropPrefab(옵션), DropSpawnerBehaviour(옵션, IItemDropSpawner 구현) 할당
  - definitionProviderOverride는 비워두면 ScriptableObject DB 사용, Addressables/JSON Provider SO가 있으면 여기 연결
- 플레이어 : `ItemActionRunner`(Resolver), `PlayerInventory`, `PlayerStats`
- 다중 인벤토리 사용 시 `InventoryManager`에 식별자와 함께 인벤토리 등록(기본은 player 자동 등록 가능)
- `InventoryUI` : SlotPrefab/ContentRoot/Canvas, ContextMenu/Tooltip/DetailPanel, ItemActionRunner 연결
- 컨텍스트 메뉴 : panel, buttonContainer, buttonPrefab(ContextMenuButtonView) 연결; Vertical Layout + Content Size Fitter 권장
- 드롭 프리팹 : `ItemPickup` 포함, `ItemManager` SpawnDrop 시 `Setup` 호출

## 확장 / TODO
- Definition Provider → Addressables/JSON/DB 교체 구현 (Provider 주입)
- DropSpawner 인터페이스 구현(풀링/Addressables)
- ItemManager 이벤트 → GameEventBus 포워딩/구독자 추가(퀘스트/알림/로그 등)
- 장비 모델/슬롯 시각화, 장비 보너스 시스템(StatsModifier) 확장
- 툴팁/컨텍스트/디테일 범용 Presenter/Provider 패턴 확장(스킬/퀵슬롯)
