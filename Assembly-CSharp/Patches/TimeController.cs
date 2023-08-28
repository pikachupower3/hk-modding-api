using MonoMod;
using System;
using UnityEngine;

#pragma warning disable 1591

public static class TimeController
{
    private static float slowMotionTimeScale = 1.0f;

    private static float pauseTimeScale = 1.0f;

    private static float platformBackgroundTimeScale = 1f;

    private static float genericTimeScale = 1f;

    public static float SlowMotionTimeScale
    {
        get
        {
            return TimeController.slowMotionTimeScale;
        }
        set
        {
            TimeController.SetTimeScaleFactor(ref TimeController.slowMotionTimeScale, value);
        }
    }

    public static float PauseTimeScale
    {
        get
        {
            return TimeController.pauseTimeScale;
        }
        set
        {
            TimeController.SetTimeScaleFactor(ref TimeController.pauseTimeScale, value);
        }
    }

    public static float PlatformBackgroundTimeScale
    {
        get
        {
            return TimeController.platformBackgroundTimeScale;
        }
        set
        {
            TimeController.SetTimeScaleFactor(ref TimeController.platformBackgroundTimeScale, value);
        }
    }

    public static float GenericTimeScale
    {
        get
        {
            return TimeController.genericTimeScale;
        }
        set
        {
            TimeController.SetTimeScaleFactor(ref TimeController.genericTimeScale, value);
        }
    }

    private static void SetTimeScaleFactor(ref float field, float val)
    {
        if (field != val)
        {
            field = val;
            float num = TimeController.slowMotionTimeScale * TimeController.pauseTimeScale * TimeController.platformBackgroundTimeScale * TimeController.genericTimeScale;
            if (num < 0.01f)
            {
                num = 0f;
            }
            Time.timeScale = num;
        }
    }
}
