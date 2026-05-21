using UnityEngine;

/// <summary>
/// Animator의 State에 붙는 컴포넌트
/// 상태 진입/종료 시 AnimationStateEventDisaptcher 콜백 전달
/// </summary>
public class AnimationStateEventBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// 해당 Animation State의 enum Key
    /// Animation 윈도우에서 매핑
    /// </summary>
    public EAnimationStateEventKey animationCallbackKey;

    /// <summary>
    /// Animator가 부착된 게임 오브젝트의 AnimationStateEventDisaptcher
    /// </summary>
    private IAnimationStateListener _listener;

    /// <summary>
    /// 리스너 탐색 확인 플래그
    /// </summary>
    private bool _isListenerChecked = false;

    /// <summary>
    /// Handler 캐싱
    /// </summary>
    private void EnsureListener(Animator animator)
    {
        if (_isListenerChecked == false)
        {
            animator.TryGetComponent<IAnimationStateListener>(out _listener);

            _isListenerChecked = true;
        }
    }

    /// <summary>
    /// 애니매이션 상태로 진입할 때 실행
    /// </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureListener(animator);

        _listener?.OnStateEnter(animationCallbackKey);
    }

    /// <summary>
    /// 애니매이션 상태에서 나갈 때 실행
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnsureListener(animator);

        _listener?.OnStateExit(animationCallbackKey);
    }
}
