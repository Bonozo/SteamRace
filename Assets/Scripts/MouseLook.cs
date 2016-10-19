using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.y = localEulerAngles.y + Input.GetAxisRaw("Mouse X");
            localEulerAngles.x = localEulerAngles.x - Input.GetAxisRaw("Mouse Y");
            transform.localEulerAngles = localEulerAngles;
        }
    }
}