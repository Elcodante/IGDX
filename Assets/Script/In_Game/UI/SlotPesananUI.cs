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

    // Fungsi baru: Slot ini tahu cara menampilkan datanya sendiri
    public void TampilkanData(OrderData data)
    {
        wadahSlot.SetActive(true);

        if (ikonMakanan != null) ikonMakanan.sprite = data.ikonMakanan;
        if (teksNamaMenu != null) teksNamaMenu.text = data.idResep.ToUpper();

        // Memanggil Helper yang sudah kita buat
        if (teksKeyword != null) teksKeyword.text = "KEYWORDS: " + OrderTextHelper.BuatTeksKeyword(data);
        if (teksDialog != null) teksDialog.text = OrderTextHelper.BuatTeksDialog(data);
    }

    public void Sembunyikan()
    {
        wadahSlot.SetActive(false);
    }
}