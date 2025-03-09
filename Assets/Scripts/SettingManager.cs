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
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }
}
