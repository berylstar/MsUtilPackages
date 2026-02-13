using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UIPanel_TesterOne ui = UIManager.Instance.Open<UIPanel_TesterOne>(EUIType.UIPanel_TesterOne);
            ui.Initialize();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            UIManager.Instance.Close(EUIType.UIPanel_TesterOne, true);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            bool isActive = UIManager.Instance.TryGet(EUIType.UIPanel_TesterOne, out UIPanel_TesterOne ui);

            Debug.Log(isActive);
            Debug.Log(ui);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            UIPanel_TesterTwo ui = UIManager.Instance.Open<UIPanel_TesterTwo>(EUIType.UIPanel_TesterTwo);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            UIManager.Instance.Close(EUIType.UIPanel_TesterTwo, true);
        }
    }
}
