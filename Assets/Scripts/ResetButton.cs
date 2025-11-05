using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    public void Reset()
    {
        //// 목숨부족시 재시작 불가.
        //if (LifeManager.Instance.CurrentLives <= 0)
        //{
        //    // todo : 광고유도 - 동영님
        //    // 광고보면 LifeManager.Instance.RecoverAllLife(); 호출
        //    return;
        //}

        // 메인씬에서만 광고 타이머 체크
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            // 5분 타이머가 완료되었는지 체크
            if (GameplayAdTimer.Instance != null && GameplayAdTimer.Instance.ShouldShowInterstitial())
            {
                // 전면광고 표시
                SoundManager.Instance.PlaySelectSound();

                AdsManager.I.TryShowInterstitial(onClosed: () =>
                {
                    // 광고가 닫힌 후 타이머 리셋
                    GameplayAdTimer.Instance.ResetTimer();

                    // 씬 재시작
                    SceneManager.LoadScene("MainScene");
                });

                return; // 광고 표시 후 함수 종료
            }
        }

        // 타이머가 완료되지 않았거나 튜토리얼씬인 경우 바로 재시작
        SoundManager.Instance.PlaySelectSound();
        SceneManager.LoadScene("MainScene");
        //LifeManager.Instance.UseLife();
    }
}
