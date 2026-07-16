using UnityEngine;
using UnityEngine.EventSystems;

public class NPCDropTarget : MonoBehaviour, IDropHandler
{
    private NPCController npcController;

    void Awake()
    {
        // Ambil komponen NPCController yang dibuat temanmu
        npcController = GetComponent<NPCController>();
        if (npcController == null)
        {
            Debug.LogError("NPCDropTarget harus dipasang di objek yang memiliki NPCController!");
        }
    }

    // Fungsi otomatis Unity saat mendeteksi ada objek di-drop ke NPC
    public void OnDrop(PointerEventData eventData)
    {
        if (npcController == null) return;

        if (npcController.currentState != NPCState.WaitingForFood) 
        {
            Debug.Log("NPC menolak: 'Tanya dulu pesanan saya dong!'");
            return; 
        }

        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        DraggableItem2D foodItem = droppedObj.GetComponent<DraggableItem2D>();
        
        if (foodItem != null && foodItem.dataBahan != null)
        {
            string idMakananDiberikan = foodItem.dataBahan.ingredientID; 
            string idResepDiminta = npcController.currentOrder.idResep; 

            if (idMakananDiberikan == idResepDiminta)
            {
                Debug.Log("Sesuai! NPC senang dan bersiap pulang.");

                if (OrderManager.Instance != null)
                {
                    // cari pesanan di daftar aktif yang ID-nya sama dengan pesanan NPC ini
                    for (int i = 0; i < OrderManager.Instance.daftarPesananAktif.Count; i++) 
                    {
                        if (OrderManager.Instance.daftarPesananAktif[i].idResep == idResepDiminta)
                        {
                            // Hapus dari daftar agar tiketnya tidak muncul lagi di dapur
                            OrderManager.Instance.daftarPesananAktif.RemoveAt(i); 
                            break; 
                        }
                    }
                }

                // Hancurkan makanan dan suruh NPC pulang
                Destroy(foodItem.gameObject);
                npcController.Pulang(); 
            }
            else
            {
                Debug.Log("Salah makanan! NPC menolak masakan ini.");
            }
        }
    }
}