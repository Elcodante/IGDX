using UnityEngine;
using UnityEngine.UI;

public class CollectionBook : MonoBehaviour
{
    [Header("UI Tampilan")]
    public Image gambarBuku;

    [Header("Data Koleksi (Assets Gambar)")]
    public Sprite[] daftarAsetGambar;

    [Header("Tombol Navigasi")]
    public Button tombolKiri;
    public Button tombolKanan;

    private int indexSekarang = 0;

    private void Start()
    {
        
        if (tombolKiri != null) tombolKiri.onClick.AddListener(TampilSebelumnya);
        if (tombolKanan != null) tombolKanan.onClick.AddListener(TampilBerikutnya);

        
        UpdateTampilanKoleksi();
    }

    public void TampilBerikutnya()
    {
        if (daftarAsetGambar.Length == 0) return;

        indexSekarang++;
        
        
        if (indexSekarang >= daftarAsetGambar.Length)
        {
            indexSekarang = 0;
        }

        UpdateTampilanKoleksi();
    }

    public void TampilSebelumnya()
    {
        if (daftarAsetGambar.Length == 0) return;

        indexSekarang--;

        
        if (indexSekarang < 0)
        {
            indexSekarang = daftarAsetGambar.Length - 1;
        }

        UpdateTampilanKoleksi();
    }

    private void UpdateTampilanKoleksi()
    {
        if (daftarAsetGambar.Length == 0 || gambarBuku == null) return;

        for (int i = 0; i < daftarAsetGambar.Length; i++)
        {
            if (daftarAsetGambar[i] != null)
            {
                gambarBuku.sprite = daftarAsetGambar[indexSekarang];
            }
        }
    }
}
