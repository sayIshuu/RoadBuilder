using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        _canvas.enabled = false;
    }

    public void OpenSettingPanel()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = true;
    }

    public void CloseSettingPanel()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = false;
    }

    public void ChangeThemeMode()
    {
        SoundManager.Instance.PlaySelectSound();
        ThemeManager.Instance.ToggleTheme();
    }

    public void ChangeVibrationMode()
    {
        SoundManager.Instance.PlaySelectSound();
        VibrationManager.Instance.ToggleVibrationIntensity();

        if (PlayerPrefs.GetInt("INTENSITY", 1) == 1)
        {
            VibrationManager.Instance.Vibrate(VibrationManager.VibrationType.Pop);
        }
    }
}
