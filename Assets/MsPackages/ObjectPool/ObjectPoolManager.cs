using UnityEngine;
using System.Collections.Generic;

public static class ObjectPoolManager
{
    private static GameObject _objectHolder;

    /// <summary>
    /// Resources에서 불러온 게임 오브젝트 캐싱 딕셔너리
    /// </summary>
    private static readonly Dictionary<EPoolableType, GameObject> _resourceDict = new();

    /// <summary>
    /// 현재 풀링한 오브젝트 딕셔너리
    /// </summary>
    private static readonly Dictionary<EPoolableType, Queue<PoolableBehaviour>> _poolableDict = new();

    /// <summary>
    /// 현재 풀링한 갯수 딕셔너리
    /// </summary>
    private static readonly Dictionary<EPoolableType, int> _poolingCountDict = new();

    private static bool _isInitialized = false;

    /// <summary>
    /// ScriptableObject로부터 프리팹 리소스를 사전에 딕셔너리로 구축
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized) return;

        // SO 로드 (경로는 프로젝트 상황에 맞게 수정)
        PoolableResourcesData resourceData = Utils.Load<PoolableResourcesData>("ResourceData/PoolableResourcesData");

        if (resourceData == null)
        {
            Debug.LogError("[ObjectPoolManager] PoolableResourcesData 로드 실패!");
            return;
        }

        _resourceDict.Clear();

        // LINQ를 사용하지 않고 반복문으로 딕셔너리 채우기
        for (int i = 0; i < resourceData.poolableResourceList.Count; i++)
        {
            PoolableResource data = resourceData.poolableResourceList[i];

            if (data.poolableObject == null)
                continue;

            if (_resourceDict.ContainsKey(data.poolableType) == false)
            {
                _resourceDict.Add(data.poolableType, data.poolableObject);
            }
        }

        _isInitialized = true;
    }

    /// <summary>
    /// 오브젝트 풀에서 꺼내기
    /// </summary>
    public static PoolableBehaviour GetFromPool(EPoolableType poolableType)
    {
        // 오브젝트 홀더가 없을 때
        if (_objectHolder == null)
        {
            _objectHolder = new GameObject("@ObjectHolder");
            _poolableDict.Clear();
            _poolingCountDict.Clear();
        }

        if (_poolableDict.ContainsKey(poolableType) == false)
        {
            _poolableDict.Add(poolableType, new());
        }

        if (_poolableDict[poolableType].Count < 1)
        {
            CreatePoolables(poolableType);
        }

        PoolableBehaviour poolable = _poolableDict[poolableType].Dequeue();

        if (poolable != null)
        {
            if (poolable is IPoolableCommander poolableCommander)
            {
                poolableCommander.SetUsed();
            }
        }

        return poolable;
    }

    /// <summary>
    /// 풀에 보관할 오브젝트 생성
    /// </summary>
    private static void CreatePoolables(EPoolableType poolableType)
    {
        if (_resourceDict.TryGetValue(poolableType, out GameObject prefab) == false)
        {
            Debug.LogError($"[ObjectPoolManager] 등록되지 않은 풀링 타입입니다: {poolableType}");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError($"[ObjectPoolManager] Failed to load prefab : {poolableType}");
            return;
        }

        // 지수적 증가 방식
        if (_poolingCountDict.TryGetValue(poolableType, out int poolCount) == false)
        {
            poolCount = 1;
        }
        else
        {
            poolCount *= 2;
        }

        _poolingCountDict[poolableType] = poolCount;

        for (int i = 0; i < poolCount; i++)
        {
            GameObject instance = GameObject.Instantiate(prefab, _objectHolder.transform);
            instance.SetActive(false);

            if (instance.TryGetComponent(out PoolableBehaviour component))
            {
                _poolableDict[poolableType].Enqueue(component);
            }
            else
            {
                Debug.LogError($"[ObjectPoolManager] Component {typeof(PoolableBehaviour)} not found on {prefab.name}");

                // 잘못된 오브젝트는 파괴
                GameObject.Destroy(instance);
                break;
            }
        }
    }

    /// <summary>
    /// 오브젝트 풀로 반환
    /// </summary>
    public static void ReturnToPool(PoolableBehaviour poolable)
    {
        if (poolable == null)
            return;

        if (poolable.IsUsing == false)
            return;

        if (_poolableDict.ContainsKey(poolable.PoolType))
        {
            if (poolable is IPoolableCommander poolableCommander)
            {
                poolableCommander.SetReleased();
            }

            _poolableDict[poolable.PoolType].Enqueue(poolable);
        }
    }
}
