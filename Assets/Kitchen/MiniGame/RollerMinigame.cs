using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RollerMinigame : MonoBehaviour, IMinigameMechanic
{
    [Header("UI Visual")]
    public Slider progressBar;
    public RectTransform panahIndikator; 
    
    [Header("Pengaturan Posisi UI")]
    [Tooltip("Canvas tempat UI berada (Wajib diisi jika UI berada dalam Canvas Camera/World Space)")]
    public Canvas parentCanvas; 
    [Tooltip("Ubah offset ini untuk mengatur jarak UI dari objek")]
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
        
        if (parentCanvas == null && panahIndikator != null)
        {
            parentCanvas = panahIndikator.GetComponentInParent<Canvas>();
        }

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

        // --- Perbaikan Update Posisi UI agar tidak terpental ---
        UpdatePosisiUI();

        if (currentTime >= timeLimit)
        {
            StopMinigame(); 
            return;
        }

        // --- Logika New Input System ---
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                posisiAwalSentuh = Mouse.current.position.ReadValue();
                sedangMenggeser = true;
            }
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
        Camera mainCam = Camera.main;
        if (mainCam == null || panahIndikator == null) return;

        // 1. Dapatkan posisi objek di dunia 3D/2D beserta offset-nya
        Vector3 worldPos = transform.position + offsetPosisiUI;

        // 2. Konversi posisi dunia ke koordinat layar (Screen Point)
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        // Jika objek di belakang kamera, abaikan agar tidak terpental balik
        if (screenPos.z < 0) return;

        // 3. Konversi aman dari Screen Point ke AnchoredPosition Canvas
        RectTransform canvasRect = parentCanvas != null ? parentCanvas.GetComponent<RectTransform>() : panahIndikator.parent as RectTransform;

        Camera uiCamera = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? parentCanvas.worldCamera : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 localPoint))
        {
            // Terapkan ke Panah Indikator
            panahIndikator.anchoredPosition = localPoint;

            // Terapkan ke Progress Bar (diberi sedikit jarak ke atas)
            if (progressBar != null)
            {
                RectTransform progressRect = progressBar.GetComponent<RectTransform>();
                if (progressRect != null)
                {
                    progressRect.anchoredPosition = localPoint + new Vector2(0, 50f);
                }
            }
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