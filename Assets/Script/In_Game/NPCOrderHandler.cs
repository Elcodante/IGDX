using UnityEngine;
using System.Collections.Generic;

public class NPCOrderHandler : MonoBehaviour
{
    [Header("Daftar Pesanan")]
    public List<OrderData> daftarPesanan = new List<OrderData>();
    private bool pesananSudahDikirimKeDapur = false;

    [Header("Pengaturan waktu dan skor")]
    [Tooltip("Batas waktu (detik) sebelum skor pesanan jatuh ke nilai minimum")]
    public float batasWaktuTunggu = 60f; //Detik

    private float waktuMenunggu = 0f;
    private bool sedangMenungguMakanan = false;

    // Reset status saat NPC baru spawn
    public void ResetHandler()
    {
        pesananSudahDikirimKeDapur = false;
        sedangMenungguMakanan = false;
        waktuMenunggu = 0f;
        daftarPesanan.Clear();
    }

    void Update()
    {
        if (sedangMenungguMakanan)
        {
            waktuMenunggu += Time.deltaTime;
        }
    }

    // Tanggung jawab 1: Mengacak Pesanan
    public void GenerateRandomOrder(MenuData[] menuList, int minVariasi, int maxVariasi)
    {
        if (menuList == null || menuList.Length == 0) return;

        int jumlahPesanan = Random.Range(minVariasi, maxVariasi + 1);

        for (int i = 0; i < jumlahPesanan; i++)
        {
            OrderData pesananBaru = new OrderData();
            MenuData menuDipilih = menuList[Random.Range(0, menuList.Length)];

            pesananBaru.idResep = menuDipilih.menuName;
            pesananBaru.ikonMakanan = menuDipilih.menuImage;
            pesananBaru.tepung = menuDipilih.jenisTepung;
            pesananBaru.isian = (TingkatIsian)Random.Range(0, 3);
            pesananBaru.targetManis = (TingkatRasa)Random.Range(0, 4);
            pesananBaru.targetLembut = (TingkatRasa)Random.Range(0, 4);
            pesananBaru.targetGurih = (TingkatRasa)Random.Range(0, 4);

            daftarPesanan.Add(pesananBaru);
        }
    }

    // Tanggung jawab 2: Mengirim ke Dapur
    public void KirimKeDapur()
    {
        if (!pesananSudahDikirimKeDapur && OrderManager.Instance != null)
        {
            foreach (OrderData pesanan in daftarPesanan)
            {
                OrderManager.Instance.KirimPesananKeDapur(pesanan);
            }
            pesananSudahDikirimKeDapur = true;

            sedangMenungguMakanan = true;
            waktuMenunggu = 0f;

            Debug.Log("Pesanan NPC dikirim ke dapur.");
        }
    }

    // Tanggung jawab 3: Mengecek Makanan dari Pemain
    public bool CobaTerimaMakanan(string idMakananDiberikan)
    {
        for (int i = 0; i < daftarPesanan.Count; i++)
        {
            if (daftarPesanan[i].idResep == idMakananDiberikan)
            {
                HitungSkorMakanan(daftarPesanan[i].idResep);

                daftarPesanan.RemoveAt(i); // Coret dari daftar

                if(ApakahSemuaPesananSelesai())
                {
                    sedangMenungguMakanan = false;
                    Debug.Log("Semua pesanan NPC telah selesai.");
                }

                return true;
            }
        }
        return false;
    }

    // Tanggung jawab 4: Mengecek apakah sudah kenyang
    public bool ApakahSemuaPesananSelesai()
    {
        return daftarPesanan.Count == 0;
    }

    private void HitungSkorMakanan(string namaMakanan)
    {
        float persentaseWaktuTerpakai = Mathf.Clamp01(waktuMenunggu / batasWaktuTunggu);

        int skorDidapat = Mathf.RoundToInt(Mathf.Lerp(100f, 10f, persentaseWaktuTerpakai));

        Debug.Log($"Skor untuk makanan {namaMakanan}: {skorDidapat} (Waktu menunggu: {waktuMenunggu:F2}s)");

        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.TambahSkor(skorDidapat);
        }
    }
}