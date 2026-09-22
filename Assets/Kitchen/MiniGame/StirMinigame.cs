using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StirMinigame : MonoBehaviour, IMinigameMechanic, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI Visual")]
    public Slider progressBar;

    private bool isMinigameActive = false;
    private bool isHolding = false;
    
    private float currentTime = 0f;
    // private float timeLimit = 5f;
    
    private float currentStirProgress = 0f;
    private float targetStirProgress = 100f;
    
    [Header("Sensitivitas Mengaduk")]
    [Tooltip("Semakin besar angkanya, semakin cepat penuh saat diaduk")]
    public float stirSensitivity = 2f;

    private Vector2 lastMousePosition;
    private Action<float> onFinishedCallback;

    private void Start()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    public void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished)
    {
        AudioManager.instance.PlaySFXMengaduk();
        // Pengaman data resep
        if (recipe != null)
        {
            targetStirProgress = 100f * recipe.targetDifficulty;
        }
        else
        {
            targetStirProgress = 100f;
        }

        currentStirProgress = 0f;
        currentTime = 0f;
        isHolding = false;
        onFinishedCallback = onMinigameFinished;
        isMinigameActive = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.maxValue = targetStirProgress;
            progressBar.value = 0;
        }

    }

    private void Update()
    {
        if (!isMinigameActive) return;

        currentTime += Time.deltaTime;
    }

    // Deteksi saat pertama kali klik/sentuh mangkuk
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isMinigameActive) return;
        isHolding = true;
        lastMousePosition = eventData.position;
    }

    // Deteksi saat menggerakkan mouse/jari sambil menahan klik
    public void OnDrag(PointerEventData eventData)
    {
        if (!isMinigameActive || !isHolding) return;

        // Hitung jarak seberapa jauh mouse bergerak sejak frame terakhir
        float distanceMoved = Vector2.Distance(eventData.position, lastMousePosition);
        
        if (distanceMoved > 0.1f) // Beri sedikit threshold agar getaran kecil tidak dihitung
        {
            // Tambah progress adukan berdasarkan jarak gerakan dikali sensitivitas
            currentStirProgress += distanceMoved * stirSensitivity * Time.deltaTime;
            
            if (progressBar != null) 
                progressBar.value = currentStirProgress;

            // Cek apakah sudah selesai diaduk
            if (currentStirProgress >= targetStirProgress)
            {
                StopMinigame();
            }
        }

        lastMousePosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }

    public void StopMinigame()
    {
        isMinigameActive = false;
        isHolding = false;
        if (progressBar != null) progressBar.gameObject.SetActive(false);

        Debug.Log("Adonan selesai diaduk dengan sempurna!");
        onFinishedCallback?.Invoke(1.0f);
        AudioManager.instance.StopSFXWalk();
    }
}