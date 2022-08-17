using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public static Character Singleton { get; private set; }
    public Transform Transform { get; private set; }
    public Vector3 SpawnPosition { get; set; }
    public CharacterController CharController { get; set; }

    public bool IsFinish { get; set; }

    private Transform _cameraTransform;

    private float _stepTime;
    private bool _isStepping;

    private float _playerSpeed = 5.5f;
    private float _jumpHeight = 1.0f;
    private float _gravityValue = -9.81f;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private bool _isJumping;

    private bool _isStandUp;

    private bool _inGround;

    private void Start()
    {
        Singleton = this;
        CharController = GetComponent<CharacterController>();
        Transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;

        if (PlayerPrefs.HasKey("Graphics") && PlayerPrefs.GetInt("Graphics") == 0)
        {
            AllObjects.Singleton.VolumeGameObject.SetActive(false);
        }
    }

    private void Update()
    {
        _groundedPlayer = CharController.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        if (!AllObjects.Singleton.CharacterIsBusy)
        {
            CharController.Move(AllObjects.Singleton.JoyController.Horizontal() * _cameraTransform.right * Time.deltaTime * _playerSpeed);
            CharController.Move(AllObjects.Singleton.JoyController.Vertical() * _cameraTransform.forward * Time.deltaTime * _playerSpeed);
        }
        if (!_isStepping && !_isJumping)
        {
            if (CharController.velocity.magnitude > 4f)
            {
                _stepTime = 0.45f;
                StartCoroutine(Step());
            }
            else if (CharController.velocity.magnitude > 2f && CharController.velocity.magnitude < 4f)
            {
                _stepTime = 0.8f;
                StartCoroutine(Step());
            }
            else if (CharController.velocity.magnitude < 2f && CharController.velocity.magnitude > 0.1f)
            {
                _stepTime = 1.5f;
                StartCoroutine(Step());
            }
        }

        if (_isJumping && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;

        if (!AllObjects.Singleton.CharacterIsBusy)
        {
            CharController.Move(_playerVelocity * Time.deltaTime);
        }

        if (_isStandUp && CharController.height <= 2.25f)
        {
            CharController.height += Time.deltaTime * 4f;
        }

        if (!_groundedPlayer && Transform.position.y < -10f)
        {
            Transform.position = new Vector3(Transform.position.x, 5f, Transform.position.z);
        }
    }

    public void Jump()
    {
        StartCoroutine(JumpWait());
    }

    IEnumerator JumpWait()
    {
        AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.JumpClips[Random.Range(0, AllObjects.Singleton.JumpClips.Length)]);
        _isJumping = true;
        yield return new WaitForSeconds(0.1f);
        _isJumping = false;
    }

    IEnumerator Step()
    {
        _isStepping = true;
        if (_inGround)
        {
            AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.StepClips[Random.Range(0, AllObjects.Singleton.StepClips.Length)]);

        }
        else
        {
            AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.StepClipsHouse[Random.Range(0, AllObjects.Singleton.StepClipsHouse.Length)]);
        }
        yield return new WaitForSeconds(_stepTime);
        _isStepping = false;
    }

    public void Side(bool up)
    {
        if (up)
        {
            _playerSpeed = 5.5f;
            AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.SeadDownClip);
            _isStandUp = true;
        }
        else
        {

            CharController.height = 1;
            _playerSpeed = 3;
            AllObjects.Singleton.MainSource.PlayOneShot(AllObjects.Singleton.SeadDownClip);
            _isStandUp = false;
        }
    }

    public void Respawn()
    {
        Transform.position = SpawnPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            _inGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            _inGround = false;
        }
    }

    public IEnumerator TransformChange(Vector3 position)
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        Transform.position = position;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;
    }
}
