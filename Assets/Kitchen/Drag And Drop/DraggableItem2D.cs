using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggableItem2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data Rasa (Hanya untuk Makanan Matang)")]
    public CustomizationResult customization;
    public IngredientData dataBahan;
    private Collider2D col;
    private Vector3 offset;

    private Vector3 posisiAwal;
    public bool isDroppedSuccessfully = false;

    [HideInInspector] public List<IngredientData> garnishSudahMasuk = new List<IngredientData>();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
      
    }

    public void SetupData(IngredientData dataBaru)
    {
        dataBahan = dataBaru;
        GetComponent<SpriteRenderer>().sprite = dataBahan.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = transform.position;
        isDroppedSuccessfully = false; // RESET status setiap kali mulai ditarik

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        offset = transform.position - mousePos;

        if (col != null) col.enabled = false;

        if (FoodTooltipUI.Instance != null) FoodTooltipUI.Instance.SembunyikanTooltip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        transform.position = mousePos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (col != null) col.enabled = true;

        // KUNCI UTAMA: Jika setelah dilepas statusnya masih false, kembalikan ke awal!
        if (!isDroppedSuccessfully)
        {
            transform.position = posisiAwal;
        }
    }

    // --- DETEKSI HOVER MOUSE ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FoodTooltipUI.Instance != null)
        {

            bool adaCustomization =
            customization.manis != Tingkat.TidakAda ||
            customization.lembut != Tingkat.TidakAda ||
            customization.gurih != Tingkat.TidakAda ||
            customization.isian != Tingkat.TidakAda;

            if (adaCustomization)
            {
                string teks = OrderTextHelper.BuatTeksCustomization(customization);
                FoodTooltipUI.Instance.TampilkanTooltip(teks);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (FoodTooltipUI.Instance != null)
        {
            FoodTooltipUI.Instance.SembunyikanTooltip();
        }
    }
}