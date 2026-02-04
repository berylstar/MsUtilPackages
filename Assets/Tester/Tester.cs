using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    [SerializeField] private StatInitializer<int> hp;
    [SerializeField] private StatInitializer<float> moveSpeed;

    private void Start()
    {
        hp.Initialize();
        moveSpeed.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            hp.Stat.AddBaseValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            hp.Stat.SubtractBaseValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            hp.Stat.AddMaxValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            hp.Stat.AddMaxValue(-1);
        }
    }
}
