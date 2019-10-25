using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CreditRoller : MonoBehaviour
{
    public float ScrollDelay = 2;
    public float ScrollTime = 10;
    private static CreditRoller _instance;
    private RectTransform rectTransform;
    private Text text;

    void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<Text>();
    }

    private IEnumerator rollCredits()
    {
        text.enabled = true;
        yield return new WaitForSeconds(ScrollDelay);
        while (rectTransform.pivot.y > 0)
        {
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, rectTransform.pivot.y - Time.deltaTime / ScrollTime);
            yield return null;
        }
        rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);
    }

    public static void RollCredits()
    {
        _instance.StartCoroutine(_instance.rollCredits());
    }

    public static void ResetCredits()
    {
        _instance.text.enabled = false;
        _instance.rectTransform.pivot = new Vector2(_instance.rectTransform.pivot.x, 1);
    }
}
