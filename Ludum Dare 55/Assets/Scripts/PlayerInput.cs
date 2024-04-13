using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool IsMouseDown;
    
    private void Update()
    {
        IsMouseDown = Input.GetMouseButton(0);
    }
}
