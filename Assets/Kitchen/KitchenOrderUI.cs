using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KitchenOrderUI : MonoBehaviour
{
    public static KitchenOrderUI Instance { get; private set; }
    // Struct sederhana untuk menampung teks pesanan
    [System.Serializable]
    public struct SimpleOrder
    {
        public string namaMakanan;
        public string keyword;
        public string orderId;

        public SimpleOrder(string nama, string key, string id)
        {
            namaMakanan = nama;
            keyword = key;
            orderId = id;
        }
    }

    [Header("UI Teks Pesanan 1 (Atas)")]
    public TextMeshProUGUI namaMakanan1Text;
    public TextMeshProUGUI atributText1;

    [Header("UI Teks Pesanan 2 (Bawah)")]
    public TextMeshProUGUI namaMakanan2Text;
    public TextMeshProUGUI atributText2;

    [Header("Tombol Navigasi")]
    public Button prevButton;
    public Button nextButton;

    private int currentPage = 0;
    private const int ITEMS_PER_PAGE = 2;

    // List lokal untuk menyimpan pesanan teks
    private List<SimpleOrder> daftarPesananDapur = new List<SimpleOrder>();
    

    private void Awake()
    {
        Instance = this;
        SlotPesananUI.OnPesananDicatat += TerimaTeksDapur;
        Debug.Log("Event Aktif");
    }

    private void OnDestroy()
    {
         if (Instance == this) Instance = null;
        SlotPesananUI.OnPesananDicatat -= TerimaTeksDapur;
    }

    public void HapusPesananByOrderId(string orderId)
    {
        int index = daftarPesananDapur.FindIndex(o => o.orderId == orderId);

        if (index != -1)
        {
            Debug.Log($"[KITCHEN_ORDER] Menghapus tiket dengan orderId '{orderId}'.");
            HapusPesanan(index);
        }
        else
        {
            Debug.LogWarning($"[KITCHEN_ORDER] orderId '{orderId}' tidak ditemukan di papan!");
        }
    }

    // Tangkap data dan simpan ke List lokal
    private void TerimaTeksDapur(string namaMakanan, string keyword, string orderId) // BARU
    {
        daftarPesananDapur.Add(new SimpleOrder(namaMakanan, keyword, orderId));
        UpdateUI();
    }

    public void NextPage()
    {
        int totalPesanan = daftarPesananDapur.Count;
        
        if ((currentPage + 1) * ITEMS_PER_PAGE < totalPesanan)
        {
            currentPage++;
            UpdateUI();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        int totalPesanan = daftarPesananDapur.Count;

        if (totalPesanan == 0)
        {
            KosongkanTampilan();
            if (prevButton != null) prevButton.interactable = false;
            if (nextButton != null) nextButton.interactable = false;
            return;
        }

        // Hitung batasan halaman
        int maxPage = Mathf.Max(0, Mathf.CeilToInt((float)totalPesanan / ITEMS_PER_PAGE) - 1);
        if (currentPage > maxPage) currentPage = maxPage;

        int indexAwal = currentPage * ITEMS_PER_PAGE;

        // --- Render Slot 1 (Atas) ---
        if (indexAwal < totalPesanan)
        {
            SetSlot(namaMakanan1Text, atributText1, daftarPesananDapur[indexAwal]);
            Debug.Log("Set Slot 1 Aktif");
        }
        else
        {
            ClearSlot(namaMakanan1Text, atributText1);
        }

        // --- Render Slot 2 (Bawah) ---
        if (indexAwal + 1 < totalPesanan)
        {
            SetSlot(namaMakanan2Text, atributText2, daftarPesananDapur[indexAwal + 1]);
            Debug.Log("Set Slot 2 Aktif");
        }
        else
        {
            ClearSlot(namaMakanan2Text, atributText2);
        }

        // Update tombol navigasi
        if (prevButton != null) prevButton.interactable = (currentPage > 0);
        if (nextButton != null) nextButton.interactable = ((currentPage + 1) * ITEMS_PER_PAGE < totalPesanan);
    }

    private void SetSlot(TextMeshProUGUI namaTxt, TextMeshProUGUI atributTxt, SimpleOrder data)
    {
        if (namaTxt != null)
        {
            namaTxt.text = data.namaMakanan;
            namaTxt.gameObject.SetActive(true);
            Debug.Log("Di Set");
        }
        else
        {
            Debug.Log("namaTxt is null, cannot set text for namaMakanan.");
        }
        if (atributTxt != null)
        {
            atributTxt.text = data.keyword;
            atributTxt.gameObject.SetActive(true);
        }else
        {
            Debug.Log("atributTxt is null, cannot set text for keyword.");
        }
    }

    private void ClearSlot(TextMeshProUGUI namaTxt, TextMeshProUGUI atributTxt)
    {
        if (namaTxt != null) { namaTxt.text = ""; }
        if (atributTxt != null) { atributTxt.text = ""; }
    }

    public void PesananSelesai()
    {
        // KitchenOrderUI.HapusPesanan(indexPesanan);
    }

    private void KosongkanTampilan()
    {
        ClearSlot(namaMakanan1Text, atributText1);
        ClearSlot(namaMakanan2Text, atributText2);
    }

    public void HapusPesanan(int index)
    {
        if (index >= 0 && index < daftarPesananDapur.Count)
        {
            daftarPesananDapur.RemoveAt(index);

            if (currentPage > 0 &&
                currentPage * ITEMS_PER_PAGE >= daftarPesananDapur.Count)
            {
                currentPage--;
            }

            UpdateUI();
        }
    }

    
}