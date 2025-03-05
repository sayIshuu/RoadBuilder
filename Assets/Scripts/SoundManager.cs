using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioClip displaySound;  
    [SerializeField] private AudioClip levelUpSound;

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
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayDisplaySound()
    {
        PlaySound(displaySound);
    }

    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound);
    }
}
