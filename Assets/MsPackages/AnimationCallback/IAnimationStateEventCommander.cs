/// <summary>
/// AnimationStateEventBehaviour 실행할 수 있도록 하는 애니매이션 이벤트 명령 인터페이스
/// </summary>
public interface IAnimationStateEventCommander
{
    void InvokeEnterCallback(EAnimationStateEventKey key);

    void InvokeExitCallback(EAnimationStateEventKey key);
}