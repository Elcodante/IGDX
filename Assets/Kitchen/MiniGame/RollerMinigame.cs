using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // <-- KODE BARU: Wajib dipanggil untuk New Input System

public class RollerMinigame : MonoBehaviour, IMinigameMechanic
{
    [Header("UI Visual")]
    public Slider progressBar;
    public RectTransform panahIndikator; 
    
    [Header("Pengaturan Posisi UI")]
    [Tooltip("Ubah angka Y ini di Inspector biar panah/bar pas di atas roller")]
    public Vector3 offsetPosisiUI = new Vector3(0, 1.5f, 0); 

    [Header("Pengaturan Geser")]
    public float jarakMinimalGeser = 50f; 

    private bool isMinigameActive = false;
    private float currentTime = 0f;
    private float timeLimit = 10f;
    
    private int currentScore = 0;
    private int targetScore = 6; 
    
    private bool tungguGeserAtas = true; 
    private Vector2 posisiAwalSentuh;
    private bool sedangMenggeser = false;

    private Action<float> onFinishedCallback;

    private void Start()
    {
        MatikanSemuaUI();
    }

    public void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished)
    {
        if (recipe != null)
        {
            timeLimit = recipe.timeLimit;
            targetScore = Mathf.RoundToInt(6 * recipe.targetDifficulty);
        }
        else
        {
            timeLimit = 10f; 
            targetScore = 6; 
        }
        
        currentScore = 0;
        currentTime = 0f;
        tungguGeserAtas = true; 
        onFinishedCallback = onMinigameFinished;
        isMinigameActive = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.maxValue = targetScore;
            progressBar.value = 0;
        }

        UpdateVisualPanah();
        Debug.Log($"Mulai Giling! Target: {targetScore} gesekan dalam {timeLimit} detik.");
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        currentTime += Time.deltaTime;

        // --- KODE BARU: Kunci posisi panah & bar di atas roller ---
        UpdatePosisiUI();

        if (currentTime >= timeLimit)
        {
            StopMinigame(); 
            return;
        }

        // --- KODE BARU: Menggunakan New Input System ---
        if (Mouse.current != null)
        {
            // Saat klik kiri ditekan
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                posisiAwalSentuh = Mouse.current.position.ReadValue();
                sedangMenggeser = true;
            }
            // Saat klik kiri dilepas
            else if (Mouse.current.leftButton.wasReleasedThisFrame && sedangMenggeser)
            {
                float jarakY = Mouse.current.position.ReadValue().y - posisiAwalSentuh.y;
                
                if (Mathf.Abs(jarakY) >= jarakMinimalGeser)
                {
                    if (jarakY > 0 && tungguGeserAtas)
                    {
                        tungguGeserAtas = false;
                        currentScore++;
                        UpdateVisualPanah();
                    }
                    else if (jarakY < 0 && !tungguGeserAtas)
                    {
                        tungguGeserAtas = true;
                        currentScore++;
                        UpdateVisualPanah();
                    }
                }

                sedangMenggeser = false;
                if (progressBar != null) progressBar.value = currentScore;

                if (currentScore >= targetScore)
                {
                    StopMinigame(); 
                }
            }
        }
    }

    private void UpdatePosisiUI()
    {
        // Ubah koordinat dunia 2D ke koordinat UI (Screen Space)
        if (Camera.main != null)
        {
            Vector3 posisiLayar = Camera.main.WorldToScreenPoint(transform.position + offsetPosisiUI);
            
            if (panahIndikator != null) panahIndikator.position = posisiLayar;
            
            // Opsional: Bar juga kita buat ngikut di atas panah
            if (progressBar != null) progressBar.transform.position = posisiLayar + new Vector3(0, 50f, 0); 
        }
    }

    private void UpdateVisualPanah()
    {
        if (panahIndikator != null)
        {
            panahIndikator.gameObject.SetActive(true);
            float rotasiZ = tungguGeserAtas ? 0f : 180f; 
            panahIndikator.localRotation = Quaternion.Euler(0f, 0f, rotasiZ);
        }
    }

    private void MatikanSemuaUI()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (panahIndikator != null) panahIndikator.gameObject.SetActive(false);
    }

    public void StopMinigame()
    {
        isMinigameActive = false;
        MatikanSemuaUI();

        float score = (float)currentScore / targetScore;
        score = Mathf.Clamp01(score); 
        
        onFinishedCallback?.Invoke(score);
    }
}