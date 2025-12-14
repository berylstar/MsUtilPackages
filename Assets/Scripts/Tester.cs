using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    [InspectorReadonly] public float num2 = 2;
    [InspectorReadonly, SerializeField] private float num3 = 3;

    [Header("Tester")]
    public int intValue = 1;
    public float floatValue = 0.5f;
    public bool boolValue = true;

    public GameObject target;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {

        }

        if (Input.GetKeyDown(KeyCode.F3))
        {

        }
    }
}
