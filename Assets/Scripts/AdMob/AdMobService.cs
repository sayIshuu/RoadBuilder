// AdMobService.cs
using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

/// <summary>
/// AdMob 광고 서비스 구현
/// </summary>
public class AdMobService : IAdsService
{
    private readonly AdMobConfig _config; // AdMob 설정

    private BannerView _bannerView; // 배너 광고
    private InterstitialAd _interstitial; // 전면 광고
    private RewardedAd _rewarded; // 보상형 광고

    private bool _initialized; // 초기화 상태

    public AdMobService(AdMobConfig config)
    {
        _config = config;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        // (선택) 테스트 기기 설정
        var requestConfig = new RequestConfiguration();
        MobileAds.SetRequestConfiguration(requestConfig);

        var tcs = new TaskCompletionSource<bool>();
        MobileAds.Initialize(initStatus => { tcs.SetResult(true); });
        await tcs.Task;

        _initialized = true;
        Debug.Log("[AdMob] Initialized");
    }

    // -------------- Banner --------------
    public void LoadBannerBottom()
    {
        DestroyBanner();

        var adUnitId = _config.GetBannerId();

        // 권장: Anchored Adaptive Banner (가로폭에 맞춤)
        // Unity 플러그인에선 full width 상수 또는 화면폭 기반으로 사이즈 결정
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);
        var request = new AdRequest();

        // 이벤트 핸들러 등록
        _bannerView.OnBannerAdLoaded += () => Debug.Log("[AdMob] Banner loaded");
        _bannerView.OnBannerAdLoadFailed += (LoadAdError err) => Debug.LogWarning($"[AdMob] Banner load failed: {err}");
        _bannerView.OnAdFullScreenContentOpened += () => Debug.Log("[AdMob] Banner opened");
        _bannerView.OnAdFullScreenContentClosed += () => Debug.Log("[AdMob] Banner closed");
        _bannerView.OnAdPaid += (AdValue val) => Debug.Log($"[AdMob] Banner paid: {val.Value} {val.CurrencyCode}");

        _bannerView.LoadAd(request);
    }

    public void ShowBanner()
    {
        if (_bannerView == null) return;
        _bannerView.Show();
    }

    public void HideBanner()
    {
        _bannerView?.Hide();
    }

    public void DestroyBanner()
    {
        _bannerView?.Destroy();
        _bannerView = null;
    }

    // -------------- Interstitial --------------
    public void PreloadInterstitial()
    {
        DisposeInterstitial();

        var adUnitId = _config.GetInterstitialId();
        var request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdMob] Interstitial load failed: {error}");
                return;
            }

            _interstitial = ad;

            // 전면광고 전/후 처리
            _interstitial.OnAdFullScreenContentOpened += () => Debug.Log("[AdMob] Interstitial opened");
            _interstitial.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("[AdMob] Interstitial closed");
                DisposeInterstitial();
                PreloadInterstitial(); // 다음 것을 미리 로드
            };
            _interstitial.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogWarning($"[AdMob] Interstitial show failed: {err}");
                DisposeInterstitial();
            };
            _interstitial.OnAdPaid += (AdValue val) => Debug.Log($"[AdMob] Interstitial paid: {val.Value} {val.CurrencyCode}");

            Debug.Log("[AdMob] Interstitial loaded");
        });
    }

    public bool IsInterstitialReady() => _interstitial != null && _interstitial.CanShowAd();

    // 전면 광고 표시
    public bool ShowInterstitial(Action onClosed = null)
    {
        if (!IsInterstitialReady()) return false;

        // 닫힘 이벤트 보장용 래핑
        void Closed()
        {
            try { onClosed?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }

        // 일회성 델리게이트 추가
        _interstitial.OnAdFullScreenContentClosed += Closed;

        _interstitial.Show();
        return true;
    }

    private void DisposeInterstitial()
    {
        _interstitial?.Destroy();
        _interstitial = null;
    }

    // -------------- Rewarded --------------
    public void PreloadRewarded()
    {
        DisposeRewarded();

        var adUnitId = _config.GetRewardedId();
        var request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdMob] Rewarded load failed: {error}");
                return;
            }

            _rewarded = ad;

            _rewarded.OnAdFullScreenContentOpened += () => Debug.Log("[AdMob] Rewarded opened");
            _rewarded.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("[AdMob] Rewarded closed");
                DisposeRewarded();
                PreloadRewarded(); // 다음 것 미리 로드
            };
            _rewarded.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogWarning($"[AdMob] Rewarded show failed: {err}");
                DisposeRewarded();
            };
            _rewarded.OnAdPaid += (AdValue val) => Debug.Log($"[AdMob] Rewarded paid: {val.Value} {val.CurrencyCode}");

            Debug.Log("[AdMob] Rewarded loaded");
        });
    }

    public bool IsRewardedReady() => _rewarded != null && _rewarded.CanShowAd();

    // 보상형 광고 표시
    public bool ShowRewarded(Action<string, double> onRewardEarned = null, Action onClosed = null)
    {
        if (!IsRewardedReady()) return false;

        // 보상 콜백: (type, amount) 전달
        _rewarded.Show((Reward reward) =>
        {
            try { onRewardEarned?.Invoke(reward.Type, reward.Amount); }
            catch (Exception e) { Debug.LogException(e); }
        });

        // 닫힘 이벤트 보장
        if (onClosed != null)
        {
            _rewarded.OnAdFullScreenContentClosed += () =>
            {
                try { onClosed.Invoke(); }
                catch (Exception e) { Debug.LogException(e); }
            };
        }

        return true;
    }

    private void DisposeRewarded()
    {
        _rewarded?.Destroy();
        _rewarded = null;
    }
}
