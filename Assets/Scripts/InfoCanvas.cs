using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Button noButton;
    [SerializeField] private Button yesButton;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        noButton?.onClick.AddListener(OnClickNoBtn);
        yesButton?.onClick.AddListener(OnClickYesBtn);

        _canvas.enabled = false;
    }

    public void OpenInfoCanvas()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = true;
    }

    public void OnClickNoBtn()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = false;
    }

    public void OnClickYesBtn()
    {
        SoundManager.Instance.PlaySelectSound();
        SceneManager.LoadScene(sceneName);
    }
}
