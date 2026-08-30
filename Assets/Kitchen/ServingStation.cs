using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ServingStation : MonoBehaviour, IDropHandler
{
    [Header("Referensi Piring (Dapur)")]
    [Tooltip("Masukkan 3 objek piring kosong di dapur ke sini")]
    public Transform[] plateSlots = new Transform[3];

    [Header("UI Meja Kasir Dinamis")]
    [Tooltip("Masukkan Panel_Background_Kasir yang memiliki Content Size Fitter")]
    public GameObject panelBackgroundKasir;

    [Tooltip("Masukkan ketiga objek UI Box_1, Box_2, Box_3 di dalam panel kasir")]
    public Transform[] frontCounterBoxes = new Transform[3];

    [Header("UI Tombol")]
    public Button serveButton;

    private DraggableItem2D[] currentFoods = new DraggableItem2D[3];

    void Start()
    {
        if (serveButton != null)
        {
            serveButton.gameObject.SetActive(false);
            serveButton.onClick.AddListener(OnServeButtonClicked);
        }

        // Matikan semua box UI di awal agar panel background menyusut sampai hilang
        foreach (Transform box in frontCounterBoxes)
        {
            if (box != null) box.gameObject.SetActive(false);
        }

        // Sembunyikan panel utamanya di awal permainan
        if (panelBackgroundKasir != null) panelBackgroundKasir.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        DraggableItem2D dragItem = droppedObj.GetComponent<DraggableItem2D>();
        if (dragItem != null && dragItem.dataBahan != null)
        {
            // --- PELINDUNG 1: CEK DUPLIKASI ---
            // Pastikan barang yang ditarik ini belum ada di dalam daftar piring
            for (int i = 0; i < currentFoods.Length; i++)
            {
                if (currentFoods[i] == dragItem)
                {
                    // Barang sudah pernah masuk! Langsung batalkan agar tidak memenuhi piring lain.
                    return;
                }
            }
            // ----------------------------------

            int piringKosongIndex = -1;
            for (int i = 0; i < currentFoods.Length; i++)
            {
                if (currentFoods[i] == null)
                {
                    piringKosongIndex = i;
                    break;
                }
            }

            if (piringKosongIndex != -1)
            {
                currentFoods[piringKosongIndex] = dragItem;

                // --- PELINDUNG 2: BERI TAHU MAKANAN AGAR TIDAK KABUR ---
                dragItem.isDroppedSuccessfully = true;
                // --------------------------------------------------------

                // Gunakan SetParent dengan false agar skala objek 2D tidak rusak oleh Canvas
                dragItem.transform.SetParent(plateSlots[piringKosongIndex], false);
                dragItem.transform.position = plateSlots[piringKosongIndex].position;

                SpriteRenderer sr = dragItem.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "ItemDiAtasUI"; // Memasukkan sprite ke jalur VIP
                    sr.sortingOrder = 10;
                }

                if (serveButton != null) serveButton.gameObject.SetActive(true);
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

        // Nyalakan panel kasir jika ada makanan yang mau dikirim
        if (panelBackgroundKasir != null) panelBackgroundKasir.SetActive(true);

        for (int i = 0; i < currentFoods.Length; i++)
        {
            if (currentFoods[i] != null)
            {
                adaYangDiserve = true;

                if (i < frontCounterBoxes.Length && frontCounterBoxes[i] != null)
                {
                    // 1. Nyalakan Box UI ini. 
                    // Content Size Fitter akan otomatis melebarkan panel background!
                    frontCounterBoxes[i].gameObject.SetActive(true);

                    // 2. Pindahkan masakan 2D ke dalam Box UI tersebut
                    // Kembalikan ke 'false' agar kita bisa atur skalanya secara manual
                    currentFoods[i].transform.SetParent(frontCounterBoxes[i], false);

                    // Gunakan localPosition agar posisinya terpusat persis di tengah kotak UI
                    currentFoods[i].transform.localPosition = Vector3.zero;

                    // 3. KALKULASI UKURAN OTOMATIS (Sihir Matematika!)
                    RectTransform boxRect = frontCounterBoxes[i].GetComponent<RectTransform>();
                    SpriteRenderer sr = currentFoods[i].GetComponent<SpriteRenderer>();

                    if (boxRect != null && sr != null && sr.sprite != null)
                    {
                        // Ambil ukuran lebar kotak UI (Pixel)
                        float lebarKotak = boxRect.rect.width;

                        // Ambil ukuran asli gambar makanan 2D (Unit)
                        float lebarGambar = sr.sprite.bounds.size.x;

                        // Hitung skala yang dibutuhkan. 
                        // Dikali 0.7f agar gambar mengambil 70% dari kotak (menyisakan ruang/padding di pinggirnya)
                        // Ubah pengali dari 0.7f menjadi 1.08f untuk mencapai ukuran ~11.04
                        float skalaPas = (lebarKotak / lebarGambar) * 1.08f;

                        // Skala Z tidak terlalu berpengaruh pada gambar 2D, jadi bisa kita biarkan 1f
                        currentFoods[i].transform.localScale = new Vector3(skalaPas, skalaPas, 1f);
                    }
                }

                currentFoods[i] = null;
            }
        }

        if (adaYangDiserve && serveButton != null)
        {
            serveButton.gameObject.SetActive(false);
        }
    }
}