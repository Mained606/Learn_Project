# Learn_Project

이 프로젝트는 Unity 6 기반으로 모듈식 게임 시스템 프레임워크를 구축하기 위한 학습용 프로젝트입니다.  
각 시스템은 다른 게임에서도 재사용 가능하도록 독립성, 확장성, 유지보수성을 중점적으로 적용하고 있습니다.

## 프로젝트 개요
- **개발 기간**: 2025.12.04 ~ 진행중
- **개발 인원**: 1명
- **개발 환경**:
  - Unity 6000.2.6f2
  - Rider IDE
  - Git / GitHub

## 프로젝트 구조
```
Assets/
└─ Learn/
   ├─ Scripts/
   │  ├─ Core/
   │  │  ├─ Events/
   │  │  │  └─ GameEventBus.cs
   │  │  ├─ Input/
   │  │  │  └─ PlayerControls.inputactions
   │  │  ├─ Interfaces/
   │  │  │  ├─ IGameEvent.cs
   │  │  │  ├─ IInputReader.cs
   │  │  │  └─ IInteractable.cs
   │  │  └─ Managers/
   │  │     ├─ InputManager.cs
   │  │     ├─ UIManager.cs
   │  │     ├─ ItemManager.cs
   │  │     ├─ IItemDropSpawner.cs
   │  │     └─ Singleton.cs
   │  ├─ Player/
   │  │  ├─ Controller/
   │  │  │  └─ PlayerController.cs
   │  │  ├─ Interaction/
   │  │  │  └─ PlayerInteractor.cs
   │  │  ├─ Items/
   │  │  │  ├─ EquipmentSlot.cs
   │  │  │  ├─ ItemActionResolver.cs
   │  │  │  ├─ ItemActionRunner.cs
   │  │  │  ├─ ItemData.cs
   │  │  │  ├─ ItemDefinition.cs
   │  │  │  ├─ ItemDefinitionDatabase.cs
   │  │  │  ├─ ItemPickup.cs
   │  │  │  ├─ ItemType.cs
   │  │  │  └─ PlayerItemCollector.cs
   │  │  ├─ Inventory/
   │  │  │  └─ PlayerInventory.cs
   │  │  ├─ Movement/
   │  │  │  └─ PlayerMover.cs
   │  │  ├─ StateMachine/
   │  │  │  ├─ PlayerIdleState.cs
   │  │  │  ├─ PlayerMoveState.cs
   │  │  │  ├─ PlayerState.cs
   │  │  │  └─ PlayerStateMachine.cs
   │  │  └─ Stats/
   │  │     └─ PlayerStats.cs
   │  ├─ Tests/
   │  │  └─ TestInteractable.cs
   │  └─ UI/
   │     ├─ Inventory/
   │     │  ├─ ContextMenu/
   │     │  │  ├─ ContextMenuAction.cs
   │     │  │  └─ ContextMenuButtonView.cs
   │     │  ├─ DragAndDrop/
   │     │  │  ├─ DragHandler.cs
   │     │  │  ├─ IDragSlot.cs
   │     │  │  ├─ IDragSource.cs
   │     │  │  └─ IDragDestination.cs
   │     │  ├─ InventoryContextMenu.cs
   │     │  ├─ InventoryDetailPanel.cs
   │     │  ├─ InventorySlotView.cs
   │     │  ├─ InventoryTooltip.cs
   │     │  └─ InventoryUI.cs
   │     └─ Interaction/
   │        └─ InteractionUI.cs
   └─ (기타 폴더: Prefabs / ScriptableObjects / Scenes ...)
```

## 주요 기능
- **입력 / 플레이어 제어**
  - New Input System 컨텍스트 전환(InputManager), 단발 이벤트 라우팅
  - PlayerController 오케스트레이션 → FSM(Idle/Move), Rigidbody 이동(PlayerMover)

- **상호작용**
  - Raycast 상호작용(PlayerInteractor) + Trigger 기반 아이템 수집(PlayerItemCollector)
  - 바닥 아이템(ItemPickup) 분리 설계

- **아이템 / 인벤토리**
  - 데이터 분리: ItemData(순수) / ItemDefinition(SO: heal/attackBonus/EquipmentSlot) / Database
  - 인벤토리: PlayerInventory(스택/병합/분할/슬롯), DragHandler+IDragSlot 병합/스왑 정책
  - 장비: EquipmentSlot별 단일 장착, 동일 부위 장착 시 자동 해제

- **아이템 액션 / 매니저**
  - ItemManager(싱글톤): 정의 조회, 드롭 스폰(기본 또는 IItemDropSpawner), 글로벌 이벤트
  - ItemActionRunner: 플레이어별 효과 적용/장착 상태 관리(Manager 통해 정의 조회)
  - ItemActionResolver: Consume/Equip/Unequip/Drop(HP 회복/공격력 보너스 적용)

- **UI / UX**
  - InventoryUI/SlotView: 데이터 바인딩, Runner 호출
  - 동적 컨텍스트 메뉴(Use/Equip/Unequip/Split/Drop), 툴팁/디테일 패널
  - Drag & Drop 프리뷰, 스택 병합/스왑 UX

## 세팅 가이드
- 씬에 `ItemManager` 배치 → DefinitionDatabase, DropPrefab(옵션), DropSpawnerBehaviour(옵션, IItemDropSpawner 구현) 할당
- 플레이어 : `ItemActionRunner`(Resolver), `PlayerInventory`, `PlayerStats`
- `InventoryUI` : SlotPrefab/ContentRoot/Canvas, ContextMenu/Tooltip/DetailPanel, ItemActionRunner 연결
- 컨텍스트 메뉴 : panel, buttonContainer, buttonPrefab(ContextMenuButtonView) 연결; Vertical Layout + Content Size Fitter 권장
- 드롭 프리팹 : `ItemPickup` 포함, `ItemManager` SpawnDrop 시 `Setup` 호출

## 확장 / TODO
- Definition Provider → Addressables/JSON/DB 교체 구현 (Provider 주입)
- DropSpawner 인터페이스 구현(풀링/Addressables)
- ItemManager 이벤트 → GameEventBus 포워딩/구독자 추가(퀘스트/알림/로그 등)
- 장비 모델/슬롯 시각화, 장비 보너스 시스템(StatsModifier) 확장
- 툴팁/컨텍스트/디테일 범용 Presenter/Provider 패턴 확장(스킬/퀵슬롯)
