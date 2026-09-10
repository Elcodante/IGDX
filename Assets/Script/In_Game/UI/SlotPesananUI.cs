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
    public static event Action<string, string> OnPesananDicatat;

    public void TampilkanData(OrderData data)
    {
        wadahSlot.SetActive(true);

        if (ikonMakanan != null) ikonMakanan.sprite = data.ikonMakanan;
        if (teksNamaMenu != null) teksNamaMenu.text = data.idResep.ToUpper();

        if (teksKeyword != null) teksKeyword.text = OrderTextHelper.BuatTeksKeyword(data);
        if (teksDialog != null) teksDialog.text = OrderTextHelper.BuatTeksDialog(data);
        Debug.Log("TampilkanData Aktif");
        // Panggil method pencatat
        CatatdiDapur(teksNamaMenu.text, teksKeyword.text);
    }

    public void Sembunyikan()
    {
        wadahSlot.SetActive(false);
    }

    // 2. Cukup panggil event saat method ini berjalan
    private void CatatdiDapur(string namaMakanan, string keyword)
    {
        OnPesananDicatat?.Invoke(namaMakanan, keyword);
        Debug.Log("Catat Dapur Aktif");
    }
}