using UnityEngine;
using TMPro;

public class CameraUIController : MonoBehaviour
{
    [Header("References")]
    public CameraController cameraController;

    [Header("Tombol Navigasi")]
    public GameObject tombolKeDapur;
    public GameObject tombolKeKasir;

    [Header("UI Panels")]
    public GameObject PanelKasir;
    public GameObject PanelDapur;

    private void OnEnable()
    {
        if (cameraController != null)
        {
            cameraController.OnAreaChangeStarted += SembunyikanSemuaUI;
            cameraController.OnAreaChangeCompleted += TampilkanUI;
        }
    }

    private void OnDisable()
    {
        if (cameraController != null)
        {
            cameraController.OnAreaChangeStarted -= SembunyikanSemuaUI;
            cameraController.OnAreaChangeCompleted -= TampilkanUI;
        }
    }

    private void SembunyikanSemuaUI(CameraController.CameraArea area)
    {
        PanelKasir.SetActive(false);
        PanelDapur.SetActive(false);
        tombolKeDapur.SetActive(false);
        tombolKeKasir.SetActive(false);
    }

    private void TampilkanUI(CameraController.CameraArea area)
    {
        bool isKasir = area == CameraController.CameraArea.Kasir;

        //PanelKasir.SetActive(isKasir);
        PanelDapur.SetActive(!isKasir);
        tombolKeDapur.SetActive(isKasir);
        tombolKeKasir.SetActive(!isKasir);
    }
}