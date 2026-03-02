using UnityEngine;

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

    private IAnimationStateEventCommander _commander;

    /// <summary>
    /// Handler 캐싱
    /// </summary>
    private void EnsureHandler(Animator animator)
    {
        if (_commander == null)
        {
            if (animator.TryGetComponent(out IAnimationStateEventCommander getHandler))
            {
                _commander = getHandler;
            }
        }
    }

    /// <summary>
    /// 애니매이션 상태로 진입할 때 실행
    /// </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureHandler(animator);
        _commander?.InvokeEnterCallback(animationCallbackKey);
    }

    /// <summary>
    /// 애니매이션 상태에서 나갈 때 실행
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureHandler(animator);
        _commander?.InvokeExitCallback(animationCallbackKey);
    }
}
