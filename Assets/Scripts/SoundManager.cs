using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource; // Sfx 오디오 소스
    [SerializeField] private AudioSource mainAudioSource; // Bgm 오디오

    [SerializeField] private AudioClip displaySound;
    [SerializeField] private float displaySoundVolume;
    [SerializeField] private AudioClip displaySound2;
    [SerializeField] private float displaySound2Volume;
    [SerializeField] private AudioClip displaySound3;
    [SerializeField] private float displaySound3Volume;



    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private float levelUpSoundVolume;
    [SerializeField] private AudioClip forbidSound;
    [SerializeField] protected float forbidSoundVolume;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private float gameOverSoundVolume;
    [SerializeField] private AudioClip getScoreSound;
    [SerializeField] private float getScoreSoundVolume;
    [SerializeField] private AudioClip getScoreSound2;
    [SerializeField] private float getScoreSound2Volume;
    [SerializeField] private AudioClip getLargeScoreSound;
    [SerializeField] private float getLargeScoreSoundVolume;
    [SerializeField] private AudioClip slideSound;
    [SerializeField] private float slideSoundVolume;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private float selectSoundVolume;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }

        float bgmVol = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);

        ChangeBgmVolume(bgmVol);
        ChangeSfxVolume(sfxVol);
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }


    public void PlayDisplaySound()
    {
        // 랜덤으로 displaySound1, displaySound2, displaySound3 중 하나 재생
        int random = Random.Range(0, 3);
        if (random == 0) PlaySound(displaySound, displaySoundVolume);
        else if (random == 1) PlaySound(displaySound2, displaySound2Volume);
        else PlaySound(displaySound3, displaySound3Volume);
    }

    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound, levelUpSoundVolume);
    }

    public void PlayForbidSound()
    {
        PlaySound(forbidSound, forbidSoundVolume);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound, gameOverSoundVolume);
    }

    public void PlayScoreSound()
    {
        int random = Random.Range(0, 2);
        if (random == 0) PlaySound(getScoreSound, getScoreSoundVolume);
        else PlaySound(getScoreSound2, getScoreSound2Volume);
    }

    public void PlayLargeScoreSound()
    {
        PlaySound(getLargeScoreSound, getLargeScoreSoundVolume);
    }

    public void PlaySlideSound()
    {
        PlaySound(slideSound, slideSoundVolume);
    }

    public void PlaySelectSound()
    {
        PlaySound(selectSound, selectSoundVolume);
    }

    public void ChangeBgmVolume(float volume)
    {
        mainAudioSource.volume = volume;
        PlayerPrefs.SetFloat("BGM_VOLUME", volume);
        PlayerPrefs.Save();
    }

    public void ChangeSfxVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("SFX_VOLUME", volume);
        PlayerPrefs.Save();
    }
}
