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

            bool diterima = npcController.CobaTerimaMakanan(idMakananDiberikan, out string orderIdSelesai); // BARU

            if (diterima)
            {
                Debug.Log("Sesuai! Makanan diterima oleh NPC.");

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

                // GANTI: hapus berdasarkan orderId spesifik, bukan nama
                if (KitchenOrderUI.Instance != null && !string.IsNullOrEmpty(orderIdSelesai))
                {
                    KitchenOrderUI.Instance.HapusPesananByOrderId(orderIdSelesai);
                }

                Destroy(foodItem.gameObject);
            }
            else
            {
                Debug.Log("Salah makanan! NPC menolak masakan ini.");
            }
        }
    }
}