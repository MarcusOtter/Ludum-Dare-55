using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler
{
    private PlayerInput _input;
    private Image _image;
    private bool _isMouseDown;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _input = FindFirstObjectByType<PlayerInput>();
    }

    public void Update()
    {
        var newMouseDown = _input.IsMouseDown;
        if (_isMouseDown && !newMouseDown)
        {
            _image.color = Color.white;
        } 
        
        _isMouseDown = newMouseDown;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isMouseDown) return;
        _image.color = Color.red;
    }
}
