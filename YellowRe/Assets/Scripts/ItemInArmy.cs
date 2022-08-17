using UnityEngine;

public class ItemInArmy : MonoBehaviour
{
    private Transform _transform;
    private Camera _camera;
    private RaycastHit _hit;
    private float _distance = 2.5f;

    [SerializeField] private GameObject[] _takeButton;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < _distance)
        {
            Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

            if (AllObjects.Singleton.PartNumber == 1)
            {
                if (AllObjects.Singleton.PartManager.FirstTouchBaby && !AllObjects.Singleton.PartManager.BottleInArms)
                {
                    if (Physics.Raycast(rayOrigin, _camera.transform.forward, out _hit, _distance))
                    {
                        if (_hit.collider.GetComponent<ItemInArmy>())
                        {
                            for (int i = 0; i < _takeButton.Length; i++)
                            {
                                _takeButton[i].SetActive(true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Physics.Raycast(rayOrigin, _camera.transform.forward, out _hit, _distance))
                {
                    if (_hit.collider.GetComponent<ItemInArmy>())
                    {
                        for (int i = 0; i < _takeButton.Length; i++)
                        {
                            _takeButton[i].SetActive(true);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _takeButton.Length; i++)
                        {
                            _takeButton[i].SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _takeButton.Length; i++)
            {
                _takeButton[i].SetActive(false);
            }
        }
    }

    public void Take()
    {
        if (AllObjects.Singleton.PartNumber == 1)
        {
            for (int i = 0; i < _takeButton.Length; i++)
            {
                _takeButton[i].SetActive(false);
            }

            AllObjects.Singleton.PartManager.Do(ref AllObjects.Singleton.PartManager.BottleInArms);
            AllObjects.Singleton.PartManager.BabyHands[1].SetActive(true);

            _transform.parent = _camera.transform;
            _transform.localPosition = new Vector3(0, -0.2f, 1.5f);
        }
        else
        {
            for (int i = 0; i < _takeButton.Length; i++)
            {
                _takeButton[i].SetActive(false);
            }
            Destroy(gameObject);
        }
    }
}
