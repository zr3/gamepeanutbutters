using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLogger : MonoBehaviour {

    private static MessageLogger _instance;
    private Text text;
    private List<string> messages;

    void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        text = GetComponent<Text>();
        messages = new List<string>(20);
    }
	public static void LogMessage(string message)
    {
        _instance.messages.Add(message);
        while (_instance.messages.Count > 20)
        {
            _instance.messages.RemoveAt(0);
        }
        string result = "";
        foreach (string m in _instance.messages)
        {
            result += "# " + m + "\n";
        }
        _instance.text.text = result;
    }
}
