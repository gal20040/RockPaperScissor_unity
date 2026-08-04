using admob;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public string admobBannerID = "ca-app-pub-3940256099942544/2934735716";

    public string admobInterstitialID = "ca-app-pub-3940256099942544/2934735716";

    public string admobVideoID = "ca-app-pub-3940256099942544/2934735716";

    private Admob ad;

    private void Awake() => Object.DontDestroyOnLoad(base.gameObject);

    private void Start() => initAdmob();

    private void initAdmob()
    {
        ad = Admob.Instance();
        ad.bannerEventHandler += onBannerEvent;
        ad.interstitialEventHandler += onInterstitialEvent;
        ad.rewardedVideoEventHandler += onRewardedVideoEvent;
        ad.nativeBannerEventHandler += onNativeBannerEvent;
        ad.InitAdmob(admobBannerID, admobInterstitialID);
        UnityEngine.Debug.Log("Admob Inited.");
        Admob.Instance().ShowBannerRelative(AdSize.Banner, AdPosition.BOTTOM_CENTER, 0);
        ad.loadInterstitial();
    }

    public void showInterstitial()
    {
        MonoBehaviour.print("Request for Full AD.");
        if (ad.isInterstitialReady())
        {
            ad.showInterstitial();
        }
    }

    private void onInterstitialEvent(string eventName, string msg)
    {
        UnityEngine.Debug.Log("handler onAdmobEvent---" + eventName + "   " + msg);
        if (eventName == AdmobEvent.onAdLoaded)
        {
            Admob.Instance().showInterstitial();
        }
    }

    private void onBannerEvent(string eventName, string msg) => UnityEngine.Debug.Log("handler onAdmobBannerEvent---" + eventName + "   " + msg);

    private void onRewardedVideoEvent(string eventName, string msg) => UnityEngine.Debug.Log("handler onRewardedVideoEvent---" + eventName + "   " + msg);

    private void onNativeBannerEvent(string eventName, string msg) => UnityEngine.Debug.Log("handler onAdmobNativeBannerEvent---" + eventName + "   " + msg);
}
