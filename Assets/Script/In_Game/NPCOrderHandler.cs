using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // PENTING: Tambahkan ini untuk akses UI Slider

public class NPCOrderHandler : MonoBehaviour
{
    [Header("Daftar Pesanan")]
    public List<OrderData> daftarPesanan = new List<OrderData>();

    [Header("Pengaturan Waktu (Sistem Keadilan)")]
    public float waktuDasarPerItem = 40f;
    public float bonusWaktuPemulihan = 15f;

    // --- KODE BARU: Referensi UI Bar Kesabaran ---
    [Header("Visual Kesabaran (Patience Bar)")]
    public GameObject canvasKesabaran;
    public Slider sliderKesabaran;
    public Image warnaFillSlider; // Untuk mengubah warna bar
    public Color warnaSabar = Color.green;
    public Color warnaMarah = Color.red;
    // ---------------------------------------------

    private float batasWaktuTungguTotal;
    private float waktuMenunggu = 0f;
    private bool sedangMenungguMakanan = false;

    public void ResetHandler()
    {
        sedangMenungguMakanan = false;
        waktuMenunggu = 0f;
        daftarPesanan.Clear();

        // Sembunyikan bar saat NPC baru spawn atau sedang jalan
        if (canvasKesabaran != null) canvasKesabaran.SetActive(false);
    }

    public void MulaiTungguPesanan()
    {
        sedangMenungguMakanan = true;

        // Munculkan bar tepat saat pesanan diambil
        if (canvasKesabaran != null) canvasKesabaran.SetActive(true);
    }

    void Update()
    {
        if (sedangMenungguMakanan)
        {
            waktuMenunggu += Time.deltaTime;

            // --- KODE BARU: Update Visual UI ---
            if (sliderKesabaran != null && batasWaktuTungguTotal > 0)
            {
                // Hitung sisa waktu (1 = penuh, 0 = habis)
                float sisaPersentase = 1f - Mathf.Clamp01(waktuMenunggu / batasWaktuTungguTotal);
                sliderKesabaran.value = sisaPersentase;

                // Juicing: Ubah warna secara dinamis dari hijau ke merah
                if (warnaFillSlider != null)
                {
                    warnaFillSlider.color = Color.Lerp(warnaMarah, warnaSabar, sisaPersentase);
                }
            }
        }
    }

    public void GenerateRandomOrder(MenuData[] menuList, int minVariasi, int maxVariasi)
    {
        if (menuList == null || menuList.Length == 0) return;

        int jumlahPesanan = Random.Range(minVariasi, maxVariasi + 1);
        batasWaktuTungguTotal = waktuDasarPerItem * jumlahPesanan;

        // Reset bar visual ke 100% di awal
        if (sliderKesabaran != null) sliderKesabaran.value = 1f;

        for (int i = 0; i < jumlahPesanan; i++)
        {
            OrderData pesananBaru = new OrderData();
            pesananBaru.orderId = System.Guid.NewGuid().ToString();
            MenuData menuDipilih = menuList[Random.Range(0, menuList.Length)];

            pesananBaru.idResep = menuDipilih.menuName;
            pesananBaru.ikonMakanan = menuDipilih.order.ikonMakanan;

            CustomizationData[] customizationMenu = menuDipilih.order.customizations;
            pesananBaru.customizations = new CustomizationData[customizationMenu.Length];

            for (int j = 0; j < customizationMenu.Length; j++)
            {
                pesananBaru.customizations[j] = customizationMenu[j];
                pesananBaru.customizations[j].target = (Tingkat)Random.Range(1, System.Enum.GetValues(typeof(Tingkat)).Length);
            }

            daftarPesanan.Add(pesananBaru);
        }
    }

    public bool CobaTerimaMakanan(DraggableItem2D makananPemain, out string orderIdTerhapus)
    {
        string idMakananDiberikan = makananPemain.dataBahan.ingredientID;
        orderIdTerhapus = null;

        for (int i = 0; i < daftarPesanan.Count; i++)
        {
            if (daftarPesanan[i].idResep == idMakananDiberikan)
            {
                orderIdTerhapus = daftarPesanan[i].orderId;

                HitungSkorMakanan(daftarPesanan[i], makananPemain);

                waktuMenunggu -= bonusWaktuPemulihan;
                if (waktuMenunggu < 0) waktuMenunggu = 0f;

                daftarPesanan.RemoveAt(i);

                if (ApakahSemuaPesananSelesai())
                {
                    sedangMenungguMakanan = false;
                    // Sembunyikan bar jika semua pesanan selesai
                    if (canvasKesabaran != null) canvasKesabaran.SetActive(false);
                    Debug.Log("Semua pesanan NPC telah selesai.");
                }

                return true;
            }
        }
        return false;
    }

    public bool ApakahSemuaPesananSelesai()
    {
        return daftarPesanan.Count == 0;
    }

    private void HitungSkorMakanan(OrderData pesananNPC, DraggableItem2D makananPemain)
    {
        float persentaseWaktuTerpakai = Mathf.Clamp01(waktuMenunggu / batasWaktuTungguTotal);
        float skorWaktu = Mathf.Lerp(100f, 10f, persentaseWaktuTerpakai);

        int skorAkhir = 0;
        float skorKustomisasi = 100f;

        if (pesananNPC.customizations != null && pesananNPC.customizations.Length > 0)
        {
            skorKustomisasi = 0f;
            float nilaiPerItemCustom = 100f / pesananNPC.customizations.Length;

            foreach (CustomizationData customNPC in pesananNPC.customizations)
            {
                Tingkat hasilPemain = makananPemain.DapatkanTingkatHasil(customNPC);

                if (hasilPemain == customNPC.target)
                {
                    skorKustomisasi += nilaiPerItemCustom;
                }
                else if (Mathf.Abs((int)hasilPemain - (int)customNPC.target) == 1)
                {
                    skorKustomisasi += (nilaiPerItemCustom * 0.5f);
                }
            }

            float skorFinalFloat = (skorWaktu * 0.4f) + (skorKustomisasi * 0.6f);
            skorAkhir = Mathf.RoundToInt(skorFinalFloat);
        }
        else
        {
            skorAkhir = Mathf.RoundToInt(skorWaktu);
        }

        if (ScoreManager.Instance != null) ScoreManager.Instance.TambahSkor(skorAkhir);

        NPCScoreDisplay scoreDisplay = GetComponent<NPCScoreDisplay>();
        if (scoreDisplay != null) scoreDisplay.MunculkanSkor(skorAkhir, pesananNPC.ikonMakanan);
    }
}