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
                    _instance.OnStringUpdated.Invoke(value as string);
                    break;
                case int i:
                    _instance.OnIntUpdated.Invoke(value as int? ?? default);
                    break;
                case float f:
                    _instance.OnFloatUpdated.Invoke(value as float? ?? default);
                    break;
                case bool b:
                    _instance.OnBoolUpdated.Invoke(value as bool? ?? default);
                    break;
            }
    }

    public StringEvent OnStringUpdated;
    public IntEvent OnIntUpdated;
    public FloatEvent OnFloatUpdated;
    public BoolEvent OnBoolUpdated;
}

[Serializable]
public class StringEvent : UnityEvent<string> { }
[Serializable]
public class IntEvent : UnityEvent<int> { }
[Serializable]
public class FloatEvent : UnityEvent<float> { }
[Serializable]
public class BoolEvent : UnityEvent<bool> { }