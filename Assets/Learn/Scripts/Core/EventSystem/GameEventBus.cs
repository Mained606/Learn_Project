using System;
using System.Collections.Generic;

/// <summary>
/// 게임 전역에서 사용하는 이벤트 버스.
/// 모든 시스템 간 통신은 이 클래스를 통해 이루어진다.
/// 재사용성과 디커플링(약한 결합)을 극대화하기 위한 구조.
/// </summary>
public static class GameEventBus
{
    // key: 이벤트 타입, value: 해당 이벤트 타입에 등록된 핸들러 목록
    private static readonly Dictionary<Type, Delegate> eventTable 
        = new Dictionary<Type, Delegate>();


    /// <summary>
    /// 이벤트 구독
    /// </summary>
    public static void Subscribe<T>(Action<T> handler)
        where T : struct, IGameEvent
    {
        if (handler == null) return;

        var eventType = typeof(T);

        if (eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            eventTable[eventType] = (Action<T>)existingDelegate + handler;
        }
        else
        {
            eventTable[eventType] = handler;
        }
    }


    /// <summary>
    /// 이벤트 구독 해제
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler)
        where T : struct, IGameEvent
    {
        if (handler == null) return;

        var eventType = typeof(T);

        if (eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            var current = (Action<T>)existingDelegate - handler;

            if (current == null)
                eventTable.Remove(eventType);
            else
                eventTable[eventType] = current;
        }
    }


    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public static void Publish<T>(T eventData)
        where T : struct, IGameEvent
    {
        var eventType = typeof(T);

        if (eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            // 기존 델리게이트를 캐스팅 후 호출
            ((Action<T>)existingDelegate)?.Invoke(eventData);
        }
    }


    /// <summary>
    /// 모든 이벤트 구독 정보 초기화
    /// (씬 전환 시 메모리 누수 방지를 위해 사용 가능)
    /// </summary>
    public static void ClearAll()
    {
        eventTable.Clear();
    }
}