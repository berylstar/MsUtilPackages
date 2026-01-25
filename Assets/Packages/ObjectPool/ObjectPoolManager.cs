using UnityEngine;
using System.Collections.Generic;

public enum EPoolableType
{

}

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    private GameObject _objectHolder;

    private readonly Dictionary<EPoolableType, Queue<PoolableBehaviour>> _poolableDict = new();

    private const int DEFAULT_POOL_COUNT = 5;

    /// <summary>
    /// Poolable 오브젝트의 리소스 경로
    /// </summary>
    private string GetPoolablePath(EPoolableType poolableType)
    {
        return poolableType switch
        {
            _ => string.Empty
        };
    }

    /// <summary>
    /// 오브젝트 풀에서 꺼내기
    /// </summary>
    public T GetFromPool<T>(EPoolableType poolableType) where T : PoolableBehaviour
    {
        // 오브젝트 홀더가 없을 때
        if (_objectHolder == null)
        {
            _objectHolder = new GameObject("@ObjectHolder");
            _poolableDict.Clear();
        }

        if (_poolableDict.ContainsKey(poolableType) == false)
        {
            _poolableDict.Add(poolableType, new());
        }

        if (_poolableDict[poolableType].Count < 1)
        {
            CreatePoolables<T>(poolableType, DEFAULT_POOL_COUNT);
        }

        T poolable = _poolableDict[poolableType].Dequeue() as T;
        
        if (poolable != null)
        {
            if (poolable is IPoolable iPoolable)
            {
                iPoolable.SetUsed();
            }
        }

        return poolable;
    }

    /// <summary>
    /// 풀에 보관할 오브젝트 생성
    /// </summary>
    private void CreatePoolables<T>(EPoolableType poolableType, int poolCount) where T : PoolableBehaviour
    {
        string path = GetPoolablePath(poolableType);
        GameObject prefab = Utils.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"[ObjectPoolManager] Failed to load prefab at: {path}");
            return;
        }

        for (int i = 0; i < poolCount; i++)
        {
            GameObject instance = Instantiate(prefab, _objectHolder.transform);
            instance.SetActive(false);

            if (instance.TryGetComponent(out T component))
            {
                _poolableDict[poolableType].Enqueue(component);
            }
            else
            {
                Debug.LogError($"[ObjectPoolManager] Component {typeof(T)} not found on {prefab.name}");

                // 잘못된 오브젝트는 파괴
                Destroy(instance);
                break;
            }
        }
    }

    /// <summary>
    /// 오브젝트 풀로 반환
    /// </summary>
    public void ReturnToPool(PoolableBehaviour poolable)
    {
        if (poolable == null)
            return;

        if (poolable.IsUsing == false)
            return;

        if (_poolableDict.ContainsKey(poolable.PoolType))
        {
            if (poolable is IPoolable iPoolable)
            {
                iPoolable.SetReleased();
            }

            _poolableDict[poolable.PoolType].Enqueue(poolable);
        }
    }
}
