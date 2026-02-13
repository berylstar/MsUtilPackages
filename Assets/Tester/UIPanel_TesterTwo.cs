using UnityEngine;

public class UIPanel_TesterTwo : UIPanel
{
    public override EUIType UIType => EUIType.UIPanel_TesterTwo;

    public override void OnOpen()
    {
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        this.gameObject.SetActive(false);
    }
}
