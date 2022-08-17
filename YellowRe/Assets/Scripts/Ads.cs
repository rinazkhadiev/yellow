using UnityEngine;

public class Ads : MonoBehaviour
{
    public static Ads Singleton { get; private set; }

    public bool InterstitialShowed { get; set; }

    private void Start()
    {
        Singleton = this;
    }
}
