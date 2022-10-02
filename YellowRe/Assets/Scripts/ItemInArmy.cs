using UnityEngine;
using UnityEngine.UI;

public class ItemInArmy : MonoBehaviour
{
    private Transform _transform;
    private Camera _camera;
    private RaycastHit _hit;
    private float _distance = 2.5f;

    [SerializeField] private GameObject _takeButton;

    [SerializeField] private GameObject _imageInTop;
    [SerializeField] private Image _imageInPanel;


    private void Start()
    {
        _transform = GetComponent<Transform>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < _distance)
            {
                Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

                if (!AllObjects.Singleton.PartManager.BottleInArms)
                {
                    if (Physics.Raycast(rayOrigin, _camera.transform.forward, out _hit, _distance))
                    {
                        if (_hit.collider.GetComponent<ItemInArmy>())
                        {
                            _takeButton.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                _takeButton.SetActive(false);
            }
        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3.25f)
            {
                _takeButton.SetActive(true);
            }
            else
            {
                _takeButton.SetActive(false);
            }

        }
        else
        {
            if (!AllObjects.Singleton.PartManager.PacifierIsTaked)
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3.25f)
                {
                    _takeButton.SetActive(true);
                }
                else
                {
                    _takeButton.SetActive(false);
                }
            }
            else
            {
                _takeButton.SetActive(false);
            }
        }
    }

    public void Take()
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            _takeButton.SetActive(false);

            if (!AllObjects.Singleton.PartManager.FirstTouchBaby)
            {
                AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.FirstTouchBaby);
                AllObjects.Singleton.AnalyticsEvent.OnEvent("First_babyTouch");
            }

            AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.BottleInArms);
            AllObjects.Singleton.PartManager.BabyHands[1].SetActive(true);
            AllObjects.Singleton.AnalyticsEvent.OnEvent("First_bottleTake");
            AllObjects.Singleton.Steps[0].SetActive(false);
            AllObjects.Singleton.Steps[1].SetActive(false);
            AllObjects.Singleton.Steps[2].SetActive(true);

            _transform.parent = _camera.transform;
            _transform.localPosition = new Vector3(0, -0.2f, 1.5f);
        }
        else if (AllObjects.Singleton.PartNumber == 2)
        {
            _takeButton.SetActive(false);

            _imageInPanel.color = new Color(_imageInPanel.color.r, _imageInPanel.color.g, _imageInPanel.color.b, 1f);

            AllObjects.Singleton.PartManager.FinishSecond();

            AllObjects.Singleton.AnalyticsEvent.OnEvent($"Second_{gameObject.name}");

            Destroy(_imageInTop);
            gameObject.SetActive(false);
        }
        else
        {
            AllObjects.Singleton.AnalyticsEvent.OnEvent("Third_pacifierTaked");
            AllObjects.Singleton.PartManager.PacifierIsTaked = true;
            _takeButton.SetActive(false);

            AllObjects.Singleton.PartManager.Do();

            gameObject.SetActive(false);
        }
    }
}
