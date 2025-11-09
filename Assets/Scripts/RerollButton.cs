using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RerollButton : MonoBehaviour
{
    [SerializeField] private TileGenerator tileGenerator;
    //[SerializeField] private Toggle rerollToggle;
    //[SerializeField] private Image rerollButtonBackground;
    private int rerollCount;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private CanvasGroup rerollButtonCanvasGroup; // 기본 리롤 버튼 CanvasGroup
    [SerializeField] private CanvasGroup rewardedAdButtonCanvasGroup; // 보상형 광고 버튼 CanvasGroup

    // 광고 시청 제한 (게임 세션당 3회)
    private int maxAdWatchCount = 3;
    private int currentAdWatchCount = 0;

    private void Awake()
    {
        rerollCount = 3;
        UpdateRerollCountUI();
    }

    public void Reroll()
    {
        if (rerollCount == 0)
        {
            SoundManager.Instance.PlayForbidSound();
            return;
        }

        if (BoardCheck.gameover) return;
        if (BoardCheck.isAnimating) return;

        SoundManager.Instance.PlaySelectSound();
        rerollCount--;

        tileGenerator.Reroll();

        UpdateRerollCountUI();
    }

    public void PlusRerollCount()
    {
        rerollCount++;
        UpdateRerollCountUI();
    }

    // UI 업데이트 메서드
    private void UpdateRerollCountUI()
    {
        rerollCountText.text = rerollCount.ToString();

        // 리롤 횟수가 있으면 리롤 버튼 표시, 없으면 광고 버튼 표시 (토글 방식)
        if (rerollCount > 0)
        {
            // 기본 리롤 버튼 표시
            if (rerollButtonCanvasGroup != null)
            {
                rerollButtonCanvasGroup.alpha = 1f;
                rerollButtonCanvasGroup.interactable = true;
                rerollButtonCanvasGroup.blocksRaycasts = true;
            }

            // 광고 버튼 숨김
            if (rewardedAdButtonCanvasGroup != null)
            {
                rewardedAdButtonCanvasGroup.alpha = 0f;
                rewardedAdButtonCanvasGroup.interactable = false;
                rewardedAdButtonCanvasGroup.blocksRaycasts = false;
            }
        }
        else
        {
            // 리롤 버튼 숨김
            if (rerollButtonCanvasGroup != null)
            {
                rerollButtonCanvasGroup.alpha = 0f;
                rerollButtonCanvasGroup.interactable = false;
                rerollButtonCanvasGroup.blocksRaycasts = false;
            }

            // 광고 시청 기회가 남아있으면 광고 버튼 표시
            if (rewardedAdButtonCanvasGroup != null)
            {
                bool shouldShow = currentAdWatchCount < maxAdWatchCount;
                rewardedAdButtonCanvasGroup.alpha = shouldShow ? 1f : 0f;
                rewardedAdButtonCanvasGroup.interactable = shouldShow;
                rewardedAdButtonCanvasGroup.blocksRaycasts = shouldShow;
            }
        }
    }

    // 보상형 광고 시청 메서드
    public void WatchAdForReroll()
    {
        // 광고 시청 횟수 제한 체크 (세션당 3회)
        if (currentAdWatchCount >= maxAdWatchCount)
        {
            Debug.Log("[RerollButton] Maximum ad watch count reached for this session");
            SoundManager.Instance.PlayForbidSound();
            return;
        }

        if (AdsManager.I == null)
        {
            Debug.LogWarning("[RerollButton] AdsManager not available");
            return;
        }

        AdsManager.I.TryShowRewarded(
            onReward: () =>
            {
                // 광고 시청 완료 시 리롤 3회 충전
                rerollCount += 3;
                currentAdWatchCount++;
                UpdateRerollCountUI();

                SoundManager.Instance.PlaySelectSound();
                Debug.Log($"[RerollButton] Rewarded 3 rerolls from ad ({currentAdWatchCount}/{maxAdWatchCount})");
            },
            onClosed: () =>
            {
                // 광고가 닫혔을 때 (선택적 처리)
                Debug.Log("[RerollButton] Rewarded ad closed");
            }
        );
    }
}
