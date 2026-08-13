using UnityEngine;
using System;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Data skor level ini")]
    public int totalSkor = 0;

    public static event Action<int> OnScoreBerubah;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TambahSkor(int skorTambahan)
    {
        totalSkor += skorTambahan;

        OnScoreBerubah?.Invoke(totalSkor);

        Debug.Log($"Skor bertambah: {skorTambahan}. Total skor sekarang: {totalSkor}");
    }
}
