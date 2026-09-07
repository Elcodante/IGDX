using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelKasir;
    public GameObject tombolPerpindahan;
    public GameObject tombolSettings;
    public Image potretNPC;
    public GameObject panelMakanan;
    [SerializeField] private RectTransform panelPesanan;

    [Header("UI Ukuran Panel")]
    [SerializeField] private float panjang1Slot = 300f;
    [SerializeField] private float lebar1Slot = 300f;

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
        if (panelKasir != null) panelKasir.SetActive(false);
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
        if (panelKasir == null || daftarSlotUI == null || daftarSlotUI.Length == 0) return;

        if(dataPesanan.Count <= 1)
        {
            Debug.Log("Jumlah pesanan kurang dari atau sama dengan 1, menyesuaikan ukuran panel.");
            panelPesanan.sizeDelta = new Vector2(lebar1Slot, panjang1Slot);
        }

        panelKasir.SetActive(true);
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
        panelKasir.SetActive(false);
        tombolPerpindahan.SetActive(true);
        potretNPC.enabled = false;
        IsPanelOpen = false;
        tombolSettings.SetActive(true);
        panelMakanan.SetActive(true);
    }
}