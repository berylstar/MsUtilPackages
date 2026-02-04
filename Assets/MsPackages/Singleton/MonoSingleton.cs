using UnityEngine;

/// <summary>
/// 싱글톤 패턴을 위한 클래스
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 배치된 인스턴스 탐색
                //var obj = FindObjectsOfType<T>();
                var findObject = FindFirstObjectByType<T>();

                if (findObject == null)
                {
                    // 새로 생성
                    var singletonObject = new GameObject(typeof(T).Name).AddComponent<T>();
                    _instance = singletonObject;
                }
                else
                {
                    _instance = findObject;
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 아직 인스턴스가 없으면 자신을 등록
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(base.gameObject);
            return;
        }

        // 이미 다른 인스턴스가 있으면 기존 인스턴스를 사용하고 자신은 파괴
        if (_instance != this)
        {
            Destroy(base.gameObject);
            return;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}