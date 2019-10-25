using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UITheme.asset", menuName = "peanutbutters/UITheme", order = 10)]
public class UITheme : ScriptableObject
{
    [Header("Fonts")]
    public Font TitleFont;
    public Font SubtitleFont;
    public Font ButtonFont;
}
