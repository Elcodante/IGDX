using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KitchenOrderUI : MonoBehaviour
{
    [Header("Pengaturan UI Tiket")]
    public Transform ticketContainer; // Tempat kertas tiket kumpul
    public GameObject ticketPrefab;   // Prefab cetakan tiket

    [System.Serializable]
    public struct MenuVisual
    {
        public string idResep; // Harus sama persis dengan idResep dari NPC
        public Sprite iconMasakan;
    }

    [Header("Database Gambar Tiket")]
    public List<MenuVisual> daftarMenu; 

    private void OnEnable()
    {
        // Berlangganan untuk pesanan baru
        OrderManager.OnPesananBaruMasukDapur += MunculkanTiket; 

        // Biar tiket tidak numpuk double saat player bolak-balik Kasir-Dapur
        HancurkanSemuaTiket();

        //Tarik semua pesanan yang masuk pas dapur sedang OFF
        if (OrderManager.Instance != null)
        {
            foreach (var pesanan in OrderManager.Instance.daftarPesananAktif)
            {
                MunculkanTiket(pesanan);
            }
        }
    }

    private void OnDisable()
    {
        // Berhenti berlangganan saat dapur dimatikan agar tidak terjadi memory leak
        OrderManager.OnPesananBaruMasukDapur -= MunculkanTiket; 
    }

    private void HancurkanSemuaTiket()
    {
        if (ticketContainer == null) return;
        foreach (Transform child in ticketContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void MunculkanTiket(OrderData pesananBaru)
    {
        if (ticketPrefab == null || ticketContainer == null) return;

        // Cetak tiket baru di UI dapur
        GameObject tiketBaru = Instantiate(ticketPrefab, ticketContainer);
        
        tiketBaru.transform.localScale = Vector3.one;
        tiketBaru.transform.localPosition = Vector3.zero;

        Image iconTiket = tiketBaru.GetComponentInChildren<Image>(); 
        TextMeshProUGUI teksPesanan = tiketBaru.GetComponentInChildren<TextMeshProUGUI>(); 

        // Cari ikon masakan yang cocok
        Sprite gambarKetemu = null;
        foreach (var menu in daftarMenu)
        {
            if (menu.idResep == pesananBaru.idResep) 
            {
                gambarKetemu = menu.iconMasakan;
                break;
            }
        }

        if (iconTiket != null && gambarKetemu != null)
        {
            iconTiket.sprite = gambarKetemu;
        }
        
        if (teksPesanan != null)
        {
            teksPesanan.text = pesananBaru.idResep; 
        }
    }
}