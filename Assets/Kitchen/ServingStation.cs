using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ServingStation : MonoBehaviour, IDropHandler
{
    [Header("Referensi Piring (Dapur)")]
    public Transform[] plateSlots = new Transform[2];

    [Header("UI Meja Kasir Dinamis")]
    public GameObject panelBackgroundKasir;
    public Transform[] frontCounterBoxes = new Transform[2];

    [Header("UI Tombol")]
    public Button serveButton;

    private DraggableItem2D[] currentFoods = new DraggableItem2D[2];

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

    // --- CASE 1: GARNISH (UI item) ---
    DraggableItem garnishUI = droppedObj.GetComponent<DraggableItem>();
    if (garnishUI != null)
    {
        if (garnishUI.dataBahan == null) return;

        if (CobaTambahkanGarnish(garnishUI.dataBahan))
        {
            // Garnish habis dipakai, hapus dari UI rak/tangan
            Destroy(garnishUI.gameObject);
        }
        else
        {
            Debug.Log("Tidak ada piring yang butuh garnish ini.");
        }
        return; // garnish tidak pernah masuk ke logika piring makanan di bawah
    }

    // --- CASE 2: FOOD (2D world item, existing plating logic) ---
    DraggableItem2D dragItem = droppedObj.GetComponent<DraggableItem2D>();
    if(dragItem != null)
    {
        CustomizationResult custom = dragItem.customization;
    }


    if (dragItem == null || dragItem.dataBahan == null) return;

    if (!dragItem.dataBahan.isFinalProduct)
    {
        Debug.Log($"Ditolak! {dragItem.dataBahan.ingredientName} belum jadi makanan akhir, tidak bisa disajikan.");
        return;
    }
    // --- PELINDUNG 1: CEK DUPLIKASI ---
    for (int i = 0; i < currentFoods.Length; i++)
    {
        if (currentFoods[i] == dragItem) return;
    }

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

        dragItem.isDroppedSuccessfully = true;
        Collider2D foodCol = dragItem.GetComponent<Collider2D>();
        if (foodCol != null) foodCol.enabled = false;

        dragItem.transform.SetParent(plateSlots[piringKosongIndex], false);
        dragItem.transform.position = plateSlots[piringKosongIndex].position;

        SpriteRenderer sr = dragItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "UI_Canvas";
            sr.sortingOrder = 5;
        }

        if (serveButton != null) serveButton.gameObject.SetActive(true);
    }
    else
    {
        Debug.Log("Gagal! Semua 3 piring sudah penuh!");
    }
}

private bool CobaTambahkanGarnish(IngredientData garnishData)
{
    if (garnishData == null) return false;

    foreach (var food in currentFoods)
    {
        if (food == null || food.dataBahan == null) continue;

        var dataBahan = food.dataBahan;
        if (dataBahan.daftarGarnishDibutuhkan == null || dataBahan.daftarGarnishDibutuhkan.Count == 0) continue;
        if (food.garnishSudahMasuk.Contains(garnishData)) continue;
        if (!dataBahan.daftarGarnishDibutuhkan.Contains(garnishData)) continue;

        food.garnishSudahMasuk.Add(garnishData);

        int progres = food.garnishSudahMasuk.Count;
        if (dataBahan.spriteTiapTahapGarnish != null && progres <= dataBahan.spriteTiapTahapGarnish.Count)
        {
            SpriteRenderer sr = food.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = dataBahan.spriteTiapTahapGarnish[progres - 1];
        }
        return true;
    }
    return false;
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