using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class CookingAppliance : MonoBehaviour
{
    [Header("Pengaturan Alat")]
    public string applianceName;
    public GameObject startButtonUI; 
    [SerializeField] private MonoBehaviour minigameScript; 
    
    [Header("Database & Output")]
    public List<RecipeData> resepYangBisaDimasak; 
    public GameObject draggableItemPrefab;
    public Transform spawnPoint;

    [Header("Visual Alat Masak")]
    public SpriteRenderer applianceSprite2D; // Isi ini jika alat masakmu objek 2D
    public Image applianceUIImage;           // Isi ini jika alat masakmu UI Canvas
    public Sprite spriteKosong;              // Gambar saat belum ada bahan
    public Sprite spriteTerisi;              // Gambar saat ada bahan masuk

    [System.Serializable]
    public struct VisualBahanMapping
    {
        public IngredientData bahan;
        public Sprite spriteSaatBahanMasuk;
    }

    [Header("Mapping Gambar Spesifik")]
    [Tooltip("Jika kosong, akan otomatis pakai Sprite Terisi")]
    public List<VisualBahanMapping> visualSpesifikBahan;

    [Header("Indikator UI Bahan")]
    public Transform indikatorContainer;     // Objek kosong/Layout Group tempat UI icon muncul
    public GameObject indikatorPrefab;       // Prefab yang isinya Image dan Text

    private IMinigameMechanic activeMinigame; 
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;

    private void Awake()
    {
        if (minigameScript != null) activeMinigame = minigameScript as IMinigameMechanic;
        if (activeMinigame == null) activeMinigame = GetComponent<IMinigameMechanic>();
        
        if (startButtonUI != null) startButtonUI.SetActive(false);
        UpdateVisualAlat(); // Set gambar awal ke kosong
    }

    public void AddIngredient(IngredientData ingredient)
    {
        currentIngredients.Add(ingredient);
        UpdateVisualAlat(); // Update gambar dan UI indikator
        CheckForValidRecipe();
    }

    // TOMBOL RESET 
    public void ResetIngredients()
    {
        currentIngredients.Clear();
        currentValidRecipe = null;
        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        UpdateVisualAlat(); // Kembalikan ke gambar kosong
        Debug.Log($"Bahan di {applianceName} berhasil dibuang!");
    }

    // UPDATE VISUAL & UI INDIKATOR 
    private void UpdateVisualAlat()
    {
        // Ubah Sprite
        Sprite targetSprite = spriteKosong; // Defaultnya kosong

        if (currentIngredients.Count > 0)
        {
            targetSprite = spriteTerisi; // Default jika ada isi tapi ga ada di list mapping

            IngredientData bahanTerakhir = currentIngredients[currentIngredients.Count - 1]; 

            // Cari apakah bahan terakhir ini ada di daftar gambar spesifik 
            foreach (var mapping in visualSpesifikBahan)
            {
                if (mapping.bahan == bahanTerakhir)
                {
                    targetSprite = mapping.spriteSaatBahanMasuk;
                    break;
                }
            }
        }

        // Terapkan gambarnya
        if (targetSprite != null)
        {
            if (applianceSprite2D != null) applianceSprite2D.sprite = targetSprite;
            if (applianceUIImage != null) applianceUIImage.sprite = targetSprite;
        }

        // Update UI Indikator Bahan di atas alat
        if (indikatorContainer == null || indikatorPrefab == null) return;

        // Bersihkan icon UI yang lama
        foreach (Transform child in indikatorContainer)
        {
            Destroy(child.gameObject);
        }

        // Hitung jumlah tiap bahan yang masuk
        Dictionary<IngredientData, int> hitungBahan = new Dictionary<IngredientData, int>();
        foreach (var bahan in currentIngredients)
        {
            if (hitungBahan.ContainsKey(bahan)) hitungBahan[bahan]++;
            else hitungBahan[bahan] = 1;
        }

        // Munculkan UI Icon baru sesuai jumlah bahan
        foreach (var item in hitungBahan)
        {
            GameObject iconBaru = Instantiate(indikatorPrefab, indikatorContainer);
            
            Image iconImage = iconBaru.GetComponentInChildren<Image>();
            TextMeshProUGUI qtyText = iconBaru.GetComponentInChildren<TextMeshProUGUI>();

            if (iconImage != null) iconImage.sprite = item.Key.icon;
            if (qtyText != null) qtyText.text = "x" + item.Value.ToString();
        }
    }

    private void CheckForValidRecipe()
    {
        currentValidRecipe = null;
        if (startButtonUI != null) startButtonUI.SetActive(false);

        foreach (var resep in resepYangBisaDimasak)
        {
            if (currentIngredients.Count == resep.inputIngredients.Count)
            {
                bool semuaBahanCocok = true;
                foreach (var bahanDibutuhkan in resep.inputIngredients)
                {
                    if (!currentIngredients.Contains(bahanDibutuhkan))
                    {
                        semuaBahanCocok = false;
                        break;
                    }
                }

                if (semuaBahanCocok)
                {
                    currentValidRecipe = resep;
                    break;
                }
            }
        }

        if (currentValidRecipe != null)
        {
            if (startButtonUI != null) startButtonUI.SetActive(true); 
            Debug.Log($"Resep {currentValidRecipe.name} siap dimasak!");
        }
    }

    public void OnStartButtonClicked()
    {
        if (startButtonUI != null) startButtonUI.SetActive(false);
        if (activeMinigame != null && currentValidRecipe != null)
        {
            activeMinigame.StartMinigame(currentValidRecipe, OnMinigameFinished); 
        }
    }

    private void OnMinigameFinished(float finalScore)
    {
        IngredientData hasilAkhir = (finalScore >= 0.6f) ? currentValidRecipe.successResult : currentValidRecipe.failResult;
        
        if (hasilAkhir != null && draggableItemPrefab != null)
        {
            Transform titikSpawn = (spawnPoint != null) ? spawnPoint : transform;
            GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity);
            
            DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();
            if (dragScript2D != null) dragScript2D.SetupData(hasilAkhir); 
        }
        
        currentIngredients.Clear(); 
        UpdateVisualAlat(); // Reset gambar alat masak ke kondisi kosong setelah masak
    }
}