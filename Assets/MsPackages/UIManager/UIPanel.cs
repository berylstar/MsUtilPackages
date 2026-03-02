using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UIManager에서 실행할 수 있도록 하는 UI 명령 인터페이스
/// </summary>
public interface IUICommand
{
    void OnOpen();

    void OnClose();
}

/// <summary>
/// UI 패널의 최상위 추상 클래스
/// </summary>
public abstract class UIPanel : MonoBehaviour, IUICommand
{
    /// <summary>
    /// UI 패널 타입
    /// </summary>
    public abstract EUIType UIType { get; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    public bool IsActivated { get; private set; }

    private RectTransform _rectTransform;
    protected RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    /// <summary>
    /// 패널 비활성화
    /// </summary>
    public void CloseThis()
    {
        UIManager.Instance.Close(UIType);
    }

    #region IUICommand

    /// <summary>
    /// 활성화 시퀀스
    /// </summary>
    void IUICommand.OnOpen()
    {
        if (IsActivated)
            return;

        IsActivated = true;
        this.gameObject.SetActive(true);

        InitializeRectTransform();

        PlayOpenSequence();
    }

    /// <summary>
    /// 패널 비활성화 시퀀스
    /// </summary>
    void IUICommand.OnClose()
    {
        if (IsActivated == false)
            return;

        PlayCloseSequence(() =>
        {
            IsActivated = false;
            this.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// UI가 열린 이후 처리
    /// </summary>
    protected virtual void PlayOpenSequence()
    {

    }

    /// <summary>
    /// UI가 닫히기 이전 처리
    /// </summary>
    protected virtual void PlayCloseSequence(Action onFinished)
    {
        onFinished?.Invoke();
    }
    #endregion

    /// <summary>
    /// RectTransform 초기화
    /// </summary>
    private void InitializeRectTransform()
    {
        RectTransform.anchoredPosition = Vector2.zero;
        RectTransform.sizeDelta = Vector2.zero;
        RectTransform.localScale = Vector3.one;
    }

    #region UI Element Binding
    /// <summary>
    /// 버튼 클릭 이벤트 초기화
    /// </summary>
    protected void BindOnClickButton(Button btn, UnityAction clickAction)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(clickAction);
    }

    /// <summary>
    /// 토글 변화 이벤트 초기화
    /// </summary>
    protected void BindOnChangeToggle(Toggle toggle, UnityAction<bool> changeAction)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(changeAction);
    }
    #endregion
}
