using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static Vector3 headLookRotation = Vector3.zero;

    [SerializeField]
    private Transform followTransform = null;

    [SerializeField]
    private bool headLook = true;

    [SerializeField]
    private bool stabilise = true;

    private void LateUpdate()
    {
        // position the camera at the follow transform
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }

        LateUpdateHeadLook();
    }

    private void LateUpdateHeadLook()
    {
        if (headLook)
        {
            // mouse
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                headLookRotation.y = headLookRotation.y + Input.GetAxisRaw("Mouse X");
                headLookRotation.x = headLookRotation.x - Input.GetAxisRaw("Mouse Y");
            }

            // controller
            headLookRotation.y = headLookRotation.y + Input.GetAxis("Look X");
            headLookRotation.x = headLookRotation.x + Input.GetAxis("Look Y");

            // get the follow transform's rotation so we can delta rotate from it
            Vector3 followEulerAngles = followTransform.eulerAngles;

            // z-axis stabilisation
            if (stabilise)
            {
                followEulerAngles.z = 0.0f;
            }

            // apply the look rotation to the camera
            transform.eulerAngles = followEulerAngles + headLookRotation;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        // handle mouse cursor locking
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
}