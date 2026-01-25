using UnityEngine;

public interface IPoolable
{
    void SetUsed();

    void SetReleased();
}

public abstract class PoolableBehaviour : MonoBehaviour, IPoolable
{
    [Header("Poolable")]
    [SerializeField] private EPoolableType poolType;
    public EPoolableType PoolType => poolType;

    /// <summary>
    /// 풀에서 꺼내 사용중인지 여부
    /// </summary>
    private bool _isUsing = false;
    public bool IsUsing => _isUsing;

    /// <summary>
    /// 풀에서 꺼낼때 처리
    /// </summary>
    void IPoolable.SetUsed()
    {
        if (_isUsing)
            return;

        _isUsing = true;

        OnGetFromPool();
    }

    /// <summary>
    /// 풀로 되돌릴때 처리
    /// </summary>
    void IPoolable.SetReleased()
    {
        if (_isUsing == false)
            return;

        _isUsing = false;

        OnReturnToPool();
    }

    /// <summary>
    /// 풀에서 꺼낼때 처리
    /// </summary>
    protected abstract void OnGetFromPool();

    /// <summary>
    /// 풀로 되돌릴때 처리
    /// </summary>
    protected abstract void OnReturnToPool();

    /// <summary>
    /// 이 오브젝트를 풀로 되돌린다
    /// </summary>
    protected void ReturnToPoolThis()
    {
        ObjectPoolManager.Instance.ReturnToPool(this);
    }
}
