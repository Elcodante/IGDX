using UnityEngine;
using UnityEngine.UI;

public class CollectionBook : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0; 

    [Header("UI Tampilan")]
    [SerializeField] private Image gambarBuku;

    [Header("Default Image")]
    [SerializeField] private Sprite resepnotfound;

    [Header("Data Koleksi")]
    [SerializeField] private Sprite[] daftarAsetGambar;

    [Header("Batas Resep per Level (Max Level 5)")]
    [Tooltip("Jumlah resep yang terbuka untuk setiap level dari 1 sampai 5")]
    [SerializeField] private int[] resepTerbukaPerLevel = new int[5] { 2, 4, 6, 8, 10 }; 

    [Header("Tombol Navigasi")]
    [SerializeField] private Button tombolKiri;
    [SerializeField] private Button tombolKanan;

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
        
     
        int levelValid = Mathf.Clamp(currentLevel, 0, 5);

        int batasResep = 0;

      
        if (levelValid == 0)
        {
            batasResep = 0;
        }
        else
        {
           
            batasResep = resepTerbukaPerLevel[levelValid - 1];
        }

      
        if (indexSekarang < batasResep)
        {
            gambarBuku.sprite = daftarAsetGambar[indexSekarang];
        }
        else
        {
            gambarBuku.sprite = resepnotfound;
        }
    }
}