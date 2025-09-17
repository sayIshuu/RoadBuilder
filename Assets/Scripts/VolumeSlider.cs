using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    private enum SliderType  {Bgm, Sfx}

    [SerializeField] private SliderType sliderType;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        _slider.value = GetSavedVolume();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private float GetSavedVolume()
    {
        float savedVolume = 0;

        switch (sliderType)
        {
            case SliderType.Bgm:
                savedVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);;
                break;
            case SliderType.Sfx:
                savedVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);;
                break;
        }

        return savedVolume;
    }

    private void OnSliderValueChanged(float volume)
    {
        switch (sliderType)
        {
            case SliderType.Bgm:
                SoundManager.Instance.ChangeBgmVolume(volume);
                break;
            case SliderType.Sfx:
                SoundManager.Instance.ChangeSfxVolume(volume);
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySelectSound();
    }
}
