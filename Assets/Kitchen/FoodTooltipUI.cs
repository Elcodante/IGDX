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

    private Coroutine tooltipCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            transform.position = mousePos + new Vector2(15f, -15f);
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