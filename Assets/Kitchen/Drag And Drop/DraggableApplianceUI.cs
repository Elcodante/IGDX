using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableApplianceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data Alat")]
    [Tooltip("Masukkan Prefab Panci/Wajan 2D ke sini")]
    public GameObject appliancePrefab2D; 
    
    [HideInInspector] public bool isDroppedSuccessfully = false;
    
    private Vector3 posisiAwal;
    private Transform parentAwal;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = transform.position;
        parentAwal = transform.parent;
        transform.SetParent(transform.root); // Pindah ke root Canvas biar nggak ketutupan panel lain
        transform.SetAsLastSibling();
        
        if (img != null) img.raycastTarget = false;
        isDroppedSuccessfully = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ganti Input.mousePosition menjadi eventData.position
        transform.position = eventData.position; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (img != null) img.raycastTarget = true;

        // Entah berhasil ditaruh di kompor atau gagal, UI gambar panci harus selalu balik ke rak
        // Biar pancinya nggak hilang dari daftar alat
        transform.SetParent(parentAwal);
        transform.position = posisiAwal;
    }
}