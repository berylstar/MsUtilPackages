using System;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public float num1 = 1;
    [InspectorReadonly] public float num2 = 2;
    [InspectorReadonly, SerializeField] private float num3 = 3;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Utils.Log(Utils.SecondsToMS(num1));
    }
}
