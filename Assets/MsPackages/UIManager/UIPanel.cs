using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIPanel : MonoBehaviour
{
    public abstract EUIType UIType { get; }
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

    public void OnOpen()
    {
        IsActivated = true;
        this.gameObject.SetActive(true);

        InitializeRectTransform();

        PlayOpenSequence();
    }

    public void OnClose(bool destroyFlag)
    {
        PlayCloseSequence(() =>
        {
            if (destroyFlag)
            {
                Destroy(this.gameObject);
            }
            else
            {
                IsActivated = false;
                this.gameObject.SetActive(false);
            }
        });
    }

    protected virtual void PlayOpenSequence()
    {

    }

    protected virtual void PlayCloseSequence(Action onFinished)
    {
        onFinished?.Invoke();
    }

    #region UI Element
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

    private void InitializeRectTransform()
    {
        RectTransform.anchoredPosition = Vector2.zero;
        RectTransform.sizeDelta = Vector2.zero;
        RectTransform.localScale = Vector3.one;
    }
}
