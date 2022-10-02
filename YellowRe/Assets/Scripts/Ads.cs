using UnityEngine;
using UnityEngine.SceneManagement;
using CAS;

public class Ads : MonoBehaviour
{
    public static Ads Singleton { get; private set; }

    public bool InterstitialShowed { get; set; }
    public bool SpawnInterstitialShowed { get; set; }

    private bool _forMenu;

    public IMediationManager manager { get; set; }


    private void Start()
    {
        Singleton = this;

        if (SceneManager.GetActiveScene().name != "Main")
        {
            manager = MobileAds.BuildManager()
              .WithManagerIdAtIndex(0)
              .WithInitListener((success, error) =>
              {
              })
               .WithMediationExtras(MediationExtras.facebookDataProcessing, "LDU")
              .Initialize();

            manager.OnLoadedAd += (adType) =>
            {
            };
            manager.OnFailedToLoadAd += (adType, error) =>
            {
            };

            manager.OnInterstitialAdClosed += InterstitialAdClosedEvent;

            MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = true;

            MobileAds.settings.isExecuteEventsOnUnityThread = true;

            MobileAds.settings.analyticsCollectionEnabled = true;

            manager.SetAppReturnAdsEnabled(true);
        }
    }

    public void AdInit()
    {
        manager = MobileAds.BuildManager()
              .WithManagerIdAtIndex(0)
              .WithInitListener((success, error) =>
              {
              })
               .WithMediationExtras(MediationExtras.facebookDataProcessing, "LDU")
              .Initialize();

        manager.OnLoadedAd += (adType) =>
        {
        };
        manager.OnFailedToLoadAd += (adType, error) =>
        {
        };

        manager.OnInterstitialAdClosed += InterstitialAdClosedEvent;

        MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = true;

        MobileAds.settings.isExecuteEventsOnUnityThread = true;

        MobileAds.settings.analyticsCollectionEnabled = true;

        manager.SetAppReturnAdsEnabled(true);
    }

    void InterstitialAdClosedEvent()
    {
        if (_forMenu)
        {
            SceneManager.LoadScene("Main");
        }

        _forMenu = false;
    }

    public void GoToMenu()
    {
        if (manager.IsReadyAd(AdType.Interstitial))
        {
            _forMenu = true;
            manager.ShowAd(AdType.Interstitial);
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }
}
