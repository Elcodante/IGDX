using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingAppliance : MonoBehaviour
{
    [HideInInspector] public CookingAppliance komporInduk;

    [Header("Gimik Tumpukan Bahan (2D)")]
    public bool gunakanTumpukanVisual = false; 
    public Transform tumpukanContainer;       
    public GameObject prefabVisualBahan2D;

    [Header("Pengaturan Alat / Kompor")]
    public string applianceName;
    public bool isStoveBase = false; // Centang HANYA jika objek ini adalah Kompor Utamanya
    public Transform applianceMountPoint; // Titik posisi alat ditaruh di atas kompor
    public GameObject startButtonUI; 

    [Header("Visual Indikator Kompor (Khusus Kompor)")]
    public SpriteRenderer stoveSpriteRenderer;
    public Sprite spriteKomporMati;
    public Sprite spriteKomporNyala;

    [Header("Minigame & Resep (Diisi di Alat / Panci)")]
    [SerializeField] private MonoBehaviour minigameScript; 
    public List<RecipeData> resepYangBisaDimasak; 

    [Header("Database & Output")]
    public GameObject draggableItemPrefab;
    public Transform spawnPoint;

    [Header("Visual Bahan & Indikator")]
    public SpriteRenderer applianceSprite2D; 
    public Sprite spriteKosong;              
    public Sprite spriteTerisi;              

    [System.Serializable]
    public struct VisualBahanMapping
    {
        public IngredientData bahan;
        public Sprite spriteSaatBahanMasuk;
    }
    public List<VisualBahanMapping> visualSpesifikBahan;

    [Header("Indikator UI Bahan")]
    public Transform indikatorContainer;     
    public GameObject indikatorPrefab;       

    // Data internal
    private CookingAppliance mountedAppliance; // Alat yang sedang menempel di atas kompor ini
    private IMinigameMechanic activeMinigame; 
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;

    private void Awake()
    {
        RefreshMinigameScript();
        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        // Inisialisasi kompor dalam keadaan mati
        SetStoveState(false);
        UpdateVisualAlat(); 
    }

    private void RefreshMinigameScript()
    {
        if (minigameScript != null) 
            activeMinigame = minigameScript as IMinigameMechanic;
        else 
            activeMinigame = GetComponent<IMinigameMechanic>();
    }

    // --- FUNGSI MOUNTING ALAT KE ATAS KOMPOR ---
    public bool MountAppliance(CookingAppliance newAppliance)
    {
        if (!isStoveBase) return false;

        // Jika sudah ada alat lain di atas kompor, lepas dulu atau tolak
        if (mountedAppliance != null)
        {
            Debug.Log("Kompor sudah terisi alat lain!");
            return false;
        }

        mountedAppliance = newAppliance;

        // Atur posisi alat tepat di titik mount point kompor
        Transform targetTransform = (applianceMountPoint != null) ? applianceMountPoint : transform;
        newAppliance.transform.SetParent(targetTransform);
        newAppliance.transform.localPosition = Vector3.zero;

        // Matikan fungsi drag pada alat agar kuncian posisinya aman saat di atas kompor
        DraggableItem2D dragScript = newAppliance.GetComponent<DraggableItem2D>();
        if (dragScript != null) dragScript.enabled = false;

        newAppliance.komporInduk = this;
        // Pastikan kompor tetap dalam kondisi mati saat alat dipasang
        SetStoveState(false);

        Debug.Log($"Alat {newAppliance.applianceName} berhasil dipasang di atas {applianceName}!");
        return true;
    }

    public CookingAppliance GetMountedAppliance()
    {
        return mountedAppliance;
    }

    // --- INDIKATOR SPRITE KOMPOR NYALA / MATI ---
    private void SetStoveState(bool isCooking)
    {
        if (!isStoveBase || stoveSpriteRenderer == null) return;

        if (isCooking && spriteKomporNyala != null)
        {
            stoveSpriteRenderer.sprite = spriteKomporNyala;
        }
        else if (!isCooking && spriteKomporMati != null)
        {
            stoveSpriteRenderer.sprite = spriteKomporMati;
        }
    }

    // --- LOGIKA BAHAN & RESEP ---
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

    private void CheckForValidRecipe()
    {
        currentValidRecipe = null;
        
        // Tentukan tombol Start mana yang mau dipakai (Punya sendiri, atau pinjam kompor induk)
        GameObject btnStartAktif = (komporInduk != null && komporInduk.startButtonUI != null) ? komporInduk.startButtonUI : startButtonUI;
        
        if (btnStartAktif != null) btnStartAktif.SetActive(false);

        List<RecipeData> activeRecipes = (mountedAppliance != null) ? mountedAppliance.resepYangBisaDimasak : resepYangBisaDimasak;
        if (activeRecipes == null || activeRecipes.Count == 0) return;

        foreach (var resep in activeRecipes)
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

        // Jika resep valid, nyalakan tombol UI-nya!
        if (currentValidRecipe != null && btnStartAktif != null)
        {
            btnStartAktif.SetActive(true); 
        }
    }

    // --- MULAI MEMASAK ---
    public void OnStartButtonClicked()
    {
        IMinigameMechanic targetMinigame = (mountedAppliance != null) ? mountedAppliance.activeMinigame : activeMinigame;

        if (targetMinigame == null && mountedAppliance != null)
        {
            mountedAppliance.RefreshMinigameScript();
            targetMinigame = mountedAppliance.activeMinigame;
        }

        // Ambil resep valid dari alat yang menempel (jika ada), atau dari diri sendiri
        RecipeData resepAktif = (mountedAppliance != null) ? mountedAppliance.currentValidRecipe : currentValidRecipe;

        if (targetMinigame != null && resepAktif != null)
        {
            if (startButtonUI != null) startButtonUI.SetActive(false);
            SetStoveState(true);
            targetMinigame.StartMinigame(resepAktif, OnMinigameFinished); 
        }
    }

    private void OnMinigameFinished(float finalScore)
    {
        // OTOMATIS MATIKAN SPRITE KOMPOR SETELAH MINIGAME SELESAI
        SetStoveState(false);

        IngredientData hasilAkhir = (finalScore >= 0.6f) ? currentValidRecipe.successResult : currentValidRecipe.failResult;
        
        if (hasilAkhir != null && draggableItemPrefab != null)
        {
            Transform titikSpawn = (spawnPoint != null) ? spawnPoint : transform;
            GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity);
            
            DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();
            if (dragScript2D != null) dragScript2D.SetupData(hasilAkhir); 
        }
        
        currentIngredients.Clear(); 
        if (mountedAppliance != null) mountedAppliance.currentIngredients.Clear();

        UpdateVisualAlat(); 
        if (mountedAppliance != null) mountedAppliance.UpdateVisualAlat();
    }

    private void UpdateVisualAlat()
    {
        // 1. Update Sprite Utama Alat (Panci/Wajan)
        if (applianceSprite2D != null)
        {
            Sprite targetSprite = spriteKosong; 
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
            if (targetSprite != null) applianceSprite2D.sprite = targetSprite;
        }

        // 2. Gimik Tumpukan Visual 2D (Khusus Mangkuk/Bowl jika ada)
        if (gunakanTumpukanVisual && tumpukanContainer != null && prefabVisualBahan2D != null)
        {
            foreach (Transform child in tumpukanContainer) Destroy(child.gameObject);

            for (int i = 0; i < currentIngredients.Count; i++)
            {
                GameObject visualBaru = Instantiate(prefabVisualBahan2D, tumpukanContainer);
                SpriteRenderer sr = visualBaru.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = currentIngredients[i].icon; 
                    sr.sortingOrder = i + 1;
                }
                visualBaru.transform.localPosition = new Vector3(0, i * 0.3f, 0); 
            }
        }

        // 3. UI INDIKATOR BAHAN (OTOMATIS PINJAM UI KOMPOR JIKA DI PREFAB KOSONG)
        Transform targetContainer = (indikatorContainer != null) ? indikatorContainer : (komporInduk != null ? komporInduk.indikatorContainer : null);
        GameObject targetPrefab = (indikatorPrefab != null) ? indikatorPrefab : (komporInduk != null ? komporInduk.indikatorPrefab : null);

        if (targetContainer == null || targetPrefab == null) return;

        if (currentIngredients.Count == 0)
        {
            targetContainer.gameObject.SetActive(false);
            return;
        }

        targetContainer.gameObject.SetActive(true);

        foreach (Transform child in targetContainer)
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
            GameObject iconBaru = Instantiate(targetPrefab, targetContainer);
            Image iconImage = iconBaru.GetComponentInChildren<Image>();
            TextMeshProUGUI qtyText = iconBaru.GetComponentInChildren<TextMeshProUGUI>();

            if (iconImage != null) iconImage.sprite = item.Key.icon;
            if (qtyText != null) qtyText.text = "x" + item.Value.ToString();
        }
    }
}