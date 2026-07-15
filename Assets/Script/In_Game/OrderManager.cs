using UnityEngine;
using System.Collections.Generic;
using System;
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Daftar Tiket Pesanan")]
    public List<OrderData> daftarPesananAktif = new List<OrderData>();

    public static event Action<OrderData> OnPesananBaruMasukDapur;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void KirimPesananKeDapur(OrderData pesananBaru)
    {
        daftarPesananAktif.Add(pesananBaru);

        OnPesananBaruMasukDapur?.Invoke(pesananBaru);
        Debug.Log($"Pesanan baru diterima: {pesananBaru.idResep}");
    }
}
