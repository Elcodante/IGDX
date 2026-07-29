using UnityEngine;
using TMPro;

public class CameraUIController : MonoBehaviour
{
    [Header("References")]
    public CameraController cameraController;
    public TextMeshProUGUI tombolText;

    [Header("UI Panels")]
    public GameObject PanelKasir;
    public GameObject PanelDapur;

    private void OnEnable()
    {
        if(cameraController != null)
        {
            cameraController.OnAreaChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if(cameraController != null)
        {
            cameraController.OnAreaChanged -= UpdateUI;
        }
    }

    private void UpdateUI(CameraController.CameraArea currentArea)
    {
        bool isKasir = currentArea == CameraController.CameraArea.Kasir;

        if(PanelKasir != null)
        {
            PanelKasir.SetActive(false);
        }
        if(PanelDapur != null)
        {
            PanelDapur.SetActive(!isKasir);
        }

        if (tombolText != null)
        {
            tombolText.text = isKasir ? "KE Dapur" : "KE Kasir";
        }
    }
}
