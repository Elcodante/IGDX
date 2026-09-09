using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KitchenOrderUI : MonoBehaviour
{
    [Header("UI Teks Pesanan 1 (Atas)")]
    public TextMeshProUGUI namaMakanan1Text;
    public TextMeshProUGUI atributText1;
    public TextMeshProUGUI jumlahText1;

    [Header("UI Teks Pesanan 2 (Bawah)")]
    public TextMeshProUGUI namaMakanan2Text;
    public TextMeshProUGUI atributText2;
    public TextMeshProUGUI jumlahText2;

    [Header("Tombol Navigasi")]
    public Button prevButton;
    public Button nextButton;

    private int currentPage = 0;
    private const int ITEMS_PER_PAGE = 2;

    private void OnEnable()
    {
        OrderManager.OnPesananBaruMasukDapur += RefreshOrderUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        OrderManager.OnPesananBaruMasukDapur -= RefreshOrderUI;
    }

    private void RefreshOrderUI(OrderData newOrder)
    {
        UpdateUI();
    }

    public void NextPage()
    {
        if (OrderManager.Instance == null) return;
        int totalPesanan = OrderManager.Instance.daftarPesananAktif.Count;
        
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
        if (OrderManager.Instance == null || OrderManager.Instance.daftarPesananAktif == null) 
        {
            KosongkanTampilan();
            return;
        }

        List<OrderData> listPesanan = OrderManager.Instance.daftarPesananAktif;
        int totalPesanan = listPesanan.Count;

        if (totalPesanan == 0)
        {
            KosongkanTampilan();
            if (prevButton != null) prevButton.interactable = false;
            if (nextButton != null) nextButton.interactable = false;
            return;
        }

        int maxPage = Mathf.Max(0, Mathf.CeilToInt((float)totalPesanan / ITEMS_PER_PAGE) - 1);
        if (currentPage > maxPage) currentPage = maxPage;

        int indexAwal = currentPage * ITEMS_PER_PAGE;

        if (indexAwal < totalPesanan)
        {
            SetItemData(listPesanan[indexAwal], namaMakanan1Text, atributText1, jumlahText1);
        }
        else
        {
            ClearSlot(namaMakanan1Text, atributText1, jumlahText1);
        }

        if (indexAwal + 1 < totalPesanan)
        {
            SetItemData(listPesanan[indexAwal + 1], namaMakanan2Text, atributText2, jumlahText2);
        }
        else
        {
            ClearSlot(namaMakanan2Text, atributText2, jumlahText2);
        }

        if (prevButton != null) prevButton.interactable = (currentPage > 0);
        if (nextButton != null) nextButton.interactable = ((currentPage + 1) * ITEMS_PER_PAGE < totalPesanan);
    }

    private void SetItemData(OrderData order, TextMeshProUGUI namaTxt, TextMeshProUGUI atributTxt, TextMeshProUGUI jumlahTxt)
    {
        if (namaTxt != null) 
        {
            namaTxt.gameObject.SetActive(true);
            namaTxt.text = order.idResep;
        }

        if (jumlahTxt != null) 
        {
            jumlahTxt.gameObject.SetActive(true);
            jumlahTxt.text = "1"; 
        }

        if (atributTxt != null)
        {
            atributTxt.gameObject.SetActive(true);

            List<string> listAtribut = new List<string>();

            if (listAtribut.Count > 0)
            {
                atributTxt.text = string.Join(", ", listAtribut);
            }
            else
            {
                atributTxt.text = "-";
            }
        }
    }

    private void ClearSlot(TextMeshProUGUI namaTxt, TextMeshProUGUI atributTxt, TextMeshProUGUI jumlahTxt)
    {
        if (namaTxt != null) { namaTxt.text = ""; namaTxt.gameObject.SetActive(false); }
        if (atributTxt != null) { atributTxt.text = ""; atributTxt.gameObject.SetActive(false); }
        if (jumlahTxt != null) { jumlahTxt.text = ""; jumlahTxt.gameObject.SetActive(false); }
    }

    private void KosongkanTampilan()
    {
        ClearSlot(namaMakanan1Text, atributText1, jumlahText1);
        ClearSlot(namaMakanan2Text, atributText2, jumlahText2);
    }
}