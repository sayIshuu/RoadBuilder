using System;
using System.Threading.Tasks;

public interface IAdsService
{
    Task InitializeAsync(); // MobileAds init (+ consent flow optional)
    // Banner
    void LoadBannerBottom();
    void ShowBanner();
    void HideBanner();
    void DestroyBanner();

    // Interstitial
    void PreloadInterstitial();
    bool IsInterstitialReady();
    bool ShowInterstitial(Action onClosed = null);

    // Rewarded
    void PreloadRewarded();
    bool IsRewardedReady();
    bool ShowRewarded(Action<string, double> onRewardEarned = null, Action onClosed = null);
}
