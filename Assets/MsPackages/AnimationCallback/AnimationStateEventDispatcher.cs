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
    /// AnimationState 이벤트 콜백 컨테이너
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

        public void Clear()
        {
            OnEnter = null;
            OnExit = null;
        }
    }

    /// <summary>
    /// 애니매이션 Key별 콜백 배열
    /// </summary>
    private readonly StateEventCallbacks[] _stateEventCallbacks = new StateEventCallbacks[(int)EAnimationStateKey.MaxCount];

    /// <summary>
    /// Key로 Callbacks 반환
    /// </summary>
    private StateEventCallbacks GetOrCreateCallbacks(EAnimationStateKey key)
    {
        int stateIndex = (int)key;

        if (_stateEventCallbacks[stateIndex] == null)
        {
            _stateEventCallbacks[stateIndex] = new StateEventCallbacks();
        }

        return _stateEventCallbacks[stateIndex];
    }

    /// <summary>
    /// 상태 진입시 콜백 추가
    /// </summary>
    public void RegisterOnStateEnter(EAnimationStateKey key, Action callback)
    {
        if (callback == null)
            return;

        GetOrCreateCallbacks(key).OnEnter += callback;
    }

    /// <summary>
    /// 상태 탈출시 콜백 추가
    /// </summary>
    public void RegisterOnStateExit(EAnimationStateKey key, Action callback)
    {
        if (callback == null)
            return;

        GetOrCreateCallbacks(key).OnExit += callback;
    }

    /// <summary>
    /// 콜백 전부 제거
    /// </summary>
    public void ClearAllCallbacks()
    {
        for (int i = 0; i < (int)EAnimationStateKey.MaxCount; i++)
        {
            _stateEventCallbacks[i].Clear();
        }
    }

    /// <summary>
    /// 상태 진입 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateListener.OnStateEnter(EAnimationStateKey key)
    {
        int stateIndex = (int)key;

        if (0 <= stateIndex && stateIndex < _stateEventCallbacks.Length)
        {
            _stateEventCallbacks[stateIndex]?.InvokeOnEnter();
        }
    }

    /// <summary>
    /// 상태 탈출 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateListener.OnStateExit(EAnimationStateKey key)
    {
        int stateIndex = (int)key;

        if (0 <= stateIndex && stateIndex < _stateEventCallbacks.Length)
        {
            _stateEventCallbacks[stateIndex]?.InvokeOnExit();
        }
    }
}
