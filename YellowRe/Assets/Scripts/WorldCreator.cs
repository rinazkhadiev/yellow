using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WorldCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] _objs;
    [SerializeField] private int[] _valueofobj;
    [SerializeField] private Transform _parent;

    [SerializeField] private int _distanceForObject;

    [SerializeField] private int _xPosA;
    [SerializeField] private int _xPosB;
    [SerializeField] private int _zPosA;
    [SerializeField] private int _zPosB;

    private Vector3 _position;
    private List<Vector3> _pastObjectPosition = new List<Vector3>();

    private void Start()
    {
        _pastObjectPosition.Add(new Vector3(0, 0, 0));

        for (int i = 0; i < _valueofobj.Length; i++)
        {
            for (int j = 0; j < _valueofobj[i]; j++)
            {
                _position = new Vector3(Random.Range(_xPosA, _xPosB), 0, Random.Range(_zPosA, _zPosB));
                
                for (int k = 0; k < _pastObjectPosition.Count; k++)
                {
                    if (((Mathf.Abs(_position.x - _pastObjectPosition[k].x) + Mathf.Abs(_position.z - _pastObjectPosition[k].z)) / 2) < _distanceForObject)
                    {
                        k = 0;
                       _position = new Vector3(Random.Range(_xPosA, _xPosB), 0, Random.Range(_zPosA, _zPosB));
                    }
                }

                Instantiate(_objs[i], _position, Quaternion.identity, _parent);
                _pastObjectPosition.Add(_position);
            }
        }
    }
}
