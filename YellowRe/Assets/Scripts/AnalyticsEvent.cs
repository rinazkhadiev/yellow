using UnityEngine;

public class AnalyticsEvent : MonoBehaviour
{
    public void OnEvent(string eventName)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
    }
}

