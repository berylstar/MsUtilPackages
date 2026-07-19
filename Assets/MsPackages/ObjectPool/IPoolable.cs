/// <summary>
/// 풀링되는 주 컴포넌트의 활성화/반환 수명주기
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 풀에서 꺼낼때 처리
    /// </summary>
    void OnSpawned();

    /// <summary>
    /// 풀로 되돌릴때 처리
    /// </summary>
    void OnDespawned();
}