using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataDumpBinding : MonoBehaviour
{
    public string DataDumpProperty;

    public enum DataType { String, Int, Float, Bool }
    public DataType PropertyType;

    public StringEvent OnStringUpdated;
    public IntEvent OnIntUpdated;
    public StringEvent OnIntUpdatedAsString;
    public FloatEvent OnFloatUpdated;
    public StringEvent OnFloatUpdatedAsString;
    public BoolEvent OnBoolUpdated;
    public StringEvent OnBoolUpdatedAsString;

    public void Reset()
    {
        DataDumpProperty = null;
        OnStringUpdated = null;
        OnIntUpdated = null;
        OnIntUpdatedAsString = null;
        OnFloatUpdated = null;
        OnFloatUpdatedAsString = null;
        OnBoolUpdated = null;
        OnBoolUpdatedAsString = null;
    }

    private void OnEnable()
    {
        switch (PropertyType)
        {
            case DataType.String:
                if (OnStringUpdated != null) GetOrCreate<string, StringEvent>(DataDump.Instance.OnStringUpdated, DataDumpProperty, val => OnStringUpdated.Invoke(val));
                break;
            case DataType.Int:
                if (OnIntUpdated != null) GetOrCreate<int, IntEvent>(DataDump.Instance.OnIntUpdated, DataDumpProperty, val => OnIntUpdated.Invoke(val));
                if (OnIntUpdatedAsString != null) GetOrCreate<int, IntEvent>(DataDump.Instance.OnIntUpdated, DataDumpProperty, val => OnIntUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Float:
                if (OnFloatUpdated != null) GetOrCreate<float, FloatEvent>(DataDump.Instance.OnFloatUpdated, DataDumpProperty, val => OnFloatUpdated.Invoke(val));
                if (OnFloatUpdatedAsString != null) GetOrCreate<float, FloatEvent>(DataDump.Instance.OnFloatUpdated, DataDumpProperty, val => OnFloatUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Bool:
                if (OnBoolUpdated != null) GetOrCreate<bool, BoolEvent>(DataDump.Instance.OnBoolUpdated, DataDumpProperty, val => OnBoolUpdated.Invoke(val));
                if (OnBoolUpdatedAsString != null) GetOrCreate<bool, BoolEvent>(DataDump.Instance.OnBoolUpdated, DataDumpProperty, val => OnBoolUpdatedAsString.Invoke(val.ToString()));
                break;
        }
    }

    private void GetOrCreate<T, T2>(Dictionary<string, T2> dumpEvent, string key, UnityAction<T> @event) where T2 : UnityEvent<T>, new()
    {
        if (!dumpEvent.ContainsKey(key))
        {
            dumpEvent[key] = new T2();
        }
        dumpEvent[key].AddListener(@event);
    }

    private void OnDisable()
    {
        switch (PropertyType)
        {
            case DataType.String:
                if (OnStringUpdated != null) DataDump.Instance.OnStringUpdated[DataDumpProperty].RemoveListener(val => OnStringUpdated.Invoke(val));
                break;
            case DataType.Int:
                if (OnIntUpdated != null) DataDump.Instance.OnIntUpdated[DataDumpProperty].RemoveListener(val => OnIntUpdated.Invoke(val));
                if (OnIntUpdatedAsString != null) DataDump.Instance.OnIntUpdated[DataDumpProperty].RemoveListener(val => OnIntUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Float:
                if (OnFloatUpdated != null) DataDump.Instance.OnFloatUpdated[DataDumpProperty].RemoveListener(val => OnFloatUpdated.Invoke(val));
                if (OnFloatUpdatedAsString != null) DataDump.Instance.OnFloatUpdated[DataDumpProperty].RemoveListener(val => OnFloatUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Bool:
                if (OnBoolUpdated != null) DataDump.Instance.OnBoolUpdated[DataDumpProperty].RemoveListener(val => OnBoolUpdated.Invoke(val));
                if (OnBoolUpdatedAsString != null) DataDump.Instance.OnBoolUpdated[DataDumpProperty].RemoveListener(val => OnBoolUpdatedAsString.Invoke(val.ToString()));
                break;
        }
    }
}
