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
    private RectTransform canvasRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (tooltipPanel != null)
        {
            tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            parentCanvas = tooltipPanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                canvasRect = parentCanvas.GetComponent<RectTransform>();
            }
            
            tooltipPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (tooltipRect != null && tooltipPanel.activeSelf && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Batasi posisi mouse agar panel tooltip tidak terpotong tepi layar
            Vector2 clampedMousePos = ClampToScreen(mousePos);

            // Konversi Screen Position ke Local Position Canvas (Mendukung Overlay & Camera Mode)
            Camera uiCamera = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay) 
                ? parentCanvas.worldCamera 
                : null;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                clampedMousePos,
                uiCamera,
                out Vector2 localPoint))
            {
                tooltipRect.anchoredPosition = localPoint;
            }
        }
    }

    private Vector2 ClampToScreen(Vector2 screenPosition)
    {
        // Hitung posisi target awal ditambah offset
        Vector2 targetPos = screenPosition + offsetMouse;

        // Ambil ukuran dimensi RectTransform Tooltip
        Vector2 tooltipSize = tooltipRect.rect.size;
        Vector2 pivot = tooltipRect.pivot;

        // Hitung batas minimal dan maksimal di Layar
        float minX = tooltipSize.x * pivot.x;
        float maxX = Screen.width - (tooltipSize.x * (1f - pivot.x));

        float minY = tooltipSize.y * pivot.y;
        float maxY = Screen.height - (tooltipSize.y * (1f - pivot.y));

        // Clamp koordinat X dan Y agar tidak melebihi batas layar
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        return targetPos;
    }

    public void TampilkanTooltip(string infoText)
    {
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }

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