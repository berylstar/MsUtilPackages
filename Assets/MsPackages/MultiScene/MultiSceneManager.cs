using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EScene
{
    // 열거형의 값은 Build Settings의 Scene Index
    None = -1,

    Core = 0,
    TesterScene = 1,
    TesterTwoScene = 2,
}

public class MultiSceneManager : MonoBehaviour
{
    #region Singleton
    public static MultiSceneManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    /// <summary>
    /// 슬롯을 키로 하여 로드된 씬 이름을 관리하는 딕셔너리
    /// </summary>
    private readonly Dictionary<string, EScene> _loadedSceneBySlot = new Dictionary<string, EScene>();

    /// <summary>
    /// 현재 씬 전환 작업이 진행 중인지 확인하는 플래그
    /// </summary>
    private bool _isBusy = false;

    private readonly SceneTransitionPlan _transitionPlan = new SceneTransitionPlan();

    /// <summary>
    /// 빌더 패턴을 시작하기 위한 트랜지션 객체 생성
    /// </summary>
    public SceneTransitionPlan StartTransition()
    {
        if (_isBusy)
        {
            Debug.LogWarning("이미 다른 씬 전환 작업 중입니다.");
            return null;
        }

        _transitionPlan.Clear();
        return _transitionPlan;
    }

    /// <summary>
    /// 생성된 SceneTransitionPlan을 실제 코루틴을 실행하는 진입점
    /// </summary>
    private void ExecutePlan(SceneTransitionPlan plan)
    {
        // 이미 씬 전환 중이라면 새로운 전환 요청을 무시
        if (_isBusy)
        {
            Debug.LogWarning("이미 다른 씬 전환 작업 중입니다.");
            return;
        }

        _isBusy = true;

        StartCoroutine(CoTransitionScene(plan));
    }

    /// <summary>
    /// 씬 전환 플랜 메인 코루틴
    /// </summary>
    private IEnumerator CoTransitionScene(SceneTransitionPlan plan)
    {
        // 1. 등록된 언로드 대상 씬 모두 내림
        foreach (string slotkey in plan.SlotKeysToUnload)
        {
            yield return CoUnloadScene(slotkey);
        }

        // 2. 미사용 에셋 메모리 정리
        if (plan.IsClearingUnusedAssets)
        {
            yield return CoCleanupUnusedAssets();
        }

        // 3. 플랜에 등록된 로드 대상 씬들 추가
        foreach ((string currSlot, EScene currType) in plan.ScenesToLoad)
        {
            // 빌드 세팅 누락 검사
            if (Application.CanStreamedLevelBeLoaded((int)currType) == false)
            {
                Debug.LogError($"[{currType}] 씬을 찾을 수 없습니다. Build Settings를 확인하세요.");
                continue;
            }

            // 해당 슬롯의 씬 언로드
            yield return CoUnloadScene(currSlot);

            // 비동기 로드
            yield return CoLoadAdditiveScene(currSlot, currType, plan.ActiveSceneType == currType, plan.OnProgressUpdated);
        }

        // 씬 전환 작업 종료 후 플래그 해제
        _isBusy = false;
    }

    /// <summary>
    /// 씬을 Additive 모드로 비동기 로드하는 코루틴
    /// </summary>
    private IEnumerator CoLoadAdditiveScene(string slotKey, EScene sceneType, bool isActiveScene, Action<float> callbackProgress)
    {
        int sceneIndex = (int)sceneType;
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        if (loadOp == null)
            yield break;

        // 씬을 활성화하지 않고 대기
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            callbackProgress?.Invoke(loadOp.progress);

            yield return null;
        }

        // 로딩 완료 후 씬 활성화 허용
        callbackProgress?.Invoke(1f);
        loadOp.allowSceneActivation = true;

        // 완전히 메모리에 올라가고 활성화될 때까지 대기
        while (loadOp.isDone == false)
        {
            yield return null;
        }

