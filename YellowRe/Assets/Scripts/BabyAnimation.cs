using UnityEngine;

public class BabyAnimation : MonoBehaviour
{
    public static BabyAnimation Singleton { get; private set; }

    private Animator _anim;
    private string _currentAnim;

    public bool NoNoAnim;
    private float _noNoAnimTimer = 3;

    private float _scaryAudioTimer = 15;

    private float _eyeChangeTimer = 1;
    private bool _eyeIsRed;

    private void Start()
    {
        Singleton = this;
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _anim.Play(_currentAnim);

        if (NoNoAnim && _noNoAnimTimer > 0)
        {
            _noNoAnimTimer -= Time.deltaTime;

        }
        else
        {
            NoNoAnim = false;
            _noNoAnimTimer = 3;
        }

        if (AllObjects.Singleton.PartManager.FirstTouchBaby)
        {
            if (AllObjects.Singleton.PartManager.BootleGived)
            {
                if (AllObjects.Singleton.PartManager.BabyInArms)
                {
                    if (AllObjects.Singleton.PartManager.BabyScream)
                    {
                        if (AllObjects.Singleton.PartManager.BabyIsRunning)
                        {
                            _currentAnim = Anims.Run.ToString();
                            Eye3(0.25f);
                        }
                        else
                        {
                            _currentAnim = Anims.Scream.ToString();
                            Eye3(0.25f);
                        }
                    }
                    else
                    {
                        _currentAnim = Anims.Old_Man_Idle.ToString();
                        Eye2(1.5f);
                    }
                }
                else
                {
                    _currentAnim = Anims.Sitting_Clap.ToString();
                }
            }
            else
            {
                if (NoNoAnim)
                {
                    _currentAnim = Anims.Sitting_Disapproval.ToString();
                }
                else
                {

                    _currentAnim = Anims.Sitting_Laughing.ToString();

                    if (_scaryAudioTimer > 0)
                    {
                        _scaryAudioTimer -= Time.deltaTime;
                    }
                    else
                    {
                        _scaryAudioTimer = 10;
                        AllObjects.Singleton.PartManager.ScarySource.PlayOneShot(AllObjects.Singleton.PartManager.ScarySounds[Random.Range(0, AllObjects.Singleton.PartManager.ScarySounds.Length)]);
                    }
                }
            }
        }
        else
        {
            _currentAnim = Anims.Sitting_Laughing.ToString();

            if (_scaryAudioTimer > 0)
            {
                _scaryAudioTimer -= Time.deltaTime;
            }
            else
            {
                _scaryAudioTimer = 10;
                AllObjects.Singleton.PartManager.ScarySource.PlayOneShot(AllObjects.Singleton.PartManager.ScarySounds[Random.Range(0, AllObjects.Singleton.PartManager.ScarySounds.Length)]);
            }
        }
    }

    public void Eye3(float value)
    {
        if (_eyeChangeTimer > 0)
        {
            _eyeChangeTimer -= Time.deltaTime;
            if (_eyeIsRed)
            {
                for (int i = 0; i < AllObjects.Singleton.BabyEyes.Length; i++)
                {
                    AllObjects.Singleton.BabyEyes[i].material = AllObjects.Singleton.EyesMaterials[2];
                }
            }
            else
            {
                for (int i = 0; i < AllObjects.Singleton.BabyEyes.Length; i++)
                {
                    AllObjects.Singleton.BabyEyes[i].material = AllObjects.Singleton.EyesMaterials[1];
                }
            }
        }
        else
        {
            _eyeChangeTimer = value;
            _eyeIsRed = !_eyeIsRed;
        }

    }

    public void Eye2(float value)
    {
        if (_eyeChangeTimer > 0)
        {
            _eyeChangeTimer -= Time.deltaTime;
            if (_eyeIsRed)
            {
                for (int i = 0; i < AllObjects.Singleton.BabyEyes.Length; i++)
                {
                    AllObjects.Singleton.BabyEyes[i].material = AllObjects.Singleton.EyesMaterials[0];
                }
            }
            else
            {
                for (int i = 0; i < AllObjects.Singleton.BabyEyes.Length; i++)
                {
                    AllObjects.Singleton.BabyEyes[i].material = AllObjects.Singleton.EyesMaterials[1];
                }
            }
        }
        else
        {
            _eyeChangeTimer = value;
            _eyeIsRed = !_eyeIsRed;
        }

    }
}

enum Anims
{
    Sitting_Laughing, Sitting_Disapproval, Sitting_Clap, Old_Man_Idle, Scream, Run
}
