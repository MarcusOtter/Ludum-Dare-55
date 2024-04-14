using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action OnMouseUp;
    
    internal bool IsMouseDown;
    
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp?.Invoke();
        }
        
        IsMouseDown = Input.GetMouseButton(0);
    }
}
