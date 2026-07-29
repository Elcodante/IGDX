using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraArea { Kasir, Dapur }

    [Header("Target Positions")]
    public Transform posisiKasir;
    public Transform posisiDapur;

    [Header("Movement Settings")]
    public float transitionSpeed = 5f;

    // EVENT: Dipancarkan ketika area kamera berubah
    public event Action<CameraArea> OnAreaChanged;

    public CameraArea CurrentArea { get; private set; } = CameraArea.Kasir;

    private Transform targetTransform;
    private float cameraZOffset;

    void Start()
    {
        cameraZOffset = transform.position.z;
        SetArea(CameraArea.Kasir);
    }

    void Update()
    {
        if (targetTransform == null) return;

        Vector3 desiredPosition = new Vector3(targetTransform.position.x, targetTransform.position.y, cameraZOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, transitionSpeed * Time.deltaTime);
    }

    // Dipanggil oleh Tombol UI
    public void ToggleKamera()
    {
        CameraArea nextArea = (CurrentArea == CameraArea.Kasir) ? CameraArea.Dapur : CameraArea.Kasir;
        SetArea(nextArea);
    }

    public void SetArea(CameraArea area)
    {
        CurrentArea = area;
        targetTransform = (area == CameraArea.Kasir) ? posisiKasir : posisiDapur;

        // Beritahu sistem lain (seperti UI) bahwa kamera sudah berpindah
        OnAreaChanged?.Invoke(CurrentArea);
    }
}