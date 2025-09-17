using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [SerializeField] private Button backButton;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        backButton.onClick.AddListener(CloseInfoCanvas);

        _canvas.enabled = false;
    }

    public void OpenInfoCanvas()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = true;
    }

    public void CloseInfoCanvas()
    {
        SoundManager.Instance.PlaySelectSound();
        _canvas.enabled = false;
    }
}
