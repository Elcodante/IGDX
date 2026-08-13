using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject panelHasil;
    public TextMeshProUGUI teksJudul; 
    public TextMeshProUGUI teksSkorAkhir;

    public Image[] slotBintang;
    public Button tombolLanjut;
    public Button tombolBack;

    [Header("Pengaturan Bintang")]
    public Sprite bintangKosong;
    public Sprite bintangPenuh;

    [Header("Syarat Menang")]
    public int skorsatuBintang = 100;
    public int skorduaBintang = 200;
    public int skortigaBintang = 300;

    private void Start()
    {
        if (panelHasil != null)
        {
            panelHasil.SetActive(false);
        }
    }

    public void TampilkanHasilAkhir(int skorTotalPemain)
    {
        panelHasil.SetActive(true);
        teksSkorAkhir.text = skorTotalPemain.ToString();

        int jumlahBintang = 0;
        if(skorTotalPemain >= skortigaBintang)
        {
            jumlahBintang = 3;
        }
        else if(skorTotalPemain >= skorduaBintang)
        {
            jumlahBintang = 2;
        }
        else if(skorTotalPemain >= skorsatuBintang)
        {
            jumlahBintang = 1;
        }

        for(int i = 0; i < slotBintang.Length; i++)
        {
            if(i < jumlahBintang)
            {
                slotBintang[i].sprite = bintangPenuh;
            }
            else
            {
                slotBintang[i].sprite = bintangKosong;
            }
        }

        if(jumlahBintang > 0)
        {
            teksJudul.text = "Selamat!";
        }
        else
        {
            teksJudul.text = "Coba Lagi!";
        }

        Time.timeScale = 0f; // Pause the game
    }
}
