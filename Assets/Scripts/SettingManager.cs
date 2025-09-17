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

    public void ChangeToDarkMode()
    {
        SoundManager.Instance.PlaySelectSound();
        ThemeManager.Instance.ToggleTheme();
    }

    public void ChangeToLightMode()
    {
        SoundManager.Instance.PlaySelectSound();
    }
}
