using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPesanan;
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
        Debug.Log("Panel Pesanan Terbuka. NPC lain terkunci.");
    }

    public void TutupPanelPesanan()
    {
        panelPesanan.SetActive(false);
        IsPanelOpen = false; // BUKA KUNCI: NPC lain bisa diklik kembali
        Debug.Log("Panel Pesanan Ditutup. NPC lain terbuka.");
    }
}