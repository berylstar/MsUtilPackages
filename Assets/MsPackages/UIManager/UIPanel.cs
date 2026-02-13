using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIPanel : MonoBehaviour
{
    public abstract EUIType UIType { get; }

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

    public abstract void OnOpen();

    public abstract void OnClose();

    public void InitializeRectTransform()
    {
        RectTransform.anchoredPosition = Vector2.zero;
        RectTransform.sizeDelta = Vector2.zero;
        RectTransform.localScale = Vector3.one;
    }

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
}
