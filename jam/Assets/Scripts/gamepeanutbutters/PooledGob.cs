using UnityEngine;

public class PooledGob : MonoBehaviour
{
    public bool WasSafelyDestroyed = false;
    private bool registeredApplicationCallback = false;

    public void Awake()
    {
        if (!registeredApplicationCallback)
        {
            Application.wantsToQuit += () => WasSafelyDestroyed = true;
            registeredApplicationCallback = true;
        }
    }

    public void OnDestroy()
    {
        if (!WasSafelyDestroyed)
        {
            Debug.LogError($"{gameObject.name} was not destroyed by the pooler. This is not safe!");
        }
    }
}
