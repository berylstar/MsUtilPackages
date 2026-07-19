/// <summary>
/// 스테이트 머신의 상태 인터페이스
/// </summary>
public interface IState
{
    /// <summary>
    /// 상태 진입시 실행되는 메소드
    /// </summary>
    public void Enter();

    /// <summary>
    /// 상태 이탈시 실행되는 메소드
    /// </summary>
    public void Exit();

    /// <summary>
    /// 해당 상태에서 실행될 Update
    /// </summary>
    public void Update();

    /// <summary>
    /// 해당 상태에서 실행될 FixedUpdate
    /// </summary>
    public void FixedUpdate();
}
