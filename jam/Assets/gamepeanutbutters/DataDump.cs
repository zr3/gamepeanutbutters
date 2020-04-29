using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataDump : MonoBehaviour {

    private PersistantGameState primaryState;
    private static DataDump _instance;
    public static DataDump Instance { get => _instance; }

	void Awake () {
        primaryState = ScriptableObject.CreateInstance<PersistantGameState>();
        _instance = this.CheckSingleton(_instance);
	}

    public static T Get<T>(string key)
    {
        try
        {
            return (T)(typeof(PersistantGameState).GetProperty(key).GetValue(_instance.primaryState));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting {key} in {nameof(DataDump)}: {ex.GetType()} - {ex.Message}");
        }
        return default;
    }

    public static void Set<T>(string key, T value)
    {
            typeof(PersistantGameState).GetProperty(key).SetValue(_instance.primaryState, value);
            switch(value)
            {
                case string s:
                    if (_instance.OnStringUpdated.TryGetValue(key, out var @sevent)) @sevent.Invoke(value as string);
                    break;
                case int i:
                    if (_instance.OnIntUpdated.TryGetValue(key, out var @ievent)) @ievent.Invoke(value as int? ?? default);
                    break;
                case float f:
                    if (_instance.OnFloatUpdated.TryGetValue(key, out var @fevent)) @fevent.Invoke(value as float? ?? default);
                    break;
                case bool b:
                    if (_instance.OnBoolUpdated.TryGetValue(key, out var @bevent)) @bevent.Invoke(value as bool? ?? default);
                    break;
            }
    }

    public Dictionary<string, StringEvent> OnStringUpdated = new Dictionary<string, StringEvent>();
    public Dictionary<string, IntEvent> OnIntUpdated = new Dictionary<string, IntEvent>();
    public Dictionary<string, FloatEvent> OnFloatUpdated = new Dictionary<string, FloatEvent>();
    public Dictionary<string, BoolEvent> OnBoolUpdated = new Dictionary<string, BoolEvent>();

    private void OnDestroy()
    {
        Destroy(primaryState);
        primaryState = null;
    }
}

[Serializable]
public class StringEvent : UnityEvent<string> { }
[Serializable]
public class IntEvent : UnityEvent<int> { }
[Serializable]
public class FloatEvent : UnityEvent<float> { }
[Serializable]
public class BoolEvent : UnityEvent<bool> { }