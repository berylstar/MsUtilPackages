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

    private readonly Dictionary<EUIType, UIPanel> _activeUIs = new();

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

    public T Open<T>(EUIType uiType) where T : UIPanel
    {
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
            panel.InitializeRectTransform();

            _activeUIs.Add(uiType, panel);

            // 신규 생성 시에만 정렬
            SortByType();
        }

        panel.OnOpen();

        return panel as T;
    }

    public void Close(EUIType uiType, bool destroyFlag = false)
    {
        if (_activeUIs.TryGetValue(uiType, out UIPanel uiPanel))
        {
            uiPanel.OnClose();

            if (destroyFlag)
            {
                Destroy(uiPanel.gameObject);
                _activeUIs.Remove(uiType);
            }
        }
    }

    public void Close(UIPanel uiPanel, bool destroyFlag = false)
    {
        Close(uiPanel.UIType, destroyFlag);
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

    public bool TryGet<T>(EUIType uiType, out T uiReturn) where T : UIPanel
    {
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