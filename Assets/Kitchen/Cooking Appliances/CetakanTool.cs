using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CetakanTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data & Sprite")]
    public IngredientData adonanData; // "Adonan Kembang Goyang" — dipakai saat AddIngredient ke Wajan
    public Sprite spriteKosong;
    public Sprite spriteAdaAdonan;

    [HideInInspector] public bool sudahDicelup = false;

    private Vector3 posisiAwal;
    private Transform parentAwal;
    private Image img;

    private void Awake() { img = GetComponent<Image>(); }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = transform.position;
        parentAwal = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        if (img != null) img.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = eventData.position;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (img != null) img.raycastTarget = true;
        transform.SetParent(parentAwal);
        transform.position = posisiAwal; // alat SELALU balik ke rak, gak pernah hilang
    }

    public void CelupkanKeAdonan()
    {
        sudahDicelup = true;
        if (img != null && spriteAdaAdonan != null) img.sprite = spriteAdaAdonan;
    }

    public void ResetSetelahDigoreng()
    {
        sudahDicelup = false;
        if (img != null && spriteKosong != null) img.sprite = spriteKosong;
    }
}