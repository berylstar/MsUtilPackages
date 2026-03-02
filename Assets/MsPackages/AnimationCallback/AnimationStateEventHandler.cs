using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnimationStateEventBehaviour에 콜백을 등록하는 컴포넌트
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationStateEventHandler : MonoBehaviour, IAnimationStateEventCommander
{
    /// <summary>
    /// Animation State Event 번들 클래스
    /// </summary>
    private class AnimationStateEventBundle
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
    private readonly Dictionary<EAnimationStateEventKey, AnimationStateEventBundle> eventDict = new();

    private AnimationStateEventBundle GetOrCreateBundle(EAnimationStateEventKey key)
    {
        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder) == false)
        {
            holder = new AnimationStateEventBundle();
            eventDict.Add(key, holder);
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
        eventDict.Clear();
    }

    /// <summary>
    /// 상태 진입 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateEventCommander.InvokeEnterCallback(EAnimationStateEventKey key)
    {
        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder))
        {
            holder.InvokeOnEnter();
        }
    }

    /// <summary>
    /// 상태 탈출 콜백 실행. AnimationStateEventBehaviour에서 실행
    /// </summary>
    void IAnimationStateEventCommander.InvokeExitCallback(EAnimationStateEventKey key)
    {
        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder))
        {
            holder.InvokeOnExit();
        }
    }
}
