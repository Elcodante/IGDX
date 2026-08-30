using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPesanan;
    public GameObject tombolPerpindahan;
    public GameObject tombolSettings;
    public Image potretNPC;
    public GameObject panelMakanan;

    [Header("Slot Pesanan (Cukup Isi dengan 2 Slot)")]
    public SlotPesananUI[] daftarSlotUI;

    [Header("Navigasi Halaman")]
    public Button tombolHalamanBerikutnya;
    public Button tombolHalamanSebelumnya;

    public static bool IsPanelOpen { get; private set; }

    private List<OrderData> dataPesananAktif;
    private int halamanSekarang = 0;

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
        if (tombolSettings != null) tombolSettings.SetActive(true);

        if (tombolHalamanBerikutnya != null) tombolHalamanBerikutnya.onClick.AddListener(BukaHalamanBerikutnya);
        if (tombolHalamanSebelumnya != null) tombolHalamanSebelumnya.onClick.AddListener(BukaHalamanSebelumnya);
    }

    public void TampilkanPanelPesanan(List<OrderData> dataPesanan, Sprite gambarNPC)
    {
        panelPesanan.SetActive(true);
        tombolPerpindahan.SetActive(false);
        potretNPC.enabled = true;
        IsPanelOpen = true;
        tombolSettings.SetActive(false);
        panelMakanan.SetActive(false);

        if (potretNPC != null && gambarNPC != null)
        {
            potretNPC.sprite = gambarNPC;
        }

        dataPesananAktif = dataPesanan;
        halamanSekarang = 0;

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
                // Cukup suruh slotnya untuk menampilkan data
                daftarSlotUI[i].TampilkanData(dataPesananAktif[indexData]);
            }
            else
            {
                // Suruh slotnya sembunyi
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
        panelPesanan.SetActive(false);
        tombolPerpindahan.SetActive(true);
        potretNPC.enabled = false;
        IsPanelOpen = false;
        tombolSettings.SetActive(true);
        panelMakanan.SetActive(true);
    }
}