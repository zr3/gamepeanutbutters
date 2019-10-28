using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TextJacker : MonoBehaviour {

    public Vector2 RandomTimeRange;

    private string originalText;
    private string lastText;

    private float outlineTimer = 0f;
    private float textTimer = 0f;
    private readonly char[] chars = new[] {
        '!', '#', '$', '/', '=', '*', '^', '(', ')'
    };

    private Action<string> setText;
    private Func<string> getText;

    void Start () {
        var textUi = GetComponent<Text>();
        var textMesh = GetComponent<TextMesh>();
        var textMeshPro = GetComponent<TextMeshPro>();
        if (textUi)
        {
            getText = () => textUi.text;
            setText = text => textUi.text = text;
        } else if (textMesh)
        {
            getText = () => textMesh.text;
            setText = text => textMesh.text = text;
        } else if (textMeshPro)
        {
            getText = () => textMeshPro.text;
            setText = text => textMeshPro.text = text;
        } else {
            Debug.LogError($"{nameof(TextJacker)} on {gameObject.name} needs a {nameof(Text)}, a {nameof(TextMesh)}, or a {nameof(TextMeshPro)} component.");
        }
	}
	
	void Update () {
        textTimer -= Time.deltaTime;
        if (textTimer < 0) { JackText(); }
    }

    private void JackText()
    {
        if (getText() != lastText)
        {
            // Something has changed the text
            originalText = getText();
            lastText = getText();
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < originalText.Length; ++i)
        {
            sb.Append(IsBlacklistedChar(originalText[i]) || Random.Range(0, 30) != 1 ? originalText[i] : chars[Random.Range(0, 9)]);
        }
        lastText = sb.ToString();
        setText(sb.ToString());
        textTimer = Random.Range(RandomTimeRange.x, RandomTimeRange.y);
    }
    private bool IsBlacklistedChar(char c)
    {
        return
            c == '\n' ||
            c == '\r' ||
            c == '>' ||
            c == ':' ||
            c == '/' ||
            c == ')' ||
            c == ' ' ||
            c == 'L' ||
            c == 'K' ||
            c == 'I' ||
            c == 'J' ||
            c == 'W' ||
            c == 'A' ||
            c == 'D';
    }
}
