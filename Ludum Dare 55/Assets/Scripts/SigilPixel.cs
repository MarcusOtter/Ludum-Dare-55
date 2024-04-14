using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SigilPixel : MonoBehaviour, IPointerEnterHandler
{
    internal bool IsActivated;

    [SerializeField] private Color activeColor = Color.red;
    
    private PlayerInput _input;
    private Image _image;
    private Color _startColor;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _input = FindFirstObjectByType<PlayerInput>();
        _startColor = _image.color;
    }

    public void Update()
    {
        _image.color = IsActivated ? activeColor : _startColor;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsActivated = false;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_input.IsMouseDown) return;
        IsActivated = true;
    }
}
