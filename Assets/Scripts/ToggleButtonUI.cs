using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField] private string savedKey;
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite trueSprite;
    [SerializeField] private Sprite falseSprite;

    private Button _targetButton;

    private float savedValue = 1;

    private void Awake()
    {
        _targetButton = GetComponent<Button>();
    }

    private void Start()
    {
        ToggleSprite();
    }

    public void ToggleSprite()
    {
        savedValue = PlayerPrefs.GetInt(savedKey, 1);
        targetImage.sprite = savedValue == 1 ? trueSprite : falseSprite;
    }
}
