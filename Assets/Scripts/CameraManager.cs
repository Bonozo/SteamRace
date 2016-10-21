using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private List<CameraController> cameras = new List<CameraController>();

    private CameraController selectedCamera;
    private int selectedIndex;

    private void SelectNextCamera()
    {
        SetSelectedCameraIndex((selectedIndex + 1) % cameras.Count);
    }

    private void SetSelectedCameraIndex(int index)
    {
        if (index >= 0 && index < cameras.Count)
        {
            selectedIndex = index;
            selectedCamera = cameras[selectedIndex];
            cameras.ForEach(c => c.gameObject.SetActive(c == selectedCamera));
        }
    }

    private void Start()
    {
        SetSelectedCameraIndex(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SelectNextCamera();
        }
    }
}