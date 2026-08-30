using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data Bahan")]
    public IngredientData dataBahan; 
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 posisiAwal;
    private Transform parentAwal;
    private Image itemImage;
    
    // --- VARIABEL BARU UNTUK MEMORI GAMBAR & UKURAN ---
    private Sprite iconAsli; 
    private Vector2 ukuranAwal; // Simpan ukuran kotak (RectTransform) asli

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
    }

    private void Start()
    {
        // 1. Simpan ukuran UI asli dari Inspector
        ukuranAwal = rectTransform.sizeDelta;

        if (dataBahan != null && dataBahan.icon != null)
        {
            itemImage.sprite = dataBahan.icon;
            iconAsli = dataBahan.icon; 
        }
    }

    public void SetupData(IngredientData dataBaru)
    {
        dataBahan = dataBaru;
        itemImage.sprite = dataBahan.icon; 
        iconAsli = dataBahan.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = rectTransform.position;
        parentAwal = transform.parent;
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f; 

        if (dataBahan != null && dataBahan.dragIcon != null)
        {
            itemImage.sprite = dataBahan.dragIcon;
            
            // Saat ditarik, biarkan ukurannya menyesuaikan proporsi dragIcon (telur 1 butir)
            itemImage.SetNativeSize(); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        transform.SetParent(parentAwal);
        rectTransform.position = posisiAwal;

        if (iconAsli != null)
        {
            itemImage.sprite = iconAsli;
            
            // 2. KEMBALIKAN KE UKURAN ASLI RAK, JANGAN PAKAI SetNativeSize() LAGI
            rectTransform.sizeDelta = ukuranAwal;
        }
    }
}