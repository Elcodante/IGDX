using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 1. CLASS BARU UNTUK MENYIMPAN KOMPONEN UI DI SETIAP KOTAK PESANAN
[System.Serializable]
public class SlotPesananUI
{
    public GameObject wadahSlot; // Objek utama pembungkus 1 pesanan
    public Image ikonMakanan;
    public TextMeshProUGUI teksNamaMenu;
    public TextMeshProUGUI teksDialog;
    public TextMeshProUGUI teksKeyword;
}

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPesanan;
    public GameObject tombolPerpindahan;
    public Image potretNPC;

    [Header("Slot Pesanan Maksimal (Isi dengan 3 Slot)")]
    public SlotPesananUI[] daftarSlotUI;

    public static bool IsPanelOpen { get; private set; }

    void Start()
    {
        IsPanelOpen = false;
        if (panelPesanan != null) panelPesanan.SetActive(false);
        if (tombolPerpindahan != null) tombolPerpindahan.SetActive(true);
        if (potretNPC != null)
        {
            potretNPC.sprite = null;
            potretNPC.enabled = false;
        }
    }

    public void TampilkanPanelPesanan(List<OrderData> dataPesanan, Sprite gambarNPC)
    {
        panelPesanan.SetActive(true);
        tombolPerpindahan.SetActive(false);
        potretNPC.enabled = true;
        IsPanelOpen = true;

        if (potretNPC != null && gambarNPC != null)
        {
            potretNPC.sprite = gambarNPC;
        }

        // 2. MATIKAN SEMUA SLOT TERLEBIH DAHULU AGAR BERSIH
        foreach (SlotPesananUI slot in daftarSlotUI)
        {
            slot.wadahSlot.SetActive(false);
        }

        // 3. NYALAKAN DAN ISI SLOT SESUAI JUMLAH PESANAN NPC
        for (int i = 0; i < dataPesanan.Count; i++)
        {
            // Cegah error jika pesanan melebihi jumlah slot UI yang kita siapkan
            if (i >= daftarSlotUI.Length) break;

            SlotPesananUI slotAktif = daftarSlotUI[i];
            OrderData data = dataPesanan[i];

            // Aktifkan visual kotak slot ini
            slotAktif.wadahSlot.SetActive(true);

            // Masukkan data gambar dan nama
            if (slotAktif.ikonMakanan != null) slotAktif.ikonMakanan.sprite = data.ikonMakanan;
            if (slotAktif.teksNamaMenu != null) slotAktif.teksNamaMenu.text = data.idResep.ToUpper();

            // Generate otomatis teks dialog & keywords
            if (slotAktif.teksKeyword != null) slotAktif.teksKeyword.text = "KEYWORDS: " + BuatTeksKeyword(data);
            if (slotAktif.teksDialog != null) slotAktif.teksDialog.text = BuatTeksDialog(data);
        }
    }

    public void TutupPanelPesanan()
    {
        panelPesanan.SetActive(false);
        tombolPerpindahan.SetActive(true);
        potretNPC.enabled = false;
        IsPanelOpen = false;
    }

    // --- FUNGSI PEMBANTU UNTUK MERANGKAI KATA-KATA --- //

    private string BuatTeksKeyword(OrderData data)
    {
        List<string> keyword = new List<string>();

        switch (data.targetManis)
        {
            case TingkatRasa.TidakPakai:
                break;
            case TingkatRasa.Sedikit:
                keyword.Add("Sedikit manis");
                break;
            case TingkatRasa.Sedang:
                keyword.Add("Manis sedang");
                break;
            case TingkatRasa.Banyak:
                keyword.Add("Sangat manis");
                break;
        }

        switch (data.targetGurih)
        {
            case TingkatRasa.TidakPakai:
                break;
            case TingkatRasa.Sedikit:
                keyword.Add("Sedikit gurih");
                break;
            case TingkatRasa.Sedang:
                keyword.Add("Gurih sedang");
                break;
            case TingkatRasa.Banyak:
                keyword.Add("Sangat gurih");
                break;
        }

        switch (data.targetLembut)
        {
            case TingkatRasa.TidakPakai:
                break;
            case TingkatRasa.Sedikit:
                keyword.Add("Sedikit lembut");
                break;
            case TingkatRasa.Sedang:
                keyword.Add("Lembut sedang");
                break;
            case TingkatRasa.Banyak:
                keyword.Add("Sangat lembut");
                break;
        }

        switch (data.isian)
        {
            case TingkatIsian.Sedikit:
                keyword.Add("Isian sedikit");
                break;
            case TingkatIsian.Sedang:
                keyword.Add("Isian sedang");
                break;
            case TingkatIsian.Banyak:
                keyword.Add("Isian banyak");
                break;
        }

        if (keyword.Count == 0) return "Original";

        return string.Join(", ", keyword); // Hasilnya: "Sedikit manis, Gurih sedang"
    }

    private string BuatTeksDialog(OrderData data)
    {
        // Anda bisa membuat percabangan dialog yang jauh lebih kompleks dan bervariasi di sini
        string dialog = $"\"Aku mau pesan {data.idResep}. ";

        if (data.targetManis == TingkatRasa.Banyak) dialog += "Aku suka banget yang manis, gula yang banyak ya. ";
        else if (data.targetManis == TingkatRasa.Sedikit) dialog += "Manisnya sedikit aja, jangan giung. ";

        if (data.targetGurih == TingkatRasa.Banyak || data.targetGurih == TingkatRasa.Sedang) dialog += "Terus agak gurih juga enak. ";

        if (data.targetLembut == TingkatRasa.Banyak) dialog += "Jangan terlalu padat, aku lebih suka yang lembut.\"";
        else dialog += "\"";

        return dialog;
    }
}