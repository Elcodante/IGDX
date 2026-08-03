using System;
using UnityEngine;
using UnityEngine.UI;

public class PassiveMinigame : MonoBehaviour, IMinigameMechanic
{
    [Header("UI Visual")]
    public Slider progressBar;

    private bool isCooking = false;
    private float currentTime = 0f;
    private float timeLimit = 5f;

    private Action<float> onFinishedCallback;

    private void Start()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    public void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished)
    {
        if (recipe != null)
        {
            // Ambil batas waktu dari data resep
            timeLimit = recipe.timeLimit;
        }
        else
        {
            Debug.LogWarning("Resep kosong! Menggunakan waktu default 5 detik.");
            timeLimit = 5f; 
        }
        
        currentTime = 0f;
        onFinishedCallback = onMinigameFinished;
        isCooking = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.maxValue = timeLimit; 
            progressBar.value = 0;
        }

        Debug.Log($"Mulai masak pasif! Tunggu {timeLimit} detik.");
    }

    private void Update()
    {
        if (!isCooking) return;

        // Tambah waktu
        currentTime += Time.deltaTime;

        // Update visual UI Slider pelan-pelan penuh
        if (progressBar != null) 
        {
            progressBar.value = currentTime;
        }

        // Kalau waktu sudah habis, hentikan minigame
        if (currentTime >= timeLimit)
        {
            StopMinigame(); 
        }
    }

    public void StopMinigame()
    {
        isCooking = false;
        if (progressBar != null) progressBar.gameObject.SetActive(false);

        // Karena ini pasif (tidak ada gagalnya), kasih skor sempurna (1.0f)
        onFinishedCallback?.Invoke(1.0f);
        Debug.Log("Masak pasif selesai! Mengeluarkan hasil masakan.");
    }
}