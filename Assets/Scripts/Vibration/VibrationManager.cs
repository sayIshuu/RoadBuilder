using System;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public enum VibrationType
    {
        //System Sound Type
        Default = 1352,
        Peek = 1519,
        Pop = 1520,

        //Impact Type
        Heavy,
        Medium,
        Light,
        Rigid,
        Soft,

        //Notification Type
        Nope,
        Error,
        Success,
        Warning,
    }

    public static VibrationManager Instance { get; private set; }
    private VibrationInstance _vibrationInstance;

    private int _intensity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }

        Init();

        int savedIntensity = PlayerPrefs.GetInt("INTENSITY", 1);
        ApplyVibrationIntensity(savedIntensity);
    }

    private void Init()
    {
        try
        {
#if UNITY_EDITOR
            _vibrationInstance = new VibrationEditor();
#elif UNITY_ANDROID
                _vibrationInstance = new VibrationAndroid();
#elif UNITY_IOS
                _vibrationInstance = new VibrationIOS();
#endif
        }
        catch (NotSupportedException e)
        {
            Debug.LogError($"Vibration: {e}");
            return;
        }
        catch (Exception e)
        {
            Debug.LogError($"Vibration: {e}");
            return;
        }

        Debug.Log($"Vibration: Initialized Successfully {_vibrationInstance}");
    }

    public void Vibrate(VibrationType vibrationType)
    {
        if (_vibrationInstance == null) return;
        if (!IsVibrationAvailable()) return;

        _vibrationInstance.Vibrate(vibrationType, _intensity);
    }

    public void VibrateCustomized(long[] pattern, int[] amplitude)
    {
#if !UNITY_ANDROID
            return;
#endif

        _vibrationInstance.VibrateCustom(pattern, amplitude);
    }

    public bool IsVibrationAvailable()
    {
        return _vibrationInstance.IsVibrationAvailable();
    }

    public void ToggleVibrationIntensity()
    {
        ApplyVibrationIntensity(_intensity == 1 ? 0 : 1);
    }

    public void ApplyVibrationIntensity(int intensity)
    {
        _intensity = intensity;

        PlayerPrefs.SetInt("INTENSITY", intensity);
        PlayerPrefs.Save();
    }
}
