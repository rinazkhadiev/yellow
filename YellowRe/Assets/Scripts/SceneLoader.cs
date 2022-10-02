using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton { get; private set; }

    [SerializeField] private Image _loadBarImage;
    [SerializeField] private Text _loadText;

    private string _language;
    private int _graphics;

    [SerializeField] private GameObject _highSettings;
    [SerializeField] private GameObject _lowSettings;

    [SerializeField] private Button _firstPartButton;
    [SerializeField] private Button _secondPartButton;
    [SerializeField] private Button _thirdPartButton;

    [SerializeField] private GameObject _soonText;
    [SerializeField] private GameObject[] _partTexts;

    private void Start()
    {
        Singleton = this;

        if(SceneManager.GetActiveScene().name == "Main")
        {
            if (PlayerPrefs.HasKey("Session"))
            {
                PlayerPrefs.SetInt("Session", 2);
            }
            else
            {
                PlayerPrefs.SetInt("Session", 1);
            }

            if (PlayerPrefs.HasKey("Graphics") && PlayerPrefs.GetInt("Graphics") == 0)
            {
                _highSettings.SetActive(false);
                _lowSettings.SetActive(true);
            }

            if (PlayerPrefs.HasKey("PartTwo"))
            {
                _secondPartButton.interactable = true;

                if (PlayerPrefs.HasKey("PartThird"))
                {
                    _thirdPartButton.interactable = true;
                    _partTexts[2].SetActive(true);
                    _thirdPartButton.gameObject.GetComponent<Animation>().Play();
                }
                else
                {
                    _secondPartButton.gameObject.GetComponent<Animation>().Play();
                    _partTexts[1].SetActive(true);
                }
            }
            else
            {
                _firstPartButton.gameObject.GetComponent<Animation>().Play();
                _partTexts[0].SetActive(true);
            }

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                }
            });
        }
    }

    public void Graphics(int value)
    {
        PlayerPrefs.SetInt("Graphics", value);
    }

    public void StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartPlay(int partNumber)
    {
        _loadBarImage.gameObject.SetActive(true);
        _loadText.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Part", partNumber);
        StartCoroutine(AsyncLoad());
    }

    public void OpenSite(string url)
    {
        Application.OpenURL(url);
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation opertaion = SceneManager.LoadSceneAsync("Play");
        while (!opertaion.isDone)
        {
            float progress = opertaion.progress / 0.9f;
            if (progress <= 0.98f)
            {
                _loadBarImage.fillAmount = progress;
                _loadText.text = string.Format("{0:0}%", progress * 100);
            }
            yield return null;
        }
    }
}
