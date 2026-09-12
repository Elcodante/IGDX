using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Kita bikin wadah data khusus untuk UI di sini, biar RecipeData asli aman!
[System.Serializable]
public struct BukuResepData
{
    [Header("Referensi Resep Asli (Opsional)")]
    public RecipeData resepDasar;
    
    [Header("Tampilan UI Teks & Gambar")]
    public string namaMasakan;
    public Sprite ikonMasakan;
    
    [TextArea(2, 5)] 
    public string teksBahan;
    
    [TextArea(2, 4)] 
    public string teksAlat;
    
    [TextArea(4, 8)] 
    public string teksCara;
}

public class RecipeBookUI : MonoBehaviour
{
    [Header("Data UI Content")]
    public CanvasGroup contentCanvasGroup; 
    public Image iconMasakan;
    public TextMeshProUGUI namaMasakan;
    public TextMeshProUGUI teksBahan;
    public TextMeshProUGUI teksAlat;
    public TextMeshProUGUI teksCara;

    [Header("Kertas Background (Wajib urut dari Depan ke Belakang)")]
    [Tooltip("Isi dengan: BG, BG (1), BG (2)")]
    public RectTransform[] papers; 
    
    [Header("Database Buku Resep UI")]
    // List ini sekarang menggunakan struct BukuResepData yang kita buat di atas
    public List<BukuResepData> listResepUI; 
    private List<BukuResepData> resepAktif = new List<BukuResepData>();


    // Variabel Internal
    private Vector2[] basePositions;
    private int currentIndex = 0;
    private bool isAnimating = false;

    void Start()
    {
        basePositions = new Vector2[papers.Length];
        for (int i = 0; i < papers.Length; i++)
        {
            basePositions[i] = papers[i].anchoredPosition;
        }
        FilterResepSesuaiLevel();
        UpdateUIContent();
    }

    private void FilterResepSesuaiLevel()
    {
        resepAktif.Clear();

        Debug.Log($"[RECIPE_BOOK] Total listResepUI (master): {listResepUI.Count}");

        if (LevelManager.Instance == null || LevelManager.Instance.currentMenuList == null)
        {
            Debug.LogWarning("[RECIPE_BOOK] LevelManager.Instance atau currentMenuList NULL, fallback tampilkan SEMUA resep.");
            resepAktif.AddRange(listResepUI);
            currentIndex = 0;
            return;
        }

        foreach (var data in listResepUI)
        {
            if (data.resepDasar == null)
            {
                Debug.LogWarning($"[RECIPE_BOOK] Entry '{data.namaMasakan}' punya resepDasar NULL, gak bisa dicocokkan!");
                continue;
            }
            if (data.resepDasar.successResult == null)
            {
                Debug.LogWarning($"[RECIPE_BOOK] resepDasar '{data.resepDasar.name}' punya successResult NULL!");
                continue;
            }

            bool ketemu = false;
            foreach (var menu in LevelManager.Instance.currentMenuList)
            {
                if (menu.finalProduct != null && data.resepDasar.successResult == menu.finalProduct)
                {
                    resepAktif.Add(data);
                    ketemu = true;
                    Debug.Log($"[RECIPE_BOOK] MATCH: '{data.namaMasakan}' cocok dengan menu '{menu.menuName}'");
                    break;
                }
            }

            if (!ketemu)
                Debug.Log($"[RECIPE_BOOK] '{data.namaMasakan}' TIDAK ada di menuList level ini, disembunyikan.");
        }

        Debug.Log($"[RECIPE_BOOK] Hasil akhir resepAktif: {resepAktif.Count} dari {listResepUI.Count}");
        currentIndex = 0;
    }

    public void NextRecipe()
    {
        if (isAnimating || resepAktif.Count <= 1) return; // resepAktif, bukan listResepUI
        
        currentIndex++;
        if (currentIndex >= resepAktif.Count) currentIndex = 0; 
        
        StartCoroutine(AnimateShuffle(1));
    }

    public void PrevRecipe()
    {
        if (isAnimating || resepAktif.Count <= 1) return; // resepAktif, bukan listResepUI
        
        currentIndex--;
        if (currentIndex < 0) currentIndex = resepAktif.Count - 1; 
        
        StartCoroutine(AnimateShuffle(-1));
    }

    private IEnumerator AnimateShuffle(int direction)
    {
        isAnimating = true;

        RectTransform frontPaper = papers[0]; 

        // --- FASE 1: Kertas ditarik ---
        float time = 0;
        float duration = 0.18f; 
        Vector2 startPos = frontPaper.anchoredPosition;
        Vector2 pullTargetPos = startPos + new Vector2(350f, -40f); 

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            frontPaper.anchoredPosition = Vector2.Lerp(startPos, pullTargetPos, t);
            if (contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t); 
            
            yield return null;
        }

        // --- FASE 2: Ganti Data & Pindah Posisi ---
        UpdateUIContent(); 
        frontPaper.SetSiblingIndex(0);

        RectTransform temp = papers[0];
        for (int i = 0; i < papers.Length - 1; i++)
        {
            papers[i] = papers[i + 1];
        }
        papers[papers.Length - 1] = temp;

        // --- FASE 3: Kertas kembali ---
        time = 0;
        duration = 0.22f; 
        
        Vector2[] startPositions = new Vector2[papers.Length];
        for (int i = 0; i < papers.Length; i++) startPositions[i] = papers[i].anchoredPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            if (contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t); 

            for (int i = 0; i < papers.Length; i++)
            {
                papers[i].anchoredPosition = Vector2.Lerp(startPositions[i], basePositions[i], t);
            }
            yield return null;
        }

        for (int i = 0; i < papers.Length; i++) papers[i].anchoredPosition = basePositions[i];
        if (contentCanvasGroup != null) contentCanvasGroup.alpha = 1f;

        isAnimating = false;
    }

    private void UpdateUIContent()
    {
        if (resepAktif == null || resepAktif.Count == 0)
        {
            if (namaMasakan != null) namaMasakan.text = "";
            if (iconMasakan != null) iconMasakan.enabled = false;
            if (teksBahan != null) teksBahan.text = "";
            if (teksAlat != null) teksAlat.text = "";
            if (teksCara != null) teksCara.text = "";
            return;
        }

        BukuResepData data = resepAktif[currentIndex]; // <-- resepAktif, bukan listResepUI

        if (namaMasakan != null) namaMasakan.text = data.namaMasakan;

        if (iconMasakan != null)
        {
            if (data.ikonMasakan != null)
            {
                iconMasakan.sprite = data.ikonMasakan;
                iconMasakan.enabled = true;
            }
            else
            {
                iconMasakan.enabled = false;
            }
        }

        if (teksBahan != null) teksBahan.text = data.teksBahan;
        if (teksAlat != null) teksAlat.text = data.teksAlat;
        if (teksCara != null) teksCara.text = data.teksCara;
    }
}