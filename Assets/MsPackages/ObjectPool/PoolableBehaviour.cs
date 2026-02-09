using System;
using UnityEngine;

public interface IPoolable
{
    void SetUsed();

    void SetReleased();
}

public class PoolableBehaviour : MonoBehaviour, IPoolable
{
    [Header("Poolable")]
    [SerializeField] private EPoolableType poolType;
    public EPoolableType PoolType => poolType;

    [SerializeField] private MonoBehaviour mainComponent;
    public MonoBehaviour MainComponent => mainComponent;

    /// <summary>
    /// 풀에서 꺼낼때 처리
    /// </summary>
    public event Action OnGetFromPool;

    /// <summary>
    /// 풀로 되돌릴때 처리
    /// </summary>
    public event Action OnReturnToPool;

    /// <summary>
    /// 풀에서 꺼내 사용중인지 여부
    /// </summary>
    public bool IsUsing { get; private set; }

    /// <summary>
    /// 풀에서 꺼낼때 처리
    /// </summary>
    void IPoolable.SetUsed()
    {
        if (IsUsing)
            return;

        IsUsing = true;

        OnGetFromPool?.Invoke();
    }

    /// <summary>
    /// 풀로 되돌릴때 처리
    /// </summary>
    void IPoolable.SetReleased()
    {
        if (IsUsing == false)
            return;

        IsUsing = false;

        OnReturnToPool?.Invoke();
    }

    /// <summary>
    /// 이 오브젝트를 풀로 되돌린다
    /// </summary>
    public void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnToPool(this);
    }
}
