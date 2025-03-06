using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioClip displaySound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip forbidSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip getScoreSound;

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
    }

    // 사운드 재생 메서드
    public void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayDisplaySound()
    {
        PlaySound(displaySound, 1.0f);
    }

    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound, 1.0f);
    }

    public void PlayForbidSound()
    {
        PlaySound(forbidSound, 0.5f);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound, 0.6f);
    }

    public void PlayScoreSound()
    {
        PlaySound(getScoreSound, 0.5f);
    }
}
