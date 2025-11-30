using UnityEngine;

public class Tester : MonoBehaviour
{
    public float num1 = 1;
    [InspectorReadonly] public float num2 = 2;
    [InspectorReadonly, SerializeField] private float num3 = 3;

    private void Start()
    {
        Utils.Log(this.gameObject.IsLayer("Water"));
    }

    private void Update()
    {
        num1 += Time.deltaTime;
        num2 += Time.deltaTime;
        num3 += Time.deltaTime;
    }
}
