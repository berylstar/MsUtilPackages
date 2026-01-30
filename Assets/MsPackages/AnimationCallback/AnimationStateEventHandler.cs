using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnimationStateEventBehaviour에 콜백을 등록하는 컴포넌트
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationStateEventHandler : MonoBehaviour
{
    /// <summary>
    /// Animation State Event 번들 클래스
    /// </summary>
    private class AnimationStateEventBundle
    {
        private event Action OnEnter;
        private event Action OnExit;

        public void RegisterOnEnter(Action callback)
        {
            OnEnter += callback;
        }

        public void RegisterOnExit(Action callback)
        {
            OnExit += callback;
        }

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

    public void RegisterEnterCallback(EAnimationStateEventKey key, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder) == false)
        {
            holder = new AnimationStateEventBundle();
            eventDict.Add(key, holder);
        }

        holder.RegisterOnEnter(callback);
    }

    public void RegisterExitCallback(EAnimationStateEventKey key, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder) == false)
        {
            holder = new AnimationStateEventBundle();
            eventDict.Add(key, holder);
        }

        holder.RegisterOnExit(callback);
    }

    public void InvokeEnterCallback(EAnimationStateEventKey key)
    {
        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder))
        {
            holder.InvokeOnEnter();
        }
    }

    public void InvokeExitCallback(EAnimationStateEventKey key)
    {
        if (eventDict.TryGetValue(key, out AnimationStateEventBundle holder))
        {
            holder.InvokeOnExit();
        }
    }
}
