using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPesanan;

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
    }

    public void TampilkanPanelPesanan(OrderData dataPesanan)
    {
        panelPesanan.SetActive(true);
        IsPanelOpen = true; 

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
        IsPanelOpen = false; // BUKA KUNCI: NPC lain bisa diklik kembali
        Debug.Log("Panel Pesanan Ditutup. NPC lain terbuka.");
    }
}