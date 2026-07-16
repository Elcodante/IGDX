using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    public void TampilkanPanelPesanan(OrderData dataPesanan, Sprite gambarNPC)
    {
        panelPesanan.SetActive(true);
        tombolPerpindahan.SetActive(false); // Nonaktifkan tombol perpindahan saat panel pesanan terbuka
        potretNPC.enabled = true; // Aktifkan potret NPC saat panel pesanan terbuka
        IsPanelOpen = true; 

        if(potretNPC != null && gambarNPC != null)
        {
            potretNPC.sprite = gambarNPC;
        }
        teksNamaMakanan.text = dataPesanan.idResep;
        
        string detailOrder = "Detail Pesanan:\n";
        detailOrder += "- Tepung: " + dataPesanan.tepung.ToString() + "\n";
        detailOrder += "- Isian: " + dataPesanan.isian.ToString() + "\n";
        detailOrder += "- Rasa Khas: " + 
            (dataPesanan.targetManis > 50 ? "Manis " : "") + 
            (dataPesanan.targetLembut > 50 ? "Lembut " : "") +
            (dataPesanan.targetGurih > 50 ? "Gurih" : "");

        teksKostumisasi.text = detailOrder;

        Debug.Log("Panel Pesanan Terbuka. NPC lain terkunci.");
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