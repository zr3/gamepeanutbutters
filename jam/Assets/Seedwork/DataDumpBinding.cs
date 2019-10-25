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

    private void Awake()
    {
        switch (PropertyType)
        {
            case DataType.String:
                if (OnStringUpdated != null) DataDump.Instance.OnStringUpdated.AddListener(val => OnStringUpdated.Invoke(val));
                break;
            case DataType.Int:
                if (OnIntUpdated != null) DataDump.Instance.OnIntUpdated.AddListener(val => OnIntUpdated.Invoke(val));
                if (OnIntUpdatedAsString != null) DataDump.Instance.OnIntUpdated.AddListener(val => OnIntUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Float:
                if (OnFloatUpdated != null) DataDump.Instance.OnFloatUpdated.AddListener(val => OnFloatUpdated.Invoke(val));
                if (OnFloatUpdatedAsString != null) DataDump.Instance.OnFloatUpdated.AddListener(val => OnFloatUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Bool:
                if (OnBoolUpdated != null) DataDump.Instance.OnBoolUpdated.AddListener(val => OnBoolUpdated.Invoke(val));
                if (OnBoolUpdatedAsString != null) DataDump.Instance.OnBoolUpdated.AddListener(val => OnBoolUpdatedAsString.Invoke(val.ToString()));
                break;
        }
    }

    private void OnDisable()
    {
        switch (PropertyType)
        {
            case DataType.String:
                if (OnStringUpdated != null) DataDump.Instance.OnStringUpdated.RemoveListener(val => OnStringUpdated.Invoke(val));
                break;
            case DataType.Int:
                if (OnIntUpdated != null) DataDump.Instance.OnIntUpdated.RemoveListener(val => OnIntUpdated.Invoke(val));
                if (OnIntUpdatedAsString != null) DataDump.Instance.OnIntUpdated.RemoveListener(val => OnIntUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Float:
                if (OnFloatUpdated != null) DataDump.Instance.OnFloatUpdated.RemoveListener(val => OnFloatUpdated.Invoke(val));
                if (OnFloatUpdatedAsString != null) DataDump.Instance.OnFloatUpdated.RemoveListener(val => OnFloatUpdatedAsString.Invoke(val.ToString()));
                break;
            case DataType.Bool:
                if (OnBoolUpdated != null) DataDump.Instance.OnBoolUpdated.RemoveListener(val => OnBoolUpdated.Invoke(val));
                if (OnBoolUpdatedAsString != null) DataDump.Instance.OnBoolUpdated.RemoveListener(val => OnBoolUpdatedAsString.Invoke(val.ToString()));
                break;
        }
    }
}
