using UnityEngine;

[CreateAssetMenu(fileName = "AdMobConfig", menuName = "Scriptable Objects/AdMobConfig")]
public class AdMobConfig : ScriptableObject
{
    [Header("Use test ad unit ids during development")]
    public bool useTestIds = true;

    [Header("Android - Real Ad Unit IDs")]
    public string androidAppId;
    public string androidBannerId;
    public string androidInterstitialId;
    public string androidRewardedId;

    [Header("Android - Test Ad Unit IDs")]
    public string testBannerId        = "ca-app-pub-3940256099942544/6300978111";
    public string testInterstitialId  = "ca-app-pub-3940256099942544/1033173712";
    public string testRewardedId      = "ca-app-pub-3940256099942544/5224354917";

    public string GetBannerId()       => useTestIds ? testBannerId       : androidBannerId;
    public string GetInterstitialId() => useTestIds ? testInterstitialId : androidInterstitialId;
    public string GetRewardedId()     => useTestIds ? testRewardedId     : androidRewardedId;
}
