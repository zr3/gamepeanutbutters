using System;
using UnityEngine;

public class WaitForSecondsOr : CustomYieldInstruction
{
    private float targetTime;
    private Func<bool> predicate;
    private bool ignoreTimeScale;

    public WaitForSecondsOr(float seconds, Func<bool> predicate, bool ignoreTimeScale = false)
    {
        this.predicate = predicate;
        targetTime = (ignoreTimeScale ? Time.unscaledTime : Time.time) + seconds;
        this.ignoreTimeScale = ignoreTimeScale;
    }

    public override bool keepWaiting => (ignoreTimeScale ? Time.unscaledTime : Time.time) < targetTime && !predicate();
}
