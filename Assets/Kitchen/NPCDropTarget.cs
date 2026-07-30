using UnityEngine;
using UnityEngine.EventSystems;

public class NPCDropTarget : MonoBehaviour, IDropHandler
{
    private NPCController npcController;

    void Awake()
    {
        npcController = GetComponent<NPCController>();
        if (npcController == null)
        {
            Debug.LogError("NPCDropTarget harus dipasang di objek yang memiliki NPCController!");
        }
    }

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

            // MENGGUNAKAN FUNGSI ENKAPSULASI: Biarkan NPC yang mengecek daftar pesanannya sendiri
            bool diterima = npcController.CobaTerimaMakanan(idMakananDiberikan);

            if (diterima)
            {
                Debug.Log("Sesuai! Makanan diterima oleh NPC.");

                // Hapus 1 tiket pesanan terkait dari papan dapur
                if (OrderManager.Instance != null)
                {
                    for (int i = 0; i < OrderManager.Instance.daftarPesananAktif.Count; i++)
                    {
                        if (OrderManager.Instance.daftarPesananAktif[i].idResep == idMakananDiberikan)
                        {
                            OrderManager.Instance.daftarPesananAktif.RemoveAt(i);
                            break;
                        }
                    }
                }

                // Hancurkan makanan dari tangan pemain
                Destroy(foodItem.gameObject);
            }
            else
            {
                Debug.Log("Salah makanan! NPC menolak masakan ini.");
                // Karena makanan tidak di-Destroy, otomatis fitur 'Snap Back' 
                // di DraggableItem2D akan menarik makanan ini kembali ke meja.
            }
        }
    }
}