using UnityEngine;

/// <summary>
/// Animator State를 식별하기 위한 enum Key
/// </summary>
public enum EAnimationStateEventKey
{
    
}

/// <summary>
/// Animator의 State에 붙는 컴포넌트
/// 상태 진입/종료 시 AnimationStateEventHandler 콜백 전달
/// </summary>
public class AnimationStateEventBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// 해당 Animation State의 enum Key
    /// Animation 윈도우에서 매핑
    /// </summary>
    public EAnimationStateEventKey animationCallbackKey;

    private AnimationStateEventHandler handler;

    /// <summary>
    /// Handler 캐싱
    /// </summary>
    private void EnsureHandler(Animator animator)
    {
        if (handler == null)
        {
            if (animator.TryGetComponent(out AnimationStateEventHandler getHandler))
            {
                handler = getHandler;
            }
        }
    }

    /// <summary>
    /// 애니매이션 상태로 진입할 때 실행
    /// </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureHandler(animator);
        handler?.InvokeEnterCallback(animationCallbackKey);
    }

    /// <summary>
    /// 애니매이션 상태에서 나갈 때 실행
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureHandler(animator);
        handler?.InvokeExitCallback(animationCallbackKey);
    }
}
