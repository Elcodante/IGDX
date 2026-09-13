using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelKasir;
    public GameObject tombolPerpindahan;
    public GameObject tombolSettings;
    public GameObject panelMakanan;
    public GameObject text_Jumlah_NPC;

    [Header("Animasi Target")]
    public RectTransform potretNPC;
    public RectTransform panelPesanan;

    [Header("Posisi Animasi (Bisa disesuaikan)")]
    public Vector2 posisiPotretAsli;
    public Vector2 posisiPotretLuar = new Vector2(-500, 0);
    public Vector2 posisiPanelAsli;
    public Vector2 posisiPanelLuar = new Vector2(0, -800);

    [Header("UI Ukuran Panel")]
    public float panjang1Slot = 300f;
    public float lebar1Slot = 300f;

    [Header("Slot Pesanan (Masukkan objek Slot_1 & Slot_2)")]
    public SlotPesananUI[] daftarSlotUI;

    [Header("Navigasi Halaman")]
    public Button tombolHalamanBerikutnya;
    public Button tombolHalamanSebelumnya;

    public static bool IsPanelOpen { get; private set; }
    private List<OrderData> dataPesananAktif;
    private int halamanSekarang = 0;

    void Start()
    {
        // Simpan posisi default saat game dimuat
        if (potretNPC != null) posisiPotretAsli = potretNPC.anchoredPosition;
        if (panelPesanan != null) posisiPanelAsli = panelPesanan.anchoredPosition;
     
        IsPanelOpen = false;

        if (panelKasir != null) panelKasir.SetActive(false);
        if (tombolPerpindahan != null) tombolPerpindahan.SetActive(true);
        if (tombolSettings != null) tombolSettings.SetActive(true);

        if (potretNPC != null)
        {
            Image imgNPC = potretNPC.GetComponent<Image>();
            if (imgNPC != null)
            {
                imgNPC.sprite = null;
                imgNPC.enabled = false;
            }
        }

        if (tombolHalamanBerikutnya != null) tombolHalamanBerikutnya.onClick.AddListener(BukaHalamanBerikutnya);
        if (tombolHalamanSebelumnya != null) tombolHalamanSebelumnya.onClick.AddListener(BukaHalamanSebelumnya);
    }

    public void TampilkanPanelPesanan(List<OrderData> dataPesanan, Sprite gambarNPC)
    {
        if (panelKasir == null || daftarSlotUI == null || daftarSlotUI.Length == 0) return;

        // Mengecilkan panel jika pesanan hanya 1
        if (dataPesanan.Count <= 1)
        {
            panelPesanan.sizeDelta = new Vector2(lebar1Slot, panjang1Slot);
        }

        dataPesananAktif = dataPesanan;
        halamanSekarang = 0;

        Image imgNPC = potretNPC.GetComponent<Image>();
        if (imgNPC != null && gambarNPC != null)
        {
            imgNPC.sprite = gambarNPC;
            imgNPC.enabled = true;
        }

        // Mulai Orkestra Animasi!
        StartCoroutine(SekuensMunculUtama());
    }

    private IEnumerator SekuensMunculUtama()
    {
        // 1. Persiapan sebelum masuk ke layar
        panelKasir.SetActive(true);
        if (tombolPerpindahan != null) tombolPerpindahan.SetActive(false);
        if (tombolSettings != null) tombolSettings.SetActive(false);
        if (panelMakanan != null) panelMakanan.SetActive(false);
        if(text_Jumlah_NPC != null) text_Jumlah_NPC.SetActive(false);
        IsPanelOpen = true;

        // Lempar objek ke luar layar
        potretNPC.anchoredPosition = posisiPotretLuar;
        panelPesanan.anchoredPosition = posisiPanelLuar;

        // Matikan slot sementara
        foreach (var slot in daftarSlotUI)
        {
            if (slot != null) slot.Sembunyikan();
        }

        // 2. Animasi meluncur serentak (0.4 detik)
        float durasiGerak = 0.4f;
        float waktu = 0f;

        while (waktu < durasiGerak)
        {
            waktu += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, waktu / durasiGerak);

            potretNPC.anchoredPosition = Vector2.Lerp(posisiPotretLuar, posisiPotretAsli, t);
            panelPesanan.anchoredPosition = Vector2.Lerp(posisiPanelLuar, posisiPanelAsli, t);
            yield return null;
        }

        // Pastikan posisi akhirnya persis di tujuan
        potretNPC.anchoredPosition = posisiPotretAsli;
        panelPesanan.anchoredPosition = posisiPanelAsli;

        // 3. Setelah mendarat, minta tiap Slot menjalankan animasinya sendiri
        UpdateTampilanHalaman();
    }

    private void UpdateTampilanHalaman()
    {
        if (dataPesananAktif == null) return;

        int jumlahSlot = daftarSlotUI.Length;
        int indexAwal = halamanSekarang * jumlahSlot;

        for (int i = 0; i < jumlahSlot; i++)
        {
            int indexData = indexAwal + i;

            if (indexData < dataPesananAktif.Count)
            {
                // Sutradara memberi komando "Mulai!" ke SlotPesananUI
                daftarSlotUI[i].MulaiAnimasiPesanan(dataPesananAktif[indexData]);
            }
            else
            {
                daftarSlotUI[i].Sembunyikan();
            }
        }

        if (tombolHalamanSebelumnya != null)
            tombolHalamanSebelumnya.gameObject.SetActive(halamanSekarang > 0);

        if (tombolHalamanBerikutnya != null)
            tombolHalamanBerikutnya.gameObject.SetActive(indexAwal + jumlahSlot < dataPesananAktif.Count);
    }

    public void BukaHalamanBerikutnya()
    {
        halamanSekarang++;
        UpdateTampilanHalaman();
    }

    public void BukaHalamanSebelumnya()
    {
        halamanSekarang--;
        UpdateTampilanHalaman();
    }

    public void TutupPanelPesanan()
    {
        if (panelKasir != null) panelKasir.SetActive(false);
        if (tombolPerpindahan != null) tombolPerpindahan.SetActive(true);
        if (tombolSettings != null) tombolSettings.SetActive(true);
        if (panelMakanan != null) panelMakanan.SetActive(true);
        if(text_Jumlah_NPC != null) text_Jumlah_NPC.SetActive(true);

        Image imgNPC = potretNPC.GetComponent<Image>();
        if (imgNPC != null) imgNPC.enabled = false;

        IsPanelOpen = false;
    }
}