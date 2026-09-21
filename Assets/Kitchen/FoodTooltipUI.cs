using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class FoodTooltipUI : MonoBehaviour
{
    public static FoodTooltipUI Instance;

    [Header("UI Referensi")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("Pengaturan")]
    public float delayTooltip = 1.2f;
    public Vector2 offsetMouse = new Vector2(15f, -15f);

    private Coroutine tooltipCoroutine;
    private RectTransform tooltipRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
            tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            parentCanvas = tooltipPanel.GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (tooltipRect != null && tooltipPanel.activeSelf && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                // GANTI: konversi screen point ke world point, karena Canvas-nya Screen Space - Camera
                Camera cam = parentCanvas.worldCamera; // kamera yang di-assign di Canvas
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    tooltipRect,
                    mousePos + offsetMouse,
                    cam,
                    out Vector3 worldPoint
                );
                tooltipRect.position = worldPoint;
            }
            else
            {
                // fallback lama, buat kalau ternyata Overlay
                tooltipRect.position = mousePos + offsetMouse;
            }
        }
    }

    public void TampilkanTooltip(string infoText)
    {
        // Batalkan coroutine sebelumnya
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }

        // Mulai delay
        tooltipCoroutine = StartCoroutine(TampilkanTooltipDelay(infoText));
    }

    private IEnumerator TampilkanTooltipDelay(string infoText)
    {
        yield return new WaitForSeconds(delayTooltip);

        if (tooltipText != null)
            tooltipText.text = infoText;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);

        tooltipCoroutine = null;
    }

    public void SembunyikanTooltip()
    {

        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}