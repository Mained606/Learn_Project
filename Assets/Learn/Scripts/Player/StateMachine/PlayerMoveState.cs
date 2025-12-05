using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine machine, PlayerController controller)
        : base(machine, controller) { }

    public override void Enter()
    {
        // Move 진입 시 애니메이션 등 처리 가능
    }

    public override void Tick()
    {
        // 이동 입력이 사라지면 Idle로 전환
        if (controller.InputReader.Move.sqrMagnitude < 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void PhysicsTick()
    {
        // 이동 입력을 받아 물리 이동 처리
        Vector2 moveInput = controller.InputReader.Move;
        controller.PlayerMover.SetMoveInput(moveInput);
    }

    public override void Exit()
    {
        // Move에서 나갈 때는 이동을 즉시 멈추도록 강제 처리
        controller.PlayerMover.SetMoveInput(Vector2.zero);
    }
}