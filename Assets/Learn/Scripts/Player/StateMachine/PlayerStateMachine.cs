using UnityEngine;

/// <summary>
/// 플레이어 상태를 관리하는 FSM
/// 모든 상태 객체는 미리 생성하여 재사용
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;

    [Header("디버그 - 현재 플레이어 상태")]
    [SerializeField] private string currentStateName;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        // 모든 상태 미리 생성하여 캐싱
        IdleState = new PlayerIdleState(this, controller);
        MoveState = new PlayerMoveState(this, controller);
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        currentState?.Exit();
        currentState = newState;

        // Inspector 디버그용
        currentStateName = newState.GetType().Name;

        currentState.Enter();
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsTick();
    }
}