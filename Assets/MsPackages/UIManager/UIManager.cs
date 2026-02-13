using System;
using System.Collections.Generic;
using UnityEngine;

public enum EUIType
{
    UIPanel_TesterOne,
    UIPanel_TesterTwo,

    //--------------------
    MaxCount
}

public class UIManager : MonoSingleton<UIManager>
{
    private const string TAG_UIROOT = "UIRoot";
    private const string PATH_UIPREFAB = "UIPrefabs/";

    private readonly Dictionary<EUIType, UIPanel> _activeUIs = new Dictionary<EUIType, UIPanel>();

    private readonly Dictionary<Type, EUIType> _typeCache = new Dictionary<Type, EUIType>();

    private Transform _uiRoot;
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

    public T Open<T>() where T : UIPanel
    {
        EUIType uiType = GetUIType<T>();

        // UI 최초 생성시
        if (_activeUIs.TryGetValue(uiType, out UIPanel panel) == false)
        {
            GameObject uiResource = Resources.Load<GameObject>($"{PATH_UIPREFAB}{uiType}");

            if (uiResource == null)
            {
                throw new Exception($"OpenUI : {uiType}");
            }

            GameObject uiObject = Instantiate(uiResource, UIRoot);

            panel = uiObject.GetComponent<UIPanel>();

            _activeUIs.Add(uiType, panel);

            // 신규 생성 시에만 정렬
            SortByType();
        }

        panel.OnOpen();

        return panel as T;
    }

    public void Close<T>(bool destroyFlag = false) where T : UIPanel
    {
        EUIType uiType = GetUIType<T>();

        Close(uiType, destroyFlag);
    }

    public void Close(EUIType uiType, bool destroyFlag = false)
    {
        if (_activeUIs.TryGetValue(uiType, out UIPanel uiPanel))
        {
            uiPanel.OnClose(destroyFlag);

            if (destroyFlag)
            {
                _activeUIs.Remove(uiType);
            }
        }
    }

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

    public bool TryGet<T>(out T uiReturn) where T : UIPanel
    {
        EUIType uiType = GetUIType<T>();

        if (_activeUIs.TryGetValue(uiType, out UIPanel uiPanel))
        {
            uiReturn = uiPanel as T;
            return true;
        }
        else
        {
            uiReturn = null;
            return false;
        }
    }

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

    private void SortByType()
    {
        for (int i = 0; i < (int)EUIType.MaxCount; i++)
        {
            EUIType uiType = (EUIType)i;

            if (_activeUIs.TryGetValue(uiType, out UIPanel uiPanel))
            {
                uiPanel.transform.SetAsLastSibling();
            }
        }
    }
}