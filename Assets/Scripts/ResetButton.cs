using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    //[SerializeField] private Toggle resetToggle;
    public void Reset()
    {
        SoundManager.Instance.PlaySelectSound();
        // 다시 시작 시 전면 광고 보여주기
        if (!AdsManager.I.TryShowInterstitial(() => { SceneManager.LoadScene("MainScene"); }))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
