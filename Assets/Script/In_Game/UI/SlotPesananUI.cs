using System; // Wajib untuk sistem Event (Action)
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotPesananUI : MonoBehaviour
{
    [Header("Referensi UI Internal")]
    public GameObject wadahSlot;
    public Image ikonMakanan;

    [Header("Referensi Typewriter")]
    public UITypewriter ketikNamaMenu;
    public UITypewriter ketikDialog;

    [Header("Teks Langsung")]
    public TextMeshProUGUI teksKeyword;

    // FITUR DARI KODE PERTAMA: Event untuk mengirim data ke dapur
    public static event Action<string, string, string> OnPesananDicatat;

    public void Sembunyikan()
    {
        wadahSlot.SetActive(false);
    }

    public void MulaiAnimasiPesanan(OrderData data)
    {
        wadahSlot.SetActive(true);
        StartCoroutine(SekuensAnimasiSlot(data));
    }

    private IEnumerator SekuensAnimasiSlot(OrderData data)
    {
        // 1. SETUP AWAL
        if (teksKeyword != null) teksKeyword.gameObject.SetActive(false);
        if (ketikNamaMenu != null) ketikNamaMenu.BersihkanTeks();
        if (ketikDialog != null) ketikDialog.BersihkanTeks();

        if (ikonMakanan != null)
        {
            ikonMakanan.sprite = data.ikonMakanan;
            Color warnaIkon = ikonMakanan.color;
            warnaIkon.a = 0f;
            ikonMakanan.color = warnaIkon;

            // 2. FADE IN GAMBAR
            float waktuFade = 0f;
            float durasiFade = 0.5f;
            while (waktuFade < durasiFade)
            {
                waktuFade += Time.unscaledDeltaTime;
                warnaIkon.a = Mathf.Lerp(0f, 1f, waktuFade / durasiFade);
                ikonMakanan.color = warnaIkon;
                yield return null;
            }
        }

        // 3. KETIK NAMA MAKANAN
        string teksNama = data.idResep.ToUpper();
        if (ketikNamaMenu != null)
        {
            ketikNamaMenu.KetikTeks(teksNama);
            yield return new WaitForSecondsRealtime(teksNama.Length * ketikNamaMenu.kecepatanKetik + 0.2f);
        }

        // 4. KETIK DESKRIPSI DIALOG
        string teksDialog = OrderTextHelper.BuatTeksDialog(data);
        if (ketikDialog != null)
        {
            ketikDialog.KetikTeks(teksDialog);
            yield return new WaitForSecondsRealtime(teksDialog.Length * ketikDialog.kecepatanKetik + 0.2f);
        }

        // 5. MUNCULKAN KEYWORDS TERAKHIR
        string keywordUntukDapur = OrderTextHelper.BuatTeksKeyword(data);
        if (teksKeyword != null)
        {
            teksKeyword.text = keywordUntukDapur;
            teksKeyword.gameObject.SetActive(true);
        }

        // 6. FITUR DARI KODE PERTAMA: Catat di dapur SETELAH animasi selesai!
        CatatdiDapur(teksNama, keywordUntukDapur, data.orderId);
    }

    // Fungsi pemanggil event dari kode pertama
    private void CatatdiDapur(string namaMakanan, string keyword, string orderId)
    {
        OnPesananDicatat?.Invoke(namaMakanan, keyword, orderId);
    }
}