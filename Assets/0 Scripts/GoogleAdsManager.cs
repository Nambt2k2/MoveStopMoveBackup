using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAdsManager : MonoBehaviour {
    const string AD_UNIT_ID_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    const string AD_UNIT_ID_IPHONE = "ca-app-pub-3940256099942544/2934735716";
    const string AD_UNIT_ID_OTHER = "unused";
#if UNITY_ANDROID
  string _adUnitId = AD_UNIT_ID_ANDROID;
#elif UNITY_IPHONE
  string _adUnitId = AD_UNIT_ID_IPHONE;
#else
  string _adUnitId = AD_UNIT_ID_OTHER;
#endif
    public static GoogleAdsManager instance;
    BannerView bannerView;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start() {
        //khoi tao google mobile ads sdk
        MobileAds.Initialize((InitializationStatus initStatus) => { });
        CreateBannerView();
        LoadAd();
    }

    public void LoadAd() {
        // create an instance of a banner view first.
        if (bannerView == null)
            CreateBannerView();

        // create our request used to load the ad.
        AdRequest adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
        ListenToAdEvents();
    }

    public void CreateBannerView() {
        if (bannerView != null)
            DestroyAd();
        bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
    }

    void DestroyAd() {
        if (bannerView != null) {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    void ListenToAdEvents() {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(System.String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
}
