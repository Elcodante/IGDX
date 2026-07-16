using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ServingStation : MonoBehaviour, IDropHandler
{
    [Header("Referensi UI")]
    public Transform dropSlot;           // Tempat makanan di dapur
    public Button serveButton;           // Tombol "Serve"
    
    [Header("Meja Depan (Kasir)")]
    [Tooltip("Tarik slot kosong yang ada di dekat NPC ke sini")]
    public Transform frontCounterSlot;   // Tempat makanan muncul di depan kasir

    private DraggableItem2D currentFood;

    void Start()
    {
        if (serveButton != null)
        {
            serveButton.gameObject.SetActive(false);
            serveButton.onClick.AddListener(OnServeButtonClicked);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        DraggableItem2D dragItem = droppedObj.GetComponent<DraggableItem2D>();
        if (dragItem != null)
        {
            if (dropSlot == null)
            {
                Debug.LogError("ERROR: Kolom 'Drop Slot' di Serving Station belum diisi di Inspector!");
                return;
            }

            if (dragItem.dataBahan == null)
            {
                Debug.LogError("ERROR: Makanan yang di-drop tidak memiliki dataBahan!");
                return;
            }

            currentFood = dragItem;
            currentFood.transform.SetParent(dropSlot);
            currentFood.transform.position = dropSlot.position;

            if (serveButton != null) serveButton.gameObject.SetActive(true);
            
            Debug.Log($"Makanan {currentFood.dataBahan.ingredientID} siap di-serve!");
        }
    }

    private void OnServeButtonClicked()
    {
        if (currentFood != null && currentFood.dataBahan != null)
        {
            Debug.Log($"Mengirim {currentFood.dataBahan.ingredientID} ke depan...[cite: 5]");

            // PINDAHKAN MAKANAN KE MEJA KASIR DEPAN
            if (frontCounterSlot != null)
            {
                currentFood.transform.SetParent(frontCounterSlot);
                currentFood.transform.position = frontCounterSlot.position;
            }

            // Sembunyikan tombol serve karena makanan sudah dikirim
            if (serveButton != null) serveButton.gameObject.SetActive(false);
            
            // Lepas referensi agar meja saji kosong lagi
            currentFood = null; 
        }
    }
}