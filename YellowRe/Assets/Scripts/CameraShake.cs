using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _value = 0.25f;
    [SerializeField] private float _speed = 2.5f;
    private float _distation;
    private Vector3 _startPos;
    private Vector3 _rotation;

    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _startPos = _transform.position;
    }

    private void Update()
    {
        _distation += (_transform.position - _startPos).magnitude;
        _startPos = _transform.position;
        _rotation.z = Mathf.Sin(_distation * _speed) * _value;
        _transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, _rotation.z + Character.Singleton.Transform.eulerAngles.z);
    }
}
