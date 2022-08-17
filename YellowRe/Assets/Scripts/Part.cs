using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Part : MonoBehaviour
{
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

    public AudioSource StartAudio;
    public AudioSource NoNoAudio;
    public AudioSource HappyAudio;
    public AudioSource BeforeScreamerAudio;
    public AudioSource ScreamerAudio;
    public AudioClip[] ScarySounds;
    public AudioSource ScarySource;
    public AudioSource StartEngineSound;

    private void Start()
    {
        if(AllObjects.Singleton.PartNumber == 1)
        {
            for (int i = 0; i < _partTexts.Length; i++)
            {
                _partTexts[i].SetActive(false);
            }
            _partTexts[_currentText].SetActive(true);
        }
    }

    public void Do(ref bool action)
    {
        action = true;
        _disableTimer = 0.5f;
        _next = true;
        _currentText++;
    }

    public void GiveBottle()
    {
        AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.BootleGived);
        Destroy(AllObjects.Singleton.PartManager.Bottle);
        AllObjects.Singleton.PartManager.GiveBottleButton.SetActive(false);

        StartAudio.Stop();
        HappyAudio.Play();
    }

    public void Finish()
    {
        StartEngineSound.PlayOneShot(StartEngineSound.clip);
        AllObjects.Singleton.FinishFirstImage.SetActive(true);
        ScreamerAudio.mute = true;
        Character.Singleton.IsFinish = true;
        PlayerPrefs.SetInt("PartTwo", 1);
        StartCoroutine(FinishSecondImage());
    }

    private void Update()
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
                _partTexts[_currentText - 1].SetActive(false);
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
                    _doorClosed = true;
                }
            }
        }
    }

    IEnumerator NextText()
    {
        _partTexts[_currentText].GetComponent<Text>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText].GetComponent<Text>().color = Color.white;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText].GetComponent<Text>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        _partTexts[_currentText].GetComponent<Text>().color = Color.white;
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
        AllObjects.Singleton.FinishSecondImage.SetActive(true);
    }
}