using UnityEngine;
using UnityEngine.InputSystem;

public class CreditPanel : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private RectTransform creditRect;
    [SerializeField] private float normalSpeed = 100f;
    [SerializeField] private float fastSpeed = 300f;
    [SerializeField] private float endPos = 2400f;

    private bool _isPressed;
    private bool _isStarted;

    private void Awake()
    {
        creditsPanel.gameObject.SetActive(false);
    }

    public void OnCreditPanel()
    {
        // 크래딧 초기화
        ResetCreditsUI();

        // 크래딧 패널 활성화
        SoundManager.Instance.PlaySelectSound();
        creditsPanel.gameObject.SetActive(true);
    }

    public void OffCreditPanel()
    {
        // 크래딧 패널 비활성화
        SoundManager.Instance.PlaySelectSound();
        creditsPanel.gameObject.SetActive(false);

        _isStarted = false;
    }

    private void ResetCreditsUI()
    {
        var pos = creditRect.anchoredPosition;
        pos.y = 0;
        creditRect.anchoredPosition = pos;

        _isPressed = false;
        _isStarted = true;
    }

    private void Update()
    {
        if (!_isStarted) return;

        // 목표 도달 시 정지
        if (creditRect.anchoredPosition.y >= endPos)
        {
            _isPressed = false;
            return;
        }

        // 에디터 마우스 입력
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                _isPressed = true;
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                _isPressed = false;
        }

        // 모바일 터치 입력
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
                _isPressed = true;
            else if (touch.press.wasReleasedThisFrame)
                _isPressed = false;
        }

        // 스크롤 속도 제어
        float speed = _isPressed ? fastSpeed : normalSpeed;
        creditRect.anchoredPosition += Vector2.up * (speed * Time.deltaTime);
    }
}
