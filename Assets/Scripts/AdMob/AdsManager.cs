// AdsManager.cs
using System.Threading.Tasks;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    [SerializeField] private AdMobConfig config;

    public static AdsManager I { get; private set; }

    private IAdsService _ads;

    private async void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _ads = new AdMobService(config);

        await _ads.InitializeAsync();

        _ads.PreloadInterstitial();
        _ads.PreloadRewarded();

        _ads.LoadBannerBottom();
        _ads.ShowBanner();
    }

    // 외부에서 호출용 래핑
    public void ShowBottomBanner(bool show)
    {
        if (show)
        {
            _ads.LoadBannerBottom();
            _ads.ShowBanner();
        }
        else
        {
            _ads.HideBanner();
            _ads.DestroyBanner();
        }
    }

    // 전면 광고
    public bool TryShowInterstitial(System.Action onClosed = null)
    {
        if (_ads.IsInterstitialReady())
            return _ads.ShowInterstitial(onClosed);
        else
            _ads.PreloadInterstitial();
        return false;
    }

    // 보상형 광고
    public bool TryShowRewarded(System.Action onReward, System.Action onClosed = null)
    {
        return _ads.ShowRewarded(
            onRewardEarned: (type, amount) => onReward?.Invoke(),
            onClosed: onClosed
        );
    }
}
