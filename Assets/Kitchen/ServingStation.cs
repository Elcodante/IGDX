using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ServingStation : MonoBehaviour, IDropHandler
{
    [Header("Referensi Piring (Dapur)")]
    [Tooltip("Masukkan 3 objek piring kosong di dapur ke sini")]
    public Transform[] plateSlots = new Transform[3]; 

    [Header("Meja Depan (Kasir)")]
    [Tooltip("Masukkan 3 slot posisi di meja kasir depan")]
    public Transform[] frontCounterSlots = new Transform[3];

    [Header("UI")]
    public Button serveButton; 

    // Array untuk menyimpan maksimal 3 masakan
    private DraggableItem2D[] currentFoods = new DraggableItem2D[3];

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

        // Kita cari tahu apakah yang di-drop adalah makanan hasil masak (2D)
        DraggableItem2D dragItem = droppedObj.GetComponent<DraggableItem2D>(); 
        if (dragItem != null && dragItem.dataBahan != null) 
        {
            // 1. Cari piring mana yang masih kosong
            int piringKosongIndex = -1;
            for (int i = 0; i < currentFoods.Length; i++)
            {
                if (currentFoods[i] == null)
                {
                    piringKosongIndex = i;
                    break; // Ketemu yang kosong, langsung stop pencarian
                }
            }

            // 2. Jika ada piring kosong, taruh makanannya!
            if (piringKosongIndex != -1)
            {

                // Simpan ke daftar
                currentFoods[piringKosongIndex] = dragItem;

                // Kunci posisi makanan di atas piring tersebut
                dragItem.transform.SetParent(plateSlots[piringKosongIndex]);
                dragItem.transform.position = plateSlots[piringKosongIndex].position;
                
                // Pastikan gambar makanan tampil di atas gambar piring
                SpriteRenderer sr = dragItem.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10; 

                if (serveButton != null) serveButton.gameObject.SetActive(true); 
                
                Debug.Log($"Makanan {dragItem.dataBahan.ingredientID} ditaruh di piring ke-{piringKosongIndex + 1}!");
            }
            else
            {
                Debug.Log("Gagal! Semua 3 piring sudah penuh!");
            }
        }
    }

    private void OnServeButtonClicked()
    {
        bool adaYangDiserve = false;

        // Looping untuk mengirim semua masakan yang ada di piring
        for (int i = 0; i < currentFoods.Length; i++)
        {
            if (currentFoods[i] != null)
            {
                adaYangDiserve = true;
                Debug.Log($"Mengirim {currentFoods[i].dataBahan.ingredientID} ke depan...[cite: 10]");

                // Pindahkan masakan ke meja kasir sesuai urutan piring
                if (i < frontCounterSlots.Length && frontCounterSlots[i] != null)
                {
                    currentFoods[i].transform.SetParent(frontCounterSlots[i]);
                    currentFoods[i].transform.position = frontCounterSlots[i].position;
                }

                // Kosongkan piring ini
                currentFoods[i] = null; 
            }
        }

        // Sembunyikan tombol jika sudah terkirim semua
        if (adaYangDiserve && serveButton != null) 
        {
            serveButton.gameObject.SetActive(false); //[cite: 10]
        }
    }
}