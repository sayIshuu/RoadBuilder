using System;
using static VibrationManager;


public class VibrationEditor : VibrationInstance
{
    public override bool IsVibrationAvailable()
    {
        return false;
    }

    public override void Vibrate(VibrationType vibrationType, float intensity)
    {
        throw new NotSupportedException();
    }

    public override void VibrateCustom(long[] pattern, int[] amplitude)
    {
        throw new NotSupportedException();
    }
}
