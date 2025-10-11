using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField] private string savedKey;
    [SerializeField] private Sprite trueSprite;
    [SerializeField] private Sprite falseSprite;

    private Image _targetImg;
    private Button _targetButton;

    private float savedValue = 1;

    private void Awake()
    {
        _targetImg = GetComponent<Image>();
        _targetButton = GetComponent<Button>();
    }

    private void Start()
    {
        ToggleSprite();
    }

    public void ToggleSprite()
    {
        savedValue = PlayerPrefs.GetInt(savedKey, 1);
        _targetImg.sprite = savedValue == 1 ? trueSprite : falseSprite;
    }
}
