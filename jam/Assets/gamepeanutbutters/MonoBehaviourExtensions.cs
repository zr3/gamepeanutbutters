using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static T CheckSingleton<T>(this MonoBehaviour self, T instance) where T : MonoBehaviour
    {
        if (instance != null)
        {
            Debug.LogError($"There should be only one instance of {nameof(T)}. Switching from the one on {instance.gameObject.name} to {self.gameObject.name}.");
        }
        return self as T;
    }
}
