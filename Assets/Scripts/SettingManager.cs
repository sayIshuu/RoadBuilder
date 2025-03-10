using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingPanel;

    void Start()
    {
        settingPanel.SetActive(false);
    }

    public void OpenSettingPanel()
    {
        SoundManager.Instance.PlaySelectSound();
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        SoundManager.Instance.PlaySelectSound();
        settingPanel.SetActive(false);
    }
}
