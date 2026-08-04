using UnityEngine;
using System.Collections.Generic;

public class NPCOrderHandler : MonoBehaviour
{
    [Header("Daftar Pesanan")]
    public List<OrderData> daftarPesanan = new List<OrderData>();
    private bool pesananSudahDikirimKeDapur = false;

    // Reset status saat NPC baru spawn
    public void ResetHandler()
    {
        pesananSudahDikirimKeDapur = false;
        daftarPesanan.Clear();
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
                daftarPesanan.RemoveAt(i); // Coret dari daftar
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
}