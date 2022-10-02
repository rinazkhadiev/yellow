using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Part : MonoBehaviour
{
    [SerializeField] private GameObject[] _firstPartObjects;
    [SerializeField] private GameObject[] _secondPartObjects;
    [SerializeField] private GameObject[] _thirdPartObjects;

    public bool FirstTouchBaby;
    public bool BottleInArms;
    public bool BootleGived;
    public bool BabyInArms;
    public bool BabyScream;
    public bool BabyIsRunning;

    public Transform Car;
    public GameObject Bottle;
    public GameObject GiveBottleButton;
    public GameObject MiddleRoomDoor;
    public GameObject[] BabyHands;
    public Transform BabyRunStartPos;

    [SerializeField] private GameObject[] _partTexts;
    [SerializeField] private GameObject _doorObject;
    [SerializeField] private AudioSource _doorClose;
    public Transform CharacterBeforeRunPos;

    private int _currentText;
    private float _disableTimer;
    private bool _next;
    private bool _doorClosed;

    [SerializeField] private GameObject _carLigths;
    private float _carLightTimer;
    private bool _isCarLight;

    public AudioSource StartAudio;
    public AudioSource NoNoAudio;
    public AudioSource HappyAudio;
    public AudioSource BeforeScreamerAudio;
    public AudioSource ScreamerAudio;
    public AudioClip[] ScarySounds;
    public AudioSource ScarySource;
    public AudioSource StartEngineSound;
    public AudioSource SecondPartEscape;
    public AudioSource HappyClick;


    private int _itemsInArms;
    [SerializeField] private GameObject[] _babyItems;

    private bool _isFirstOpen;
    public Transform CameraTransform;
    private int _currentFindItem;

    public bool PacifierIsTaked;
    public bool PacifierIsGived;

    private void Start()
    {
        CameraTransform = Camera.main.transform;

        if(AllObjects.Singleton.PartNumber == 1)
        {
            for (int i = 0; i < _firstPartObjects.Length; i++)
            {
                _firstPartObjects[i].SetActive(true);
            }

            for (int i = 0; i < _partTexts.Length; i++)
            {
                _partTexts[i].SetActive(false);
            }
            _partTexts[_currentText].SetActive(true);

            AllObjects.Singleton.AnalyticsEvent.OnEvent("First_loaded");
            StartAudio.Play();

        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            for (int i = 0; i < _secondPartObjects.Length; i++)
            {
                _secondPartObjects[i].SetActive(true);
            }

            AllObjects.Singleton.Doors[0].transform.localEulerAngles = new Vector3(0, 0, -180);
            AllObjects.Singleton.Doors[1].transform.localEulerAngles = new Vector3(0, 0, -60);
            AllObjects.Singleton.Doors[2].transform.localEulerAngles = new Vector3(0, 0, 230);

            AllObjects.Singleton.AnalyticsEvent.OnEvent("Second_loaded");
            SecondPartEscape.Play();

            for (int i = 0; i < _partTexts.Length; i++)
            {
                _partTexts[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _thirdPartObjects.Length; i++)
            {
                _thirdPartObjects[i].SetActive(true);
            }

            AllObjects.Singleton.Doors[0].transform.localEulerAngles = new Vector3(0, 0, -180);
            AllObjects.Singleton.Doors[1].transform.localEulerAngles = new Vector3(0, 0, -60);
            AllObjects.Singleton.Doors[2].transform.localEulerAngles = new Vector3(0, 0, 230);

            AllObjects.Singleton.AnalyticsEvent.OnEvent("Third_loaded");

            StartAudio.Play();

            for (int i = 0; i < _partTexts.Length; i++)
            {
                _partTexts[i].SetActive(false);
            }

            _partTexts[_currentText + 6].SetActive(true);

        }

        if (!Ads.Singleton.SpawnInterstitialShowed)
        {
            if (Ads.Singleton.manager.IsReadyAd(CAS.AdType.Interstitial))
            {
                Ads.Singleton.manager.ShowAd(CAS.AdType.Interstitial);
            }

            Ads.Singleton.SpawnInterstitialShowed = true;
        }
    }

    public void Do(ref bool action)
    {
        action = true;
        _disableTimer = 0.5f;
        _next = true;
        _currentText++;
    }

    public void Do()
    {
        _disableTimer = 0.5f;
        _next = true;
        _currentText++;
    }

    public void GiveBottle()
    {
        AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.BootleGived);
        AllObjects.Singleton.AnalyticsEvent.OnEvent("First_bottleGived");
        Destroy(AllObjects.Singleton.PartManager.Bottle);
        AllObjects.Singleton.PartManager.GiveBottleButton.SetActive(false);

        AllObjects.Singleton.Steps[2].SetActive(false);

        StartAudio.Stop();
        HappyAudio.Play();
    }

    public void Finish()
    {
        StartEngineSound.PlayOneShot(StartEngineSound.clip);
        AllObjects.Singleton.AnalyticsEvent.OnEvent("First_finish");
        AllObjects.Singleton.FinishFirstImage.SetActive(true);
        ScreamerAudio.mute = true;
        Character.Singleton.IsFinish = true;
        PlayerPrefs.SetInt("PartTwo", 1);
        StartCoroutine(FinishSecondImage());
    }

    public void FinishSecond()
    {
        _itemsInArms++;

        if (_itemsInArms >= 8)
        {
            StartEngineSound.PlayOneShot(HappyAudio.clip);
            AllObjects.Singleton.AnalyticsEvent.OnEvent("Second_finish");
            AllObjects.Singleton.FinishFirstImage.SetActive(true);
            Character.Singleton.IsFinish = true;
            PlayerPrefs.SetInt("PartThird", 1);
            StartCoroutine(FinishSecondImage());
        }
    }

    public void FinishThird()
    {
        StartEngineSound.PlayOneShot(HappyAudio.clip);
        AllObjects.Singleton.AnalyticsEvent.OnEvent("Third_finish");
        AllObjects.Singleton.FinishFirstImage.SetActive(true);
        Character.Singleton.IsFinish = true;
        StartCoroutine(FinishSecondImage());
    }

    private void Update()
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            if (_next)
            {
                if (_disableTimer > 0)
                {
                    _disableTimer -= Time.deltaTime;
                    _partTexts[_currentText - 1].transform.position = new Vector2(_partTexts[_currentText - 1].transform.position.x, _partTexts[_currentText - 1].transform.position.y + Time.deltaTime * 100);
                }
                else
                {
                    for (int i = 0; i < _partTexts.Length; i++)
                    {
                        _partTexts[i].SetActive(false);
                    }

                    _partTexts[_currentText].SetActive(true);
                    StartCoroutine(NextText());
                    _next = false;
                }
            }

            if (AllObjects.Singleton.PartManager.Bottle != null)
            {
                if (Vector3.Distance(AllObjects.Singleton.PartManager.Bottle.transform.position, Character.Singleton.Transform.position) < 6)
                {
                    if (!_doorClosed)
                    {
                        _doorObject.transform.localEulerAngles = new Vector3(0, 0, 90);
                        _doorClose.PlayOneShot(_doorClose.clip);
                        AllObjects.Singleton.AnalyticsEvent.OnEvent("First_doorClosed");
                        _doorClosed = true;
                    }
                }
            }

            if (BabyIsRunning)
            {
                if (_carLightTimer > 0)
                {
                    _carLightTimer -= Time.deltaTime;
                }
                else
                {
                    CarLight();
                    _carLightTimer = 2;
                }
            }
        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            if (_itemsInArms < 8)
            {
                if (_babyItems[_currentFindItem].activeSelf)
                {
                    AllObjects.Singleton.CurrentFindText.text = $"{(int)Vector3.Distance(_babyItems[_currentFindItem].transform.position, Character.Singleton.Transform.position)}m";

                    Quaternion _lookRotation = Quaternion.LookRotation((_babyItems[_currentFindItem].transform.position - CameraTransform.position).normalized);
                    AllObjects.Singleton.Arrow.transform.rotation = Quaternion.Slerp(AllObjects.Singleton.Arrow.transform.rotation, _lookRotation, Time.deltaTime * 5f);

                    float _yDistance = Character.Singleton.Transform.position.y - _babyItems[_currentFindItem].transform.position.y;


                    if (_yDistance < -2)
                    {
                        AllObjects.Singleton.WhatIsFloorDown.SetActive(false);
                        AllObjects.Singleton.WhatIsFloorUp.SetActive(true);
                    }
                    else if (_yDistance > 2)
                    {

                        AllObjects.Singleton.WhatIsFloorDown.SetActive(true);
                        AllObjects.Singleton.WhatIsFloorUp.SetActive(false);
                    }
                    else
                    {
                        AllObjects.Singleton.WhatIsFloorDown.SetActive(false);
                        AllObjects.Singleton.WhatIsFloorUp.SetActive(false);
                    }
                }
                else
                {
                    _currentFindItem++;
                }
            }
            else
            {
                AllObjects.Singleton.CurrentFindText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_next)
            {
                if (_disableTimer > 0)
                {
                    _disableTimer -= Time.deltaTime;
                    _partTexts[_currentText - 1 + 6].transform.position = new Vector2(_partTexts[_currentText - 1 + 6].transform.position.x, _partTexts[_currentText - 1 + 6].transform.position.y + Time.deltaTime * 100);
                }
                else
                {
                    _partTexts[_currentText - 1 + 6].SetActive(false);
                    _partTexts[_currentText + 6].SetActive(true);
                    StartCoroutine(NextText());
                    _next = false;
                }
            }
        }
    }

    private void CarLight()
    {
        if (_isCarLight)
        {
            _carLigths.SetActive(false);
            _isCarLight = false;
        }
        else
        {
            _carLigths.SetActive(true);
            _isCarLight = true;
        }
    }

    IEnumerator NextText()
    {
        if (AllObjects.Singleton.PartNumber == 3)
        {
            _partTexts[_currentText + 6].GetComponent<Text>().color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText + 6].GetComponent<Text>().color = Color.white;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText + 6].GetComponent<Text>().color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText + 6].GetComponent<Text>().color = Color.white;
        }
        else
        {
            _partTexts[_currentText].GetComponent<Text>().color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText].GetComponent<Text>().color = Color.white;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText].GetComponent<Text>().color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            _partTexts[_currentText].GetComponent<Text>().color = Color.white;
        }
    }

    public void Attention()
    {
        StartCoroutine(AttentionToText());
    }

    public IEnumerator AttentionToText()
    {
        _partTexts[_currentText -1].GetComponent<Text>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText -1].GetComponent<Text>().color = Color.white;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText -1].GetComponent<Text>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText -1].GetComponent<Text>().color = Color.white;
    }

    private IEnumerator FinishSecondImage()
    {
        yield return new WaitForSeconds(3);

        if (PlayerPrefs.GetInt("Session") == 2)
        {
            Debug.Log("A");
            AllObjects.Singleton.ReviewScript.OnReview();
        }

        if (AllObjects.Singleton.PartNumber == 3)
        {
            AllObjects.Singleton.FinishThirdImage.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.FinishSecondImage.SetActive(true);
        }
    }

    public void FindAllToysOpen()
    {
        if (!_isFirstOpen)
        {
            StartCoroutine(FindAllToys());
            _isFirstOpen = true;
        }
    }

    private IEnumerator FindAllToys()
    {
        AllObjects.Singleton.FindAllToysButton.SetActive(true);
        yield return new WaitForSeconds(10);
        AllObjects.Singleton.FindAllToysButton.SetActive(false);
    }
}