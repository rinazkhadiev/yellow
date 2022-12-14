using UnityEngine;
using System.Collections;

public class BabyController : MonoBehaviour
{
    public static BabyController Singleton { get; private set; }

    private Transform _transform;
    private Camera _camera;
    private Rigidbody _rb;
    private ParticleSystem _particleSystem;

    private RaycastHit _hit;

    private bool _babyInArmy;
    [SerializeField] private float _babyDistance = 2.5f;
    [SerializeField] private BoxCollider _collider;

    [SerializeField] private float _babySecondPartDistance = 10;

    [SerializeField] private float _speed = 1;
    [SerializeField] private float _speedThirdPart = 3;

    private float _screamerTimer = 7;
    private float _screamerEndTimer = 3;
    private float _screamerToRunTimer = 2;
    private float _failImageAlpha;

    private bool _isRunning;
    private bool _isRunningEvent;

    private bool _isDead;
    private bool _afterDead;

    int _layerMask = 1 << 8;

    private Quaternion _lookRotation;

    private float _timerToLose = 100;

    public int PacifierIndex;
    private bool _pacifierButtonIsReady = true;
    public bool BabyInPosition;

    private void Start()
    {
        Singleton = this;
        _transform = GetComponent<Transform>();
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _particleSystem = GetComponent<ParticleSystem>();

        if(AllObjects.Singleton.PartNumber == 2)
        {
            _transform.position = AllObjects.Singleton.BabySecondPartTransform.position;
            Down();
            _rb.isKinematic = true;
            _collider.enabled = false;
        }
        else if (AllObjects.Singleton.PartNumber == 3)
        {
            _transform.position = AllObjects.Singleton.BabyThirdPartTransform.position;
            Down();
            _rb.isKinematic = true;
            _collider.enabled = false;
            _transform.eulerAngles = new Vector3(0, 59, 0);
        }
    }

