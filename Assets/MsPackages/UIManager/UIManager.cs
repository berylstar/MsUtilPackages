using System;
using System.Collections.Generic;
using UnityEngine;

public enum EUIType
{
    // 열거형의 값은 소트오더로써 사용

    UIPanel_TesterOne,
    UIPanel_TesterTwo,

    //--------------------
    MaxCount
}

public class UIManager : MonoSingleton<UIManager>
{
    private const string TAG_UIROOT = "UIRoot";
    private const string PATH_UIPREFAB = "UIPrefabs/";

    /// <summary>
    /// 생성된 UI 패널 딕셔너리
    /// </summary>
    private readonly Dictionary<EUIType, UIPanel> _activeUIs = new Dictionary<EUIType, UIPanel>();

    /// <summary>
    /// Generic 타입으로 Enum 찾기 위한 성능 최적화용 캐시 딕셔너리
    /// </summary>
    private readonly Dictionary<Type, EUIType> _typeCache = new Dictionary<Type, EUIType>();

    private Transform _uiRoot;

    /// <summary>
    /// 씬에 배치되는 캔버스
    /// </summary>
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.FindWithTag(TAG_UIROOT).transform;
            }

            return _uiRoot;
        }
    }

    /// <summary>
    /// UI 패널 활성화. 이미 생성되어 있다면 OnOpen 시퀀스만 실행
    /// </summary>
    /// <typeparam name="T">UIPanel을 상속받은 클래스</typeparam>
    /// <returns>생성 또는 활성화된 UI 패널</returns>
    public T Open<T>() where T : UIPanel
    {
        EUIType uiType = GetUIType<T>();

        // UI 최초 생성시
        if (TryGet(uiType, out UIPanel panel) == false)
        {
            GameObject uiResource = Resources.Load<GameObject>($"{PATH_UIPREFAB}{uiType}");

            if (uiResource == null)
            {
                throw new Exception($"[UIManager] NOT EXISTED PREFAB : {uiType}");
            }

            GameObject uiObject = Instantiate(uiResource, UIRoot);

            panel = uiObject.GetComponent<UIPanel>();

            _activeUIs.Add(uiType, panel);

            // 신규 생성 시에만 정렬
            SortByType();
        }

        if (panel is IUICommand uiCommand)
        {
            uiCommand.OnOpen();
        }

        return panel as T;
    }

    /// <summary>
    /// UI 패널 비활성화
    /// </summary>
    public void Close<T>() where T : UIPanel
    {
        Close(GetUIType<T>());
    }

    /// <summary>
    /// UI 패널 비활성화
    /// </summary>
    public void Close(EUIType uiType)
    {
        if (TryGet(uiType, out UIPanel uiPanel))
        {
            if (uiPanel is IUICommand uiCommand)
            {
                uiCommand.OnClose();
            }
        }
    }

    /// <summary>
    /// 활성화된 모든 UI 패널 비활성화
    /// </summary>
    public void CloseAll()
    {
        foreach (EUIType uiType in _activeUIs.Keys)
        {
            Close(uiType);
        }
    }

    /// <summary>
    /// 활성화된 모든 UI 패널 제거
    /// </summary>
    public void ClearAll()
    {
        foreach (UIPanel panel in _activeUIs.Values)
        {
            if (panel != null)
            {
                Destroy(panel.gameObject);
            }
        }

        _activeUIs.Clear();
    }

    /// <summary>
    /// 클래스 이름과 동일한 Enum 값을 리플렉션으로 찾아 캐싱
    /// </summary>
    private EUIType GetUIType<T>() where T : UIPanel
    {
        if (_typeCache.TryGetValue(typeof(T), out EUIType uiType) == false)
        {
            if (Enum.TryParse(typeof(T).Name, out EUIType parsedUIType) == false)
            {
                throw new Exception($"[UIManager] Enum matching failed for type: {typeof(T).Name}");
            }

            uiType = parsedUIType;

            _typeCache.Add(typeof(T), uiType);
        }

        return uiType;
    }

    /// <summary>
    /// 활성화된 UI 패널 반환
    /// </summary>
    private bool TryGet<T>(EUIType uiType, out T uiReturn) where T : UIPanel
    {
        if (_activeUIs.TryGetValue(uiType, out UIPanel uiPanel))
        {
            if (uiPanel == null)
            {
                _activeUIs.Remove(uiType);

                uiReturn = null;
                return false;
            }
            else
            {
                uiReturn = uiPanel as T;
                return true;
            }
        }
        else
        {
            uiReturn = null;
            return false;
        }
    }

    /// <summary>
    /// EUIType 열거형 값에 따라 우선순위 소팅
    /// </summary>
    private void SortByType()
    {
        for (int i = 0; i < (int)EUIType.MaxCount; i++)
        {
            EUIType uiType = (EUIType)i;

            if (TryGet(uiType, out UIPanel uiPanel))
            {
                // Hierarchy 아래 있을 수록 앞에 보인다
                uiPanel.transform.SetAsLastSibling();
            }
        }
    }
}