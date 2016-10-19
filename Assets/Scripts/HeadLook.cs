using UnityEngine;

public class HeadLook : MonoBehaviour
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
        Vector3 localEulerAngles = transform.localEulerAngles;

        // mouse
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            localEulerAngles.y = localEulerAngles.y + Input.GetAxisRaw("Mouse X");
            localEulerAngles.x = localEulerAngles.x - Input.GetAxisRaw("Mouse Y");
        }

        // controller
        localEulerAngles.y = localEulerAngles.y + Input.GetAxis("Look X");
        localEulerAngles.x = localEulerAngles.x + Input.GetAxis("Look Y");

        transform.localEulerAngles = localEulerAngles;
    }
}