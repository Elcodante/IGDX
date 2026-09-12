using System; // Tambahkan ini di paling atas
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SlotPesananUI
{
    
    public GameObject wadahSlot;
    public Image ikonMakanan;
    public TextMeshProUGUI teksNamaMenu;
    public TextMeshProUGUI teksDialog;
    public TextMeshProUGUI teksKeyword;

    // 1. Tambahkan Event ini untuk ngirim teks ke mana saja
    public static event Action<string, string, string> OnPesananDicatat;

    public void TampilkanData(OrderData data)
    {
        wadahSlot.SetActive(true);

        if (ikonMakanan != null) ikonMakanan.sprite = data.ikonMakanan;
        if (teksNamaMenu != null) teksNamaMenu.text = data.idResep.ToUpper();

        if (teksKeyword != null) teksKeyword.text = OrderTextHelper.BuatTeksKeyword(data);
        if (teksDialog != null) teksDialog.text = OrderTextHelper.BuatTeksDialog(data);

        CatatdiDapur(teksNamaMenu.text, teksKeyword.text, data.orderId); // BARU
    }

    

    public void Sembunyikan()
    {
        wadahSlot.SetActive(false);
    }

    // 2. Cukup panggil event saat method ini berjalan
    private void CatatdiDapur(string namaMakanan, string keyword, string orderId) // BARU
    {
        OnPesananDicatat?.Invoke(namaMakanan, keyword, orderId);
    }
}