using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 
public class FoodTooltipUI : MonoBehaviour
{
    public static FoodTooltipUI Instance;

    [Header("UI Referensi")]
    public GameObject tooltipPanel; 
    public TextMeshProUGUI tooltipText; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        // Tambahkan "tooltipPanel != null" di bagian paling depan!
        if (tooltipPanel != null && tooltipPanel.activeSelf && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            transform.position = mousePos + new Vector2(15f, -15f); 
        }
    }

    public void TampilkanTooltip(string infoText)
    {
        if (tooltipText != null) tooltipText.text = infoText;
        if (tooltipPanel != null) tooltipPanel.SetActive(true);
    }

    public void SembunyikanTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }
}