        // 로드된 씬을 유니티의 Active Scene으로 지정할지 결정
        if (isActiveScene)
        {
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                SceneManager.SetActiveScene(loadedScene);
            }
        }

        // 딕셔너리에 현재 로드된 슬롯과 씬 이름 정보 갱신
        _loadedSceneBySlot[slotKey] = sceneType;
    }

    /// <summary>
    /// 슬롯 비동기 언로드 코루틴
    /// </summary>
    private IEnumerator CoUnloadScene(string slotKey)
    {
        if (_loadedSceneBySlot.TryGetValue(slotKey, out EScene sceneType) == false)
            yield break;

        if (sceneType == EScene.None)
            yield break;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync((int)sceneType);

        if (unloadOp != null)
        {
            // 언로드가 완료될 때까지 대기
            while (unloadOp.isDone == false)
            {
                yield return null;
            }
        }

        // 딕셔너리에서 해당 슬롯 정보 제거
        _loadedSceneBySlot.Remove(slotKey);
    }

    /// <summary>
    /// 메모리에 남은 불필요한 에셋들을 정리하는 코루틴
    /// </summary>
    private IEnumerator CoCleanupUnusedAssets()
    {
        AsyncOperation cleanupOp = Resources.UnloadUnusedAssets();

        while (cleanupOp.isDone == false)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 플루언트 인터페이스를 제공하여
    /// 메소드 체이닝으로 직관적인 씬 전환 플랜을 세우는 클래스
    /// </summary>
    public class SceneTransitionPlan
    {
        /// <summary>
        /// 로드할 씬의 정보를 담는 딕셔너리 (슬롯 - 씬 이름)
        /// </summary>
        public Dictionary<string, EScene> ScenesToLoad { get; } = new Dictionary<string, EScene>();

        /// <summary>
        /// 언로드할 슬롯 키를 담는 리스트
        /// </summary>
        public List<string> SlotKeysToUnload { get; } = new List<string>();

        /// <summary>
        /// 씬 로드 완료 후 Active Scene으로 지정할 씬의 이름
        /// </summary>
        public EScene ActiveSceneType { get; private set; } = EScene.None;

        /// <summary>
        /// 씬 전환 도중 미사용 메모리를 정리할 지 여부
        /// </summary>
        public bool IsClearingUnusedAssets { get; private set; } = false;

        /// <summary>
        /// 씬 로딩 진행률 콜백
        /// </summary>
        public Action<float> OnProgressUpdated;

        /// <summary>
        /// 초기화
        /// </summary>
        public void Clear()
        {
            ScenesToLoad.Clear();
            SlotKeysToUnload.Clear();
            ActiveSceneType = EScene.None;
            IsClearingUnusedAssets = false;
            OnProgressUpdated = null;
        }

        /// <summary>
        /// 로드할 씬을 플랜에 추가
        /// </summary>
        public SceneTransitionPlan Load(string slotKey, EScene sceneType, bool isActiveScene = false)
        {
            ScenesToLoad[slotKey] = sceneType;

            if (isActiveScene)
            {
                ActiveSceneType = sceneType;
            }

            return this;
        }

        /// <summary>
        /// 언로드할 씬 슬롯을 플랜에 추가
        /// </summary>
        public SceneTransitionPlan Unload(string slotkey)
        {
            SlotKeysToUnload.Add(slotkey);

            return this;
        }

        /// <summary>
        /// 씬 언로드 후 메모리 정리 수행 옵션 설정
        /// </summary>
        public SceneTransitionPlan ClearUnusedAssets()
        {
            IsClearingUnusedAssets = true;

            return this;
        }

        /// <summary>
        /// 씬 로딩 진행률 콜백 추가
        /// </summary>
        public SceneTransitionPlan RegisterOnProgress(Action<float> callbackProgress)
        {
            OnProgressUpdated = callbackProgress;

            return this;
        }

        /// <summary>
        /// 씬 전환 실행
        /// </summary>
        public void Execute()
        {
            MultiSceneManager.Instance.ExecutePlan(this);
        }
    }
}