    private void Update()
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            if (_babyInArmy)
            {
                if (AllObjects.Singleton.PartNumber == 1)
                {
                    AllObjects.Singleton.BabyButton[0].SetActive(false);
                    AllObjects.Singleton.BabyButton_Two[0].SetActive(false);
                }
                else
                {
                    AllObjects.Singleton.BabyButton[0].SetActive(true);
                    AllObjects.Singleton.BabyButton_Two[0].SetActive(true);
                }
            }
            else
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < _babyDistance)
                {
                    Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

                    if (Physics.Raycast(rayOrigin, _camera.transform.forward, out _hit, _babyDistance, _layerMask))
                    {
                        if (_hit.collider.GetComponent<BabyController>())
                        {
                            if (AllObjects.Singleton.PartNumber == 1)
                            {
                                if (AllObjects.Singleton.PartManager.BottleInArms && !AllObjects.Singleton.PartManager.BootleGived)
                                {
                                    AllObjects.Singleton.PartManager.GiveBottleButton.SetActive(true);
                                }
                                else
                                {
                                    AllObjects.Singleton.PartManager.GiveBottleButton.SetActive(false);
                                }
                            }

                            if (AllObjects.Singleton.PartNumber == 1)
                            {
                                if (!AllObjects.Singleton.PartManager.BabyIsRunning)
                                {
                                    AllObjects.Singleton.BabyButton[0].SetActive(true);
                                    AllObjects.Singleton.BabyButton_Two[0].SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            AllObjects.Singleton.BabyButton[0].SetActive(false);
                            AllObjects.Singleton.BabyButton_Two[0].SetActive(false);
                        }
                    }
                }
                else
                {
                    AllObjects.Singleton.BabyButton[0].SetActive(false);
                    AllObjects.Singleton.BabyButton_Two[0].SetActive(false);
                }
            }


            if (AllObjects.Singleton.PartManager.BabyInArms)
            {
                if (_screamerTimer > 0)
                {
                    _screamerTimer -= Time.deltaTime;
                    Up();
                }
                else
                {
                    if (_screamerEndTimer > 0)
                    {
                        AllObjects.Singleton.PartManager.BeforeScreamerAudio.Stop();
                        _screamerEndTimer -= Time.deltaTime;
                        AllObjects.Singleton.ScreamImage.SetActive(true);

                        if (!_isRunning)
                        {
                            AllObjects.Singleton.PartManager.Do(ref _isRunning);
                            AllObjects.Singleton.AnalyticsEvent.OnEvent("First_blackScreen");
                        }

                        if (_screamerEndTimer < 1)
                        {
                            AllObjects.Singleton.PartManager.BabyScream = true;
                        }
                    }
                    else
                    {
                        AllObjects.Singleton.PartManager.BeforeScreamerAudio.Stop();
                        AllObjects.Singleton.PartManager.ScreamerAudio.gameObject.SetActive(true);

                        AllObjects.Singleton.ScreamImage.SetActive(false);

                        if (_screamerToRunTimer > 0)
                        {
                            _screamerToRunTimer -= Time.deltaTime;
                            _transform.localPosition = new Vector3(0, -0.35f, 0.625f);
                        }
                        else
                        {
                            if (!AllObjects.Singleton.PartManager.BabyIsRunning)
                            {
                                _transform.position = AllObjects.Singleton.PartManager.BabyRunStartPos.position;
                                Down();
                                _rb.isKinematic = true;
                                _collider.enabled = false;
                                AllObjects.Singleton.JoystickUI.SetActive(true);
                                AllObjects.Singleton.TimerText.gameObject.SetActive(true);

                                AllObjects.Singleton.Steps[2].SetActive(false);
                                AllObjects.Singleton.Steps[3].SetActive(true);
                            }

                            AllObjects.Singleton.PartManager.BabyIsRunning = true;

                            if (!_isRunningEvent)
                            {
                                AllObjects.Singleton.AnalyticsEvent.OnEvent("First_run");
                                _isRunningEvent = true;
                            }

                            if (!AllObjects.Singleton.IsPause)
                            {
                                _lookRotation = Quaternion.LookRotation((Character.Singleton.Transform.position - _transform.position).normalized);
                                _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * 5);
                                _transform.Translate(Vector3.forward * _speed * Time.deltaTime);

                                if (_timerToLose > 95)
                                {
                                    _speed = 1;
                                }
                                else
                                {
                                    _speed += Time.deltaTime / 7;
                                }
                            }

                            if (_timerToLose > 0)
                            {
                                if (!AllObjects.Singleton.IsPause)
                                {
                                    _timerToLose -= Time.deltaTime;
                                }
                                AllObjects.Singleton.TimerText.text = $"{(int)_timerToLose}";
                            }
                            else
                            {
                                if (!Character.Singleton.IsFinish)
                                {
                                    Dead();
                                }
                            }

                            if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 2 && !Character.Singleton.IsFinish)
                            {
                                Dead();
                            }


                            if (Vector3.Distance(AllObjects.Singleton.PartManager.Car.position, Character.Singleton.Transform.position) < 5)
                            {
                                AllObjects.Singleton.StartEngineButton.SetActive(true);
                            }
                            else
                            {
                                AllObjects.Singleton.StartEngineButton.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            if (!AllObjects.Singleton.IsPause)
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > _babySecondPartDistance)
                {
                    if (Vector3.Distance(AllObjects.Singleton.BabySecondPartTransform.position, _transform.position) < 1)
                    {
                        AllObjects.Singleton.BabySecondIsRun = false;
                    }
                    else
                    {
                        _lookRotation = Quaternion.LookRotation((AllObjects.Singleton.BabySecondPartTransform.position - _transform.position).normalized);
                        _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * 5);
                        _transform.Translate(Vector3.forward * _speed * Time.deltaTime);

                        AllObjects.Singleton.BabySecondIsRun = true;
                    }

                    if(AllObjects.Singleton.RedVinetImage.color.a > 0)
                    {
                        AllObjects.Singleton.RedVinetImage.color = new Color(AllObjects.Singleton.RedVinetImage.color.r, AllObjects.Singleton.RedVinetImage.color.g, AllObjects.Singleton.RedVinetImage.color.b, AllObjects.Singleton.RedVinetImage.color.a - Time.deltaTime);
                    }

                    if (AllObjects.Singleton.PartManager.SecondPartEscape.volume > 0)
                    {
                        AllObjects.Singleton.PartManager.SecondPartEscape.volume -= Time.deltaTime;
                    }
                }
                else
                {
                    if (Mathf.Abs(_transform.position.y - Character.Singleton.Transform.position.y) < 2)
                    {
                        _lookRotation = Quaternion.LookRotation((Character.Singleton.Transform.position - _transform.position).normalized);
                        _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * 5);
                        _transform.Translate(Vector3.forward * _speed * Time.deltaTime);

                        if (AllObjects.Singleton.RedVinetImage.color.a < 0.4f)
                        {
                            AllObjects.Singleton.RedVinetImage.color = new Color(AllObjects.Singleton.RedVinetImage.color.r, AllObjects.Singleton.RedVinetImage.color.g, AllObjects.Singleton.RedVinetImage.color.b, AllObjects.Singleton.RedVinetImage.color.a + Time.deltaTime / 5);
                        }

                        AllObjects.Singleton.BabySecondIsRun = true;

                        if (AllObjects.Singleton.PartManager.SecondPartEscape.volume < 1)
                        {
                            AllObjects.Singleton.PartManager.SecondPartEscape.volume += Time.deltaTime;

                        }

                        if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 1 && !Character.Singleton.IsFinish)
                        {
                            Dead();
                        }
                    }
                }
            }
        }
        else
        {
            if (AllObjects.Singleton.PartManager.PacifierIsTaked)
            {
                AllObjects.Singleton.CurrentFindText.text = $"{(int)Vector3.Distance(_transform.position, Character.Singleton.Transform.position)}m";
                Quaternion _lookRotation = Quaternion.LookRotation((_transform.position - AllObjects.Singleton.PartManager.CameraTransform.position).normalized);
                AllObjects.Singleton.Arrow.transform.rotation = Quaternion.Slerp(AllObjects.Singleton.Arrow.transform.rotation, _lookRotation, Time.deltaTime * 5f);

                if (PacifierIndex == 0)
                {
                    if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3)
                    {
                        if (_pacifierButtonIsReady)
                        {
                            AllObjects.Singleton.PacifierGiveButton.SetActive(true);
                        }
                        else
                        {
                            AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                        }
                    }
                    else
                    {
                        AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                    }
                }
                else if (PacifierIndex == 1)
                {
                    if (Character.Singleton.InGround)
                    {
                        if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3)
                        {
                            if (_pacifierButtonIsReady)
                            {
                                AllObjects.Singleton.PacifierGiveButton.SetActive(true);
                            }
                            else
                            {
                                AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                            }
                        }
                        else
                        {
                            AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                        }
                    }
                    else
                    {
                        AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                    }
                }
                else
                {
                    if (PacifierIndex < AllObjects.Singleton.ThirdPartBabyPosition.Length)
                    {
                        if (Vector3.Distance(_transform.position, AllObjects.Singleton.ThirdPartBabyPosition[PacifierIndex].position) > 2)
                        {
                            _lookRotation = Quaternion.LookRotation((AllObjects.Singleton.ThirdPartBabyPosition[PacifierIndex].position - _transform.position).normalized);
                            _transform.rotation = Quaternion.Slerp(_transform.rotation, _lookRotation, Time.deltaTime * 5);
                            _transform.Translate(Vector3.forward * _speedThirdPart * Time.deltaTime);

                            BabyInPosition = false;
                        }
                        else
                        {
                            BabyInPosition = true;
                        }
                    }

                    if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3f)
                    {
                        if (_pacifierButtonIsReady)
                        {
                            AllObjects.Singleton.PacifierGiveButton.SetActive(true);
                        }
                        else
                        {
                            AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                        }
                    }
                    else
                    {
                        AllObjects.Singleton.PacifierGiveButton.SetActive(false);
                    }
                }

            }
            else
            {
                AllObjects.Singleton.CurrentFindText.text = $"{(int)Vector3.Distance(AllObjects.Singleton.ThirdPartPacifier.transform.position, Character.Singleton.Transform.position)}m";
                Quaternion _lookRotation = Quaternion.LookRotation((AllObjects.Singleton.ThirdPartPacifier.transform.position - AllObjects.Singleton.PartManager.CameraTransform.position).normalized);
                AllObjects.Singleton.Arrow.transform.rotation = Quaternion.Slerp(AllObjects.Singleton.Arrow.transform.rotation, _lookRotation, Time.deltaTime * 5f);
            }
        }

        if (_afterDead)
        {
            Pass();
        }
    }

    public void Take(bool up)
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            if (!AllObjects.Singleton.PartManager.FirstTouchBaby)
            {
                AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.FirstTouchBaby);
                AllObjects.Singleton.AnalyticsEvent.OnEvent("First_babyTouch");
                AllObjects.Singleton.Steps[0].SetActive(false);
                AllObjects.Singleton.Steps[1].SetActive(true);
                AllObjects.Singleton.PartManager.BabyHands[0].SetActive(true);
                BabyAnimation.Singleton.NoNoAnim = true;
                AllObjects.Singleton.PartManager.Attention();
                AllObjects.Singleton.PartManager.NoNoAudio.PlayOneShot(AllObjects.Singleton.PartManager.NoNoAudio.clip);
            }
            else
            {
                if (AllObjects.Singleton.PartManager.BottleInArms)
                {
                    if (AllObjects.Singleton.PartManager.BootleGived)
                    {
                        if (!AllObjects.Singleton.PartManager.BabyInArms)
                        {
                            Up();

                            AllObjects.Singleton.JoystickUI.SetActive(false);

                            AllObjects.Singleton.AnalyticsEvent.OnEvent("First_babyInArms");
                            AllObjects.Singleton.PartManager.BabyHands[2].SetActive(true);
                            AllObjects.Singleton.PartManager.HappyAudio.Stop();
                            AllObjects.Singleton.PartManager.BeforeScreamerAudio.Play();
                            AllObjects.Singleton.PartManager.MiddleRoomDoor.transform.localEulerAngles = new Vector3(0, 0, -60);

                            AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.BabyInArms);
                        }
                    }
                    else
                    {
                        BabyAnimation.Singleton.NoNoAnim = true;
                        AllObjects.Singleton.PartManager.Attention();
                        AllObjects.Singleton.PartManager.NoNoAudio.PlayOneShot(AllObjects.Singleton.PartManager.NoNoAudio.clip);

                    }
                }
                else
                {
                    BabyAnimation.Singleton.NoNoAnim = true;
                    AllObjects.Singleton.PartManager.Attention();
                    AllObjects.Singleton.PartManager.NoNoAudio.PlayOneShot(AllObjects.Singleton.PartManager.NoNoAudio.clip);

                }
            }
        }
        else
        {
            if (up)
            {
                Up();

            }
            else
            {
                Down();
            }
        }
    }

    private void Up()
    {
        _transform.parent = _camera.transform;
        _transform.localPosition = new Vector3(0, -0.35f, 0.85f);
        _transform.localEulerAngles = new Vector3(0, 180, 0);
        _transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _babyInArmy = true;
        _rb.isKinematic = true;
        AllObjects.Singleton.BabyButton[1].SetActive(true);
        AllObjects.Singleton.BabyButton_Two[1].SetActive(true);
    }

    private void Down()
    {
        _transform.parent = null;
        _transform.localScale = new Vector3(1, 1, 1);
        _babyInArmy = false;
        _rb.isKinematic = false;
        AllObjects.Singleton.BabyButton[1].SetActive(false);
        AllObjects.Singleton.BabyButton_Two[1].SetActive(false);
    }

    private void Dead()
    {
        if (!_isDead)
        {
            AllObjects.Singleton.FailImage.gameObject.SetActive(true);
            _failImageAlpha = 1;
            AllObjects.Singleton.PartManager.ScreamerAudio.mute = true;
            AllObjects.Singleton.LoseAudio.PlayOneShot(AllObjects.Singleton.LoseAudio.clip);
            AllObjects.Singleton.FailTexts[Random.Range(0, AllObjects.Singleton.FailTexts.Length)].SetActive(true);
            _afterDead = true;
            _isDead = true;

            if(AllObjects.Singleton.PartNumber == 1)
            {
                AllObjects.Singleton.AnalyticsEvent.OnEvent("First_dead");
            }
        }
    }

    private void Pass()
    {
        if (AllObjects.Singleton.FailImage.gameObject.activeSelf)
        {
            _failImageAlpha -= Time.deltaTime / 3f;
            AllObjects.Singleton.FailImage.color = new Color(_failImageAlpha, _failImageAlpha, _failImageAlpha, AllObjects.Singleton.FailImage.color.a);

            if (_failImageAlpha <= 0)
            {
                AllObjects.Singleton.FailMenu.SetActive(true);

                if (!Ads.Singleton.InterstitialShowed)
                {
                    if (Ads.Singleton.manager.IsReadyAd(CAS.AdType.Interstitial))
                    {
                        Ads.Singleton.manager.ShowAd(CAS.AdType.Interstitial);
                    }

                    Ads.Singleton.InterstitialShowed = true;
                }
            }
        }
    }

    public void RestartFirst()
    {
        AllObjects.Singleton.FailImage.gameObject.SetActive(false);
        Ads.Singleton.InterstitialShowed = false;

        AllObjects.Singleton.FailMenu.SetActive(false);

        for (int i = 0; i < AllObjects.Singleton.FailTexts.Length; i++)
        {
            AllObjects.Singleton.FailTexts[i].SetActive(false);
        }

        _isDead = false;
        _afterDead = false;

        if (AllObjects.Singleton.PartNumber == 1)
        {
            AllObjects.Singleton.PartManager.BabyIsRunning = false;
            _timerToLose = 100;
            _failImageAlpha = 1;
            AllObjects.Singleton.PartManager.ScreamerAudio.mute = false;
            AllObjects.Singleton.AnalyticsEvent.OnEvent("First_restart");
            Character.Singleton.StartCoroutine(Character.Singleton.TransformChange(AllObjects.Singleton.PartManager.CharacterBeforeRunPos.position));

        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            _transform.position = AllObjects.Singleton.BabySecondPartTransform.position;
            AllObjects.Singleton.AnalyticsEvent.OnEvent("Second_restart");
            Character.Singleton.StartCoroutine(Character.Singleton.TransformChange(AllObjects.Singleton.CharacterSecondPos.position));
        }
    }

    public void PacifierGive()
    {
        AllObjects.Singleton.PartManager.PacifierIsGived = true;

        AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.TeleportSound);

        if(PacifierIndex == 0)
        {
            AllObjects.Singleton.PartManager.StartAudio.Stop();
            _transform.position = AllObjects.Singleton.ThirdPartBabyPosition[PacifierIndex].position;
            AllObjects.Singleton.PartManager.Do();
        }
        else if (PacifierIndex == 1)
        {

        }
        else if (PacifierIndex == 2)
        {
            _transform.position = AllObjects.Singleton.ThirdPartBabyPosition[PacifierIndex].position;
        }
        else
        {
            if(PacifierIndex >= AllObjects.Singleton.ThirdPartBabyPosition.Length - 1)
            {
                AllObjects.Singleton.PartManager.FinishThird();
            }
        }

        AllObjects.Singleton.PartManager.HappyClick.PlayOneShot(AllObjects.Singleton.PartManager.HappyClick.clip);

        StartCoroutine(PacifierButtonOff());
        _particleSystem.Play();
        PacifierIndex++;
    }

    IEnumerator PacifierButtonOff()
    {
        _pacifierButtonIsReady = false;
        yield return new WaitForSeconds(3);
        _pacifierButtonIsReady = true;
    }
}
