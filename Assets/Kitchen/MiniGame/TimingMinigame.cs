using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimingMinigame : MonoBehaviour, IMinigameMechanic, IPointerDownHandler, IPointerUpHandler
{
[Header("UI Visual")]
    public Slider jarumSlider;      // Slider untuk indikator api
    public Slider progressSlider;   // Slider untuk progress masakan 
    public RectTransform zonaTarget; // Gambar kotak hijau di dalam background jarum

    [Header("Gameplay")]
    public float kecepatanNaik = 1.2f;  // Kecepatan indikator saat klik ditahan
    public float kecepatanTurun = 1.5f; // Kecepatan indikator turun saat klik dilepas
    public float kecepatanMasak = 0.3f; // Seberapa cepat progress penuh
    public float ukuranZona = 0.2f;     // Lebar zona hijau

    private bool isMinigameActive = false;
    private bool isHolding = false;
    
    private float posisiJarum = 0f;
    private float posisiZonaTarget = 0.5f;
    private float tujuanZonaTarget = 0.5f;
    private float progresMasak = 0f;
    
    private float timerGerakZona = 0f;
    private float timeLimit = 15f; 
    private float currentTime = 0f;

    private Action<float> onFinishedCallback;

    private void Start()
    {
        if (jarumSlider != null) jarumSlider.gameObject.SetActive(false);
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);
    }

    public void StartMinigame(RecipeData recipe, Action<float> onMinigameFinished)
    {
        timeLimit = (recipe != null) ? recipe.timeLimit : 15f;
        
        posisiJarum = 0f;
        posisiZonaTarget = 0.5f;
        tujuanZonaTarget = 0.5f;
        progresMasak = 0f;
        currentTime = 0f;
        isHolding = false;
        
        onFinishedCallback = onMinigameFinished;
        isMinigameActive = true;

        if (jarumSlider != null)
        {
            jarumSlider.gameObject.SetActive(true);
            jarumSlider.maxValue = 1f;
            jarumSlider.value = 0f;
        }
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }

        Debug.Log("Jaga Suhu! Tahan klik untuk menaikkan api, lepas untuk menurunkan.");
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        currentTime += Time.deltaTime;
        if (currentTime >= timeLimit)
        {
            EndMinigame(0f); // Waktu habis
            return;
        }

        //Logika Pergerakan Jarum 
        if (isHolding)
            posisiJarum += kecepatanNaik * Time.deltaTime;
        else
            posisiJarum -= kecepatanTurun * Time.deltaTime;

        posisiJarum = Mathf.Clamp01(posisiJarum); // Kunci di angka 0 sampai 1
        if (jarumSlider != null) jarumSlider.value = posisiJarum;

        //Logika Pergerakan Zona Target 
        timerGerakZona -= Time.deltaTime;
        if (timerGerakZona <= 0f)
        {
            timerGerakZona = UnityEngine.Random.Range(0.5f, 2f); // Ganti arah tiap 0.5 - 2 detik
            tujuanZonaTarget = UnityEngine.Random.Range(0.1f, 0.9f); // Posisi random baru
        }
        // Smoothing zona target 
        posisiZonaTarget = Mathf.Lerp(posisiZonaTarget, tujuanZonaTarget, Time.deltaTime * 2f);

        // Update visual Zona Target di UI
        if (zonaTarget != null)
        {
            // Mengubah posisi anchor X 
            zonaTarget.anchorMin = new Vector2(posisiZonaTarget - (ukuranZona / 2f), 0);
            zonaTarget.anchorMax = new Vector2(posisiZonaTarget + (ukuranZona / 2f), 1);
            zonaTarget.offsetMin = Vector2.zero;
            zonaTarget.offsetMax = Vector2.zero;
        }

        //Logika Penambahan Progress Masak
        //Cek apakah posisi jarum dalam zona 
        if (Mathf.Abs(posisiJarum - posisiZonaTarget) <= (ukuranZona / 2f))
        {
            //Progress bertambah
            progresMasak += kecepatanMasak * Time.deltaTime;
        }
        else
        {
            //Progress berkurang
            progresMasak -= (kecepatanMasak / 2f) * Time.deltaTime;
        }
        
        progresMasak = Mathf.Clamp01(progresMasak);
        if (progressSlider != null) progressSlider.value = progresMasak;

        // Cek Menang
        if (progresMasak >= 1f)
        {
            EndMinigame(1f); // Sukses 100%
        }
    }

    public void OnPointerDown(PointerEventData eventData) { isHolding = true; }
    public void OnPointerUp(PointerEventData eventData) { isHolding = false; }

    private void EndMinigame(float score)
    {
        isMinigameActive = false;
        isHolding = false;
        
        if (jarumSlider != null) jarumSlider.gameObject.SetActive(false);
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);

        if (score >= 1f) Debug.Log("Sempurna! Masakan Matang!");
        else Debug.Log("Gagal! Waktu habis.");

        onFinishedCallback?.Invoke(score);
    }

    public void StopMinigame()
    {
        if (isMinigameActive)
        {
            EndMinigame(0f); 
        }
    }
}