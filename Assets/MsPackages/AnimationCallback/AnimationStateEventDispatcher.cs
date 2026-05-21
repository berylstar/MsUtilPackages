using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation의 StateEventBehaviour에 콜백을 등록받고, 콜백 이벤트를 분배하는 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationStateEventDispatcher : MonoBehaviour, IAnimationStateListener
{
    /// <summary>
    /// Animation State Event 번들 클래스
    /// </summary>
    private class StateEventCallbacks
    {
        public event Action OnEnter;
        public event Action OnExit;

        public void InvokeOnEnter()
        {
            OnEnter?.Invoke();
        }

        public void InvokeOnExit()
        {
            OnExit?.Invoke();
        }
    }

    /// <summary>
    /// 애니매이션 Key별 콜백 딕셔너리
    /// </summary>
    private readonly Dictionary<EAnimationStateEventKey, StateEventCallbacks> _eventDict = new();

    private StateEventCallbacks GetOrCreateBundle(EAnimationStateEventKey key)
    {
        if (_eventDict.TryGetValue(key, out StateEventCallbacks holder) == false)
        {
            holder = new StateEventCallbacks();
            _eventDict.Add(key, holder);
        }

        return holder;
    }

    /// <summary>
    /// 상태 진입시 콜백 추가
    /// </summary>
    public void RegisterEnterCallback(EAnimationStateEventKey key, Action callback)
    {
        if (callback == null)
            return;

        GetOrCreateBundle(key).OnEnter += callback;
    }

    /// <summary>
    /// 상태 탈출시 콜백 추가
    /// </summary>
    public void RegisterExitCallback(EAnimationStateEventKey key, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        GetOrCreateBundle(key).OnExit += callback;
    }

    /// <summary>
    /// 콜백 전부 제거
    /// </summary>
    public void ClearAllCallbacks()
    {
        _eventDict.Clear();
    }

    /// <summary>
    /// 상태 진입 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateListener.OnStateEnter(EAnimationStateEventKey key)
    {
        if (_eventDict.TryGetValue(key, out StateEventCallbacks holder))
        {
            holder.InvokeOnEnter();
        }
    }

    /// <summary>
    /// 상태 탈출 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateListener.OnStateExit(EAnimationStateEventKey key)
    {
        if (_eventDict.TryGetValue(key, out StateEventCallbacks holder))
        {
            holder.InvokeOnExit();
        }
    }
}
