using UnityEngine;
using UnityEngine.UI;

public class CollectionBook : MonoBehaviour
{
    [Header("UI & GameObjects")]
    public GameObject[] itemKoleksi;

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
        if (itemKoleksi.Length == 0) return;

        indexSekarang++;
        
        
        if (indexSekarang >= itemKoleksi.Length)
        {
            indexSekarang = 0;
        }

        UpdateTampilanKoleksi();
    }

    public void TampilSebelumnya()
    {
        if (itemKoleksi.Length == 0) return;

        indexSekarang--;

        
        if (indexSekarang < 0)
        {
            indexSekarang = itemKoleksi.Length - 1;
        }

        UpdateTampilanKoleksi();
    }

    private void UpdateTampilanKoleksi()
    {
        if (itemKoleksi.Length == 0) return;

        for (int i = 0; i < itemKoleksi.Length; i++)
        {
            if (itemKoleksi[i] != null)
            {
                itemKoleksi[i].SetActive(i == indexSekarang);
            }
        }
    }
}
