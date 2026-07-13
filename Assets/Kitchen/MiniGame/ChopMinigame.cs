using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChopMinigame : MonoBehaviour, IMinigameMechanic, IPointerDownHandler
{
    [Header("UI Visual")]
    public Slider progressBar;

    private bool isMinigameActive = false;
    private float currentTime = 0f;
    private float timeLimit = 5f;
    
    private int currentTaps = 0;
    private int targetTaps = 10;

    private Action<float> onFinishedCallback;

    private void Start()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    public void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished)
    {
        if (recipe != null)
        {
            timeLimit = recipe.timeLimit;
            targetTaps = Mathf.RoundToInt(10 * recipe.targetDifficulty);
        }
        else
        {
            Debug.LogWarning("Resep kosong! Menggunakan nilai default untuk testing.");
            timeLimit = 5f; // Waktu default 5 detik
            targetTaps = 10; // Target default 10 tap
        }
        
        currentTaps = 0;
        currentTime = 0f;
        onFinishedCallback = onMinigameFinished;
        isMinigameActive = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.maxValue = targetTaps;
            progressBar.value = 0;
        }

        Debug.Log($"Mulai Memotong! Target: {targetTaps} tap dalam {timeLimit} detik.");
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        currentTime += Time.deltaTime;

        if (currentTime >= timeLimit)
        {
            EndMinigameTimeOut();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isMinigameActive) return;

        currentTaps++;
        
        // Indikator bahwa klik terbaca
        Debug.Log("Talenan di-tap! Jumlah tap saat ini: " + currentTaps);
        
        if (progressBar != null) 
            progressBar.value = currentTaps;

        if (currentTaps >= targetTaps)
        {
            StopMinigame(); 
        }
    }

    private void EndMinigameTimeOut()
    {
        isMinigameActive = false;
        if (progressBar != null) progressBar.gameObject.SetActive(false);

        float score = (float)currentTaps / targetTaps;
        score = Mathf.Clamp01(score); 
        
        onFinishedCallback?.Invoke(score);
    }

    public void StopMinigame()
    {
        isMinigameActive = false;
        if (progressBar != null) progressBar.gameObject.SetActive(false);

        onFinishedCallback?.Invoke(1.0f);
    }
}