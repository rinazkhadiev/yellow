using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Image _joystickBG;
    [SerializeField] private Image _joystick;
    private Vector2 _inputVector;

    public virtual void OnPointerUp(PointerEventData ped)
    {
        _inputVector = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 _pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBG.rectTransform, ped.position, ped.pressEventCamera, out _pos))
        {
            _pos.x /= _joystickBG.rectTransform.sizeDelta.x;
            _pos.y /= _joystickBG.rectTransform.sizeDelta.y;

            _inputVector = new Vector2(_pos.x * 2, _pos.y * 2);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            _joystick.rectTransform.anchoredPosition = new Vector2(_inputVector.x * (_joystickBG.rectTransform.sizeDelta.x / 2),
                                                                   _inputVector.y * (_joystickBG.rectTransform.sizeDelta.y / 2));
        }
    }

    public float Horizontal()
    {
        if (_inputVector.x != 0)
        {
            return _inputVector.x;
        }
        else
        {
            return Input.GetAxis("Horizontal");
        }
    }

    public float Vertical()
    {
        if (_inputVector.y != 0)
        {
            return _inputVector.y;
        }
        else
        {
            return Input.GetAxis("Vertical");
        }
    }
}
