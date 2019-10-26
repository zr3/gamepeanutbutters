using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour {

    public float fadeTime = 0.4f;

    private Image image;
    private static ScreenFader _instance;

    void Awake() {
        _instance = this.CheckSingleton(_instance);
        image = GetComponent<Image>();

        var newColor = image.color;
        newColor.a = 1;
        image.color = newColor;
        image.CrossFadeAlpha(0, fadeTime, true);
    }

    /// <summary>
    /// Fades the screen out over the configured amount of time.
    /// </summary>
    public static void FadeOut() => _instance.image.CrossFadeAlpha(1, _instance.fadeTime, true);

    /// <summary>
    /// Fades the screen out over the configured amount of time.
    /// </summary>
    /// <param name="predelay">Time to wait before fading.</param>
    public static void FadeOut(float predelay) => _instance.StartCoroutine(WaitAndRunAction(predelay, FadeOut));

    /// <summary>
    /// Fades the screen in over the configured amount of time.
    /// </summary>
    public static void FadeIn() => _instance.image.CrossFadeAlpha(0, _instance.fadeTime, true);

    /// <summary>
    /// Fades the screen in over the configured amount of time.
    /// </summary>
    /// <param name="predelay">Time to wait before fading.</param>
    public static void FadeIn(float predelay) => _instance.StartCoroutine(WaitAndRunAction(predelay, FadeIn));

    /// <summary>
    /// Fades the screen out, then perform an action.
    /// </summary>
    /// <param name="action">The action to perform after fading out.</param>
    /// <param name="predelay">Time to wait before fading.</param>
    /// <param name="postdelay">Time to wait after fading but before performing the action.</param>
    public static void FadeOutThen(Action action, float predelay = 0f, float postdelay = 0f)
    {
        FadeOut(predelay);
        _instance.StartCoroutine(WaitAndRunAction(_instance.fadeTime + predelay + postdelay, action));
    }

    /// <summary>
    /// Fades the screen in, then perform an action.
    /// </summary>
    /// <param name="action">The action to perform after fading in.</param>
    /// <param name="predelay">Time to wait before fading.</param>
    /// <param name="postdelay">Time to wait after fading but before performing the action.</param>
    public static void FadeInThen(Action action, float predelay = 0f, float postdelay = 0f)
    {
        FadeIn(predelay);
        _instance.StartCoroutine(WaitAndRunAction(_instance.fadeTime + predelay + postdelay, action));
    }

    private static IEnumerator WaitAndRunAction(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}
