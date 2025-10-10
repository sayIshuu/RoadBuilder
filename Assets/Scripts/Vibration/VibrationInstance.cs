using static VibrationManager;

public abstract class VibrationInstance
{
    public abstract void Vibrate(VibrationType vibrationType, float intensity);
    public abstract void VibrateCustom(long[] pattern, int[] amplitude);

    public abstract bool IsVibrationAvailable();
}
