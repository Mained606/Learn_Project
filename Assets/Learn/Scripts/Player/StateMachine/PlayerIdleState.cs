using UnityEngine;

/// <summary>
/// 플레이어가 정지해 있는 상태
/// </summary>
public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine machine, PlayerController controller)
        : base(machine, controller) { }

    public override void Enter()
    {
        // Idle 진입 시 이동 입력 강제 초기화
        controller.playerMover.SetMoveInput(Vector2.zero);
    }

    public override void Tick()
    {
        // 이동 입력 발생하면 MoveState로 전환
        if (controller.inputReader.Move.sqrMagnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    public override void PhysicsTick()
    {
        // 고정 프레임에서도 안전하게 0으로 유지
        controller.playerMover.SetMoveInput(Vector2.zero);
    }

    public override void Exit()
    {
        // Idle → Move 혹은 다른 상태로 나갈 때
        // 특별한 처리 필요 없음
    }
}