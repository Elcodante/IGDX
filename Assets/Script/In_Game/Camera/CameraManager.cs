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

    public event Action<CameraArea> OnAreaChangeStarted;
    public event Action<CameraArea> OnAreaChangeCompleted;

    // EVENT: Dipancarkan ketika area kamera berubah
    public event Action<CameraArea> OnAreaChanged;

    public CameraArea CurrentArea { get; private set; } = CameraArea.Kasir;

    private Transform targetTransform;
    private float cameraZOffset;

    private bool isMoving = false;
    void Start()
    {
        cameraZOffset = transform.position.z;
        SetArea(CameraArea.Kasir, true);
    }

    void Update()
    {
        if (targetTransform == null || !isMoving) return;

        Vector3 desiredPosition = new Vector3(targetTransform.position.x, targetTransform.position.y, cameraZOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, transitionSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, desiredPosition) < 0.05f)
        {
            transform.position = desiredPosition;
            isMoving = false;
            OnAreaChangeCompleted?.Invoke(CurrentArea);
        }
    }

    // Dipanggil oleh Tombol UI
    public void ToggleKamera()
    {
        if(isMoving) return; // Jangan pindah area jika kamera sedang bergerak

        CameraArea nextArea = (CurrentArea == CameraArea.Kasir) ? CameraArea.Dapur : CameraArea.Kasir;
        SetArea(nextArea, false);
    }

    public void SetArea(CameraArea area, bool instant = false)
    {
        CurrentArea = area;
        targetTransform = (area == CameraArea.Kasir) ? posisiKasir : posisiDapur;

        if (instant)
        {
            transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, cameraZOffset);
            OnAreaChanged?.Invoke(CurrentArea);
        }
        else
        {
            isMoving = true;
            OnAreaChangeStarted?.Invoke(CurrentArea);
        }
    }
}