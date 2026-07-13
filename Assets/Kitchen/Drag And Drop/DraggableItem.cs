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

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
    }

    private void Start()
    {
        // Otomatis mengganti gambar UI sesuai data di ScriptableObject
        if (dataBahan != null && dataBahan.icon != null)
        {
            itemImage.sprite = dataBahan.icon;
        }
    }

    public void SetupData(IngredientData dataBaru)
    {
        dataBahan = dataBaru;
        // Ganti gambar iconUI dengan icon dari data bahan
        GetComponent<UnityEngine.UI.Image>().sprite = dataBahan.icon; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = rectTransform.position;
        parentAwal = transform.parent;
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f; 
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
    }
}