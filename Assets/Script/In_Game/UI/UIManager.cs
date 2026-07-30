using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPesanan;
    public GameObject tombolPerpindahan;
    public Image potretNPC;

    [Header("Order Ticket Text Compoents")]
    public TextMeshProUGUI teksNamaMakanan;
    public TextMeshProUGUI teksKostumisasi;
    public static bool IsPanelOpen { get; private set; }

    void Start()
    {
        IsPanelOpen = false; // Reset 
        if (panelPesanan != null)
        {
            panelPesanan.SetActive(false);
        }
        if (tombolPerpindahan != null)
        {
            tombolPerpindahan.SetActive(true);
        }
        if (potretNPC != null)
        {
            potretNPC.sprite = null; // Reset potret NPC
            potretNPC.enabled = false; // Nonaktifkan potret NPC saat panel pesanan ditutup
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

        // 1. KOSONGKAN TEKS SEBELUMNYA
        teksNamaMakanan.text = "";
        string detailOrder = "Detail Pesanan:\n\n";

        // 2. LAKUKAN LOOPING SEBANYAK JUMLAH PESANAN
        for (int i = 0; i < dataPesanan.Count; i++)
        {
            // Format Judul Makanan (Contoh: "Serabi & Putu Ayu")
            teksNamaMakanan.text += dataPesanan[i].idResep;
            if (i < dataPesanan.Count - 1)
            {
                teksNamaMakanan.text += " & ";
            }

            // Format Isi Teks (Menggunakan Rich Text Unity agar nama menu berwarna kuning)
            detailOrder += $"<color=yellow>--- Pesanan {i + 1}: {dataPesanan[i].idResep} ---</color>\n";
            detailOrder += "- Tepung: " + dataPesanan[i].tepung.ToString() + "\n";
            detailOrder += "- Isian: " + dataPesanan[i].isian.ToString() + "\n";
            detailOrder += "- Takaran Gula: " + dataPesanan[i].targetManis.ToString() + "\n";
            detailOrder += "- Takaran Santan: " + dataPesanan[i].targetLembut.ToString() + "\n";
            detailOrder += "- Takaran Kelapa: " + dataPesanan[i].targetGurih.ToString() + "\n\n";
        }

        teksKostumisasi.text = detailOrder;
        Debug.Log("Panel Pesanan Terbuka. Menampilkan " + dataPesanan.Count + " pesanan.");
    }

    public void TutupPanelPesanan()
    {
        panelPesanan.SetActive(false);
        tombolPerpindahan.SetActive(true); // Aktifkan kembali tombol perpindahan
        potretNPC.enabled = false; // Nonaktifkan potret NPC saat panel pesanan ditutup
        IsPanelOpen = false; // BUKA KUNCI: NPC lain bisa diklik kembali
        Debug.Log("Panel Pesanan Ditutup. NPC lain terbuka.");
    }
}