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
    
    [Header("Gimik Interaksi")]
    public bool butuhDibuka = false;
    public bool isOpen = true; 
    public Sprite spriteTertutup; // Gambar saat panci/oven ditutup

    public bool butuhDinyalakan = false;
    public bool isOn = false; 
    public Sprite spriteNyala; // Gambar kompor saat nyala tapi belum ada wajan/bahan

    [Header("Database & Output")]
    public List<RecipeData> resepYangBisaDimasak; 
    public GameObject draggableItemPrefab;
    public Transform spawnPoint;

    [Header("Visual Alat Masak")]
    public SpriteRenderer applianceSprite2D; 
    public Image applianceUIImage;           
    public Sprite spriteKosong;              
    public Sprite spriteTerisi;              

    [Header("Gimik Tumpukan Bahan (2D)")]
    [Tooltip("Centang ini khusus untuk alat seperti mangkuk (Bowl)")]
    public bool gunakanTumpukanVisual = false; 
    public Transform tumpukanContainer;       // Titik kumpul spawn gambar bahan
    public GameObject prefabVisualBahan2D;    // Prefab kosong isi SpriteRenderer

    [System.Serializable]
    public struct VisualBahanMapping
    {
        public IngredientData bahan;
        public Sprite spriteSaatBahanMasuk;
    }

    [Header("Mapping Gambar Spesifik")]
    public List<VisualBahanMapping> visualSpesifikBahan;

    [Header("Indikator UI Bahan")]
    public Transform indikatorContainer;     
    public GameObject indikatorPrefab;       

    private IMinigameMechanic activeMinigame; 
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;

    private void Awake()
    {
        if (minigameScript != null) activeMinigame = minigameScript as IMinigameMechanic;
        if (activeMinigame == null) activeMinigame = GetComponent<IMinigameMechanic>();
        
        if (startButtonUI != null) startButtonUI.SetActive(false);
        UpdateVisualAlat(); 
    }

    public void ToggleBukaTutup()
    {
        if (!butuhDibuka) return;
        isOpen = !isOpen;
        UpdateVisualAlat();
        Debug.Log($"{applianceName} sekarang {(isOpen ? "Terbuka" : "Tertutup")}");
    }

    public void ToggleNyalaMati()
    {
        if (!butuhDinyalakan) return;
        isOn = !isOn;
        UpdateVisualAlat();
        Debug.Log($"{applianceName} sekarang {(isOn ? "Nyala" : "Mati")}");
    }

    public void AddIngredient(IngredientData ingredient)
    {
        currentIngredients.Add(ingredient);
        UpdateVisualAlat(); 
        CheckForValidRecipe();
    }

    public void ResetIngredients()
    {
        currentIngredients.Clear();
        currentValidRecipe = null;
        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        UpdateVisualAlat(); 
    }

    private void UpdateVisualAlat()
    {
        Sprite targetSprite = spriteKosong; 

        // 1. Cek apakah harus menampilkan visual tertutup
        if (butuhDibuka && !isOpen && spriteTertutup != null)
        {
            targetSprite = spriteTertutup;
        }
        else 
        {
            // 2. Jika ada bahan, utamakan visual bahan
            if (currentIngredients.Count > 0)
            {
                targetSprite = spriteTerisi; 
                IngredientData bahanTerakhir = currentIngredients[currentIngredients.Count - 1]; 

                foreach (var mapping in visualSpesifikBahan)
                {
                    if (mapping.bahan == bahanTerakhir)
                    {
                        targetSprite = mapping.spriteSaatBahanMasuk;
                        break;
                    }
                }
            }
            // 3. Jika kosong tapi kompor dinyalakan, tampilkan visual nyala
            else if (butuhDinyalakan && isOn && spriteNyala != null)
            {
                targetSprite = spriteNyala;
            }
        }

        if (targetSprite != null)
        {
            if (applianceSprite2D != null) applianceSprite2D.sprite = targetSprite;
            if (applianceUIImage != null) applianceUIImage.sprite = targetSprite;
        }

        if (indikatorContainer == null || indikatorPrefab == null) return;

        if (currentIngredients.Count == 0)
        {
            indikatorContainer.gameObject.SetActive(false);
        }
        else
        {
            indikatorContainer.gameObject.SetActive(true);
        }

        foreach (Transform child in indikatorContainer)
        {
            Destroy(child.gameObject);
        }

        Dictionary<IngredientData, int> hitungBahan = new Dictionary<IngredientData, int>();
        foreach (var bahan in currentIngredients)
        {
            if (hitungBahan.ContainsKey(bahan)) hitungBahan[bahan]++;
            else hitungBahan[bahan] = 1;
        }

        foreach (var item in hitungBahan)
        {
            GameObject iconBaru = Instantiate(indikatorPrefab, indikatorContainer);
            Image iconImage = iconBaru.GetComponentInChildren<Image>();
            TextMeshProUGUI qtyText = iconBaru.GetComponentInChildren<TextMeshProUGUI>();

            if (iconImage != null) iconImage.sprite = item.Key.icon;
            if (qtyText != null) qtyText.text = "x" + item.Value.ToString();
        }

        if (gunakanTumpukanVisual && tumpukanContainer != null && prefabVisualBahan2D != null)
        {
            // 1. Bersihkan visual tumpukan yang lama
            foreach (Transform child in tumpukanContainer)
            {
                Destroy(child.gameObject);
            }

            // 2. Munculkan gambar bahan satu per satu
            for (int i = 0; i < currentIngredients.Count; i++)
            {
                // Spawn prefab visual
                GameObject visualBaru = Instantiate(prefabVisualBahan2D, tumpukanContainer);
                
                SpriteRenderer sr = visualBaru.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Set gambarnya sesuai icon dari IngredientData
                    sr.sprite = currentIngredients[i].icon; 
                    
                    // Supaya gambar yang masuk belakangan posisinya ada di depan gambar sebelumnya
                    sr.sortingOrder = i + 1; 
                }

                // 3. Kasih jarak (offset) ke atas sedikit biar kelihatan menumpuk!
                // Angka 0.3f bisa kamu ubah-ubah kalau jarak antar bahannya kurang/ketinggian
                visualBaru.transform.localPosition = new Vector3(0, i * 0.3f, 0); 
            }
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
        }
    }

    public void OnStartButtonClicked()
    {
        if (butuhDinyalakan && !isOn)
        {
            Debug.Log($"Gagal! {applianceName} belum dinyalakan woi!");
            return;
        }

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
        
        // Opsional: Matikan otomatis setelah selesai masak
        // isOn = false; 

        UpdateVisualAlat(); 
    }
}