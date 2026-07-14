using UnityEngine;
using TMPro; 
public class CameraManager : MonoBehaviour
{
    // Mengatur status area posisi kamera
    public enum CameraArea { Kasir, Dapur }

    [Header("Target Positions")]
    public Transform posisiKasir;
    public Transform posisiDapur;

    [Header("Movement Settings")]
    public float transitionSpeed = 5f;

    [Header("UI Component Reference")]
    [Tooltip("Tarik objek Text (TMP) milik tombol ke sini agar teksnya bisa berubah otomatis.")]
    public TextMeshProUGUI tombolText;

    [Header("UI Panels Per Area")]
    public GameObject panelKasir;
    public GameObject panelDapur;

    private Transform targetTransform;
    private float cameraZOffset;
    private CameraArea currentArea = CameraArea.Kasir; // Status awal di Kasir

    void Start()
    {
        cameraZOffset = transform.position.z;
        if(panelKasir != null) panelKasir.SetActive(true);
        if(panelDapur != null) panelDapur.SetActive(false);

        // Set kamera awal di Kasir
        if (posisiKasir != null)
        {
            transform.position = new Vector3(posisiKasir.position.x, posisiKasir.position.y, cameraZOffset);
            targetTransform = posisiKasir;
            currentArea = CameraArea.Kasir;
            UpdateTombolText(); // Set tulisan tombol pertama kali
        }
    }

    void Update()
    {
        if (targetTransform == null) return;

        Vector3 desiredPosition = new Vector3(targetTransform.position.x, targetTransform.position.y, cameraZOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, transitionSpeed * Time.deltaTime);
    }

    // FUNGSI TUNGGAL: Dipanggil oleh satu tombol untuk bolak-balik posisi
    public void ToggleKamera()
    {
        if (currentArea == CameraArea.Kasir)
        {
            targetTransform = posisiDapur;
            if(panelKasir != null) panelKasir.SetActive(false);
            if(panelDapur != null) panelDapur.SetActive(true);
            currentArea = CameraArea.Dapur;
            Debug.Log("Kamera bergeser ke area Dapur.");
        }
        else
        {
            targetTransform = posisiKasir;
            currentArea = CameraArea.Kasir;
            if(panelKasir != null) panelKasir.SetActive(true);
            if(panelDapur != null) panelDapur.SetActive(false);
            Debug.Log("Kamera bergeser ke area Kasir.");
        }

        UpdateTombolText();
    }

    private void UpdateTombolText()
    {
        if (tombolText == null) return;

        // Logika petunjuk arah tombol
        if (currentArea == CameraArea.Kasir)
        {
            tombolText.text = "Ke Dapur";
        }
        else
        {
            tombolText.text = "Ke Kasir";
        }
    }
}