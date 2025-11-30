using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        Utils.Log(this.gameObject.IsLayer("Water"));
    }
}
