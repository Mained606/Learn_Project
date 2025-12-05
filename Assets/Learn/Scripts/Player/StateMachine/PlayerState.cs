using UnityEngine;

/// <summary>
/// 플레이어 상태 기본 클래스
/// </summary>
public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController controller;

    public PlayerState(PlayerStateMachine machine, PlayerController controller)
    {
        this.stateMachine = machine;
        this.controller = controller;
    }

    // 상태 진입 시 1회 호출
    public virtual void Enter() { }

    // 일반 프레임 업데이트
    public virtual void Tick() { }

    // FixedUpdate 기반 물리 업데이트
    public virtual void PhysicsTick() { }

    // 상태 종료 시 1회 호출
    public virtual void Exit() { }
}