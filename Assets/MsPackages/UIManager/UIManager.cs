using System;
using System.Collections.Generic;
using UnityEngine;

public enum EUIType
{
    ////////////////////
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

    /// <summary>
    /// UI 생성
    /// </summary>
    /// <typeparam name="T"> 호출 되어야할 클래스 - 프리팹 이름이 같아야 한다.</typeparam>
    /// <param name="_active">오브젝트 활성화 여부</param>
    /// <returns> 호출한 클래스를 리턴한다 </returns>
    public T Open<T>()
    {
        if (Enum.TryParse(typeof(T).Name, out EUIType uiType) == false)
        {
            throw new Exception($"OpenUI : {uiType}");
        }

        // UI 최초 생성시
        if (_activeUIs.ContainsKey(uiType) == false)
        {
            GameObject uiResource = Resources.Load<GameObject>($"{PATH_UIPREFAB}{uiType}");

            if (uiResource == null)
            {
                throw new Exception($"OpenUI : {uiType}");
            }

            GameObject uiObject = Instantiate(uiResource, UIRoot);

            _activeUIs.Add(uiType, uiObject.GetComponent<UIPanel>());

            SortByType();
        }

        _activeUIs[uiType].gameObject.SetActive(true);

        return _activeUIs[uiType].GetComponent<T>();
    }

    /// <summary>
    /// UI 닫기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="destroyFlag">true = 오브젝트 파괴</param>
    public void Close<T>(bool destroyFlag = false)
    {
        if (Enum.TryParse(typeof(T).Name, out EUIType uiType) == false)
        {
            throw new Exception($"CloseUI : {uiType}");
        }

        if (_activeUIs.ContainsKey(uiType))
        {
            if (destroyFlag)
            {
                Destroy(_activeUIs[uiType].gameObject);
                _activeUIs.Remove(uiType);
            }
            else
            {
                _activeUIs[uiType].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 생성된 UI 반환
    /// </summary>
    /// <param name="uiReturn">해당하는 UI</param>
    /// <param name="activeFlag">true일 때, 오브젝트 활성화</param>
    /// <returns>해당 UI 생성 여부</returns>
    public bool TryGet<T>(out T uiReturn, bool activeFlag = false)
    {
        if (Enum.TryParse(typeof(T).Name, out EUIType uiType) == false)
        {
            throw new Exception($"GetUI : {uiType}");
        }

        if (_activeUIs.ContainsKey(uiType) == false || _activeUIs[uiType] == null)
        {
            uiReturn = default;
            return false;
        }
        else
        {
            uiReturn = _activeUIs[uiType].GetComponent<T>();

            if (activeFlag)
            {
                _activeUIs[uiType].gameObject.SetActive(true);
            }

            return true;
        }
    }

    /// <summary>
    /// 열거형의 값으로 UI 순서 정렬
    /// </summary>
    private void SortByType()
    {
        for (int i = 0; i < (int)EUIType.MaxCount; i++)
        {
            EUIType uiType = (EUIType)i;

            if (_activeUIs.ContainsKey(uiType))
            {
                _activeUIs[uiType].transform.SetAsLastSibling();
            }
        }
    }
}