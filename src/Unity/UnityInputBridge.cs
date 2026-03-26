using GameCore;
using UnityEngine;

/// <summary>
/// Unity input bridge - connects GameCore input system to Unity's Input Manager.
/// </summary>
public class UnityInputBridge : IEngineInput
{
    private bool _mouseLocked = true;

    public bool IsKeyDown(string key)
    {
        return Input.GetKey(key) || Input.GetKeyDown(key);
    }

    public float GetAxis(string axis)
    {
        switch (axis.ToLower())
        {
            case "horizontal":
                return Input.GetAxis("Horizontal");
            case "vertical":
                return Input.GetAxis("Vertical");
            default:
                return 0f;
        }
    }

    public float GetMouseX()
    {
        return Input.mousePosition.x;
    }

    public float GetMouseY()
    {
        return Input.mousePosition.y;
    }

    public void LockMouse(bool locked)
    {
        _mouseLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    public bool IsMouseLocked => _mouseLocked;
}
