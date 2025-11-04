using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SfxClip
{
    public AudioClip clip;
    [Tooltip("이 사운드 클립의 개별 볼륨입니다. (0-1)")]
    [Range(0f, 3f)]
    public float volume = 1.0f;
}

/// <summary>
/// 여러 개의 오디오 클립을 순차적으로 재생하여 점차 고조되는 효과(셰퍼드 톤)를 만듭니다.
/// 타일 완성 효과 등에서 바운스마다 호출하여 사운드를 재생할 수 있습니다.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SfxManager : MonoBehaviour
{
    [Tooltip("순서대로 재생할 오디오 클립 리스트입니다. 음이 점점 높아지는 순서로 배치해야 합니다.")]
    [SerializeField]
    private List<SfxClip> risingSfxClips;

    private AudioSource audioSource;
    private int currentSfxIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 다음 순서의 효과음을 재생합니다. 리스트의 끝에 도달하면 처음으로 돌아갑니다.
    /// </summary>
    public void PlayRisingSfx()
    {
        if (risingSfxClips == null || risingSfxClips.Count == 0)
        {
            Debug.LogWarning("재생할 SFX 클립이 등록되지 않았습니다.");
            return;
        }

        SfxClip sfxToPlay = risingSfxClips[currentSfxIndex];

        if (sfxToPlay.clip != null)
        {
            // SoundManager의 전역 볼륨과 개별 클립의 볼륨을 곱하여 최종 볼륨을 계산합니다.
            float finalVolume = SoundManager.Instance.GetComponent<AudioSource>().volume * sfxToPlay.volume;
            audioSource.PlayOneShot(sfxToPlay.clip, finalVolume);
        }

        // 다음 인덱스로 이동하되, 리스트의 크기를 넘어가면 0으로 순환시킵니다.
        currentSfxIndex = (currentSfxIndex + 1) % risingSfxClips.Count;
    }

    /// <summary>
    /// 사운드 재생 인덱스를 처음으로 리셋합니다.
    /// </summary>
    public void ResetSfxIndex()
    {
        currentSfxIndex = 0;
    }
}
