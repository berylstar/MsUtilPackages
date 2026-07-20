using UnityEngine;

/// <summary>
/// Animator의 State에 붙는 컴포넌트
/// 상태 진입/종료 시 AnimationStateEventDispatcher에 콜백 전달
/// </summary>
public class AnimationStateEventBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// 해당 Animation State의 enum Key
    /// Animation 윈도우에서 매핑
    /// </summary>
    [SerializeField] private EAnimationStateKey animationCallbackKey;

    /// <summary>
    /// Animator가 부착된 게임 오브젝트의 AnimationStateEventDispatcher
    /// </summary>
    private AnimationStateEventDispatcher _dispatcher;

    /// <summary>
    /// Dispatcher 탐색 완료 플래그
    /// </summary>
    private bool _isDispatcherChecked;

    /// <summary>
    /// Inspector에서 MaxCount가 상태 Key로 설정되지 않도록 보정
    /// </summary>
    private void OnValidate()
    {
        if (animationCallbackKey != EAnimationStateKey.MaxCount)
            return;

        Debug.LogError($"{nameof(EAnimationStateKey.MaxCount)}는 Animation State Key로 사용할 수 없습니다.", this);

        animationCallbackKey = EAnimationStateKey.None;
    }

    /// <summary>
    /// Dispatcher 캐싱 및 이벤트 전달 가능 여부 확인
    /// </summary>
    private bool EnsureDispatcher(Animator animator)
    {
        if (animationCallbackKey == EAnimationStateKey.None)
            return false;

        if (_isDispatcherChecked)
            return _dispatcher != null;

        _isDispatcherChecked = true;

        if (animator.TryGetComponent(out _dispatcher))
            return true;

        Debug.LogWarning($"{animator.name}에 {nameof(AnimationStateEventDispatcher)} 컴포넌트가 없습니다.", animator);

        return false;
    }

    /// <summary>
    /// 애니매이션 상태로 진입할 때 실행
    /// </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (EnsureDispatcher(animator) == false)
            return;

        _dispatcher.DispatchStateEnter(animationCallbackKey);
    }

    /// <summary>
    /// 애니매이션 상태에서 나갈 때 실행
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (EnsureDispatcher(animator) == false)
            return;

        _dispatcher.DispatchStateExit(animationCallbackKey);
    }
}
