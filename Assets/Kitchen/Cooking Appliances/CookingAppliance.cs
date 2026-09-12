using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingAppliance : MonoBehaviour
{
    [HideInInspector] public CookingAppliance komporInduk;
    
    [SerializeField]private int maxIngredient = 10;
    private int totalIngredient;

    [Header("Gimik Tumpukan Bahan (2D)")]
    public bool gunakanTumpukanVisual = false; 
    public Transform tumpukanContainer;       
    public GameObject prefabVisualBahan2D;

    [Header("Pengaturan Alat / Kompor")]
    public string applianceName;
    public bool isStoveBase = false; 
    public Transform applianceMountPoint; 

    [Header("Visual Indikator Kompor (Khusus Kompor)")]
    public SpriteRenderer stoveSpriteRenderer;
    public Sprite spriteKomporMati;
    public Sprite spriteKomporNyala;

    [Header("Minigame & Resep (Diisi di Alat / Panci)")]
    [SerializeField] private MonoBehaviour minigameScript; 
    [SerializeField] private MonoBehaviour minigameAlternatif;
    public List<RecipeData> resepYangBisaDimasak; 

    [Header("Database & Output")]
    public GameObject draggableItemPrefab;
    public Transform spawnPoint;

    [Header("Visual Bahan & Indikator")]
    public SpriteRenderer applianceSprite2D; 
    public Sprite spriteKosong;              
    public Sprite spriteTerisi;    // Bakal dipake buat state "Lagi Masak / Tutup Rapat"
    public Sprite spriteMasak;
    
    [Header("State Tambahan (Khusus Serabi / Dll)")]
    public Sprite spriteBeres;     // Bakal dipake buat state "Udah Matang"
    public IngredientData targetHasilUntukSpriteBeres; // BARU

// BARU: simpen hasil resep terakhir yang selesai, dipakai buat cek sprite beres
    private IngredientData hasilResepTerakhir;

    [Header("Recipe UI")]
    [SerializeField] private RecipeProgressUI recipeProgressUI;
    
    // 0 = Kosong, 1 = Lagi Masak, 2 = Udah Beres
    [HideInInspector] public int stateWajan = 0;          

    [System.Serializable]
    public struct VisualBahanMapping
    {
        public IngredientData bahan;
        public Sprite spriteSaatBahanMasuk;
    }
    public List<VisualBahanMapping> visualSpesifikBahan;

    // Data internal
    private CookingAppliance mountedAppliance; // Alat yang sedang menempel di atas kompor ini
    private IMinigameMechanic activeMinigame; 

    private FoodCustomizationController foodCustom;


    public void ResetSetelahDiambil()
    {
        stateWajan = 0;
        currentIngredients.Clear();
        UpdateVisualAlat(null);
    }
    
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;

    public bool AdaAdonanSiapPakai()
    {
        return stateWajan == 2; // sama seperti kondisi "Beres" yang sudah kamu pakai buat sprite priority
    }
   

    private void Awake()
    {
        recipeProgressUI = GetComponent<RecipeProgressUI>();

        RefreshMinigameScript();

        if (recipeProgressUI != null)
            recipeProgressUI.Hide();

        SetStoveState(false);
        UbahStateWajan(0);

        foodCustom = GetComponent<FoodCustomizationController>();
    }

    private void RefreshMinigameScript()
    {
        if (minigameScript != null) 
            activeMinigame = minigameScript as IMinigameMechanic;
        else 
            activeMinigame = GetComponent<IMinigameMechanic>();
    }

    public void UnmountAppliance()
    {
        if (mountedAppliance != null)
        {
            DraggableItem2D dragScript = mountedAppliance.GetComponent<DraggableItem2D>();
            if (dragScript != null) dragScript.enabled = true;

            mountedAppliance.komporInduk = null;
            mountedAppliance.transform.SetParent(null); 
            mountedAppliance = null;

            Debug.Log("Alat berhasil dicopot dari kompor!");
        }
    }

    public bool MountAppliance(CookingAppliance newAppliance)
    {
        if (!isStoveBase) return false;

        if (mountedAppliance != null)
        {
            Destroy(mountedAppliance.gameObject);
            mountedAppliance = null;
        }

        mountedAppliance = newAppliance;
        Transform targetTransform = (applianceMountPoint != null) ? applianceMountPoint : transform;
        newAppliance.transform.SetParent(targetTransform);
        newAppliance.transform.localPosition = Vector3.zero;

        newAppliance.transform.localScale = Vector3.one * 0.88f;

        DraggableItem2D dragScript = newAppliance.GetComponent<DraggableItem2D>();
        if (dragScript != null) dragScript.enabled = false;

        newAppliance.komporInduk = this;
        SetStoveState(false);

        return true;
    }

    public CookingAppliance GetMountedAppliance()
    {
        return mountedAppliance;
    }

    private void SetStoveState(bool isCooking)
    {
        if (!isStoveBase || stoveSpriteRenderer == null) return;

        if (isCooking && spriteKomporNyala != null)
            stoveSpriteRenderer.sprite = spriteKomporNyala;
        else if (!isCooking && spriteKomporMati != null)
            stoveSpriteRenderer.sprite = spriteKomporMati;
    }

    // --- FUNGSI BARU UNTUK GANTI STATE WAJAN ---
    public void UbahStateWajan(int stateIndex)
    {
        stateWajan = stateIndex;
        UpdateVisualAlat(null);
    }

    public void AddIngredient(IngredientData ingredient)
    {
        if (totalIngredient >= maxIngredient)
        {
            return;
        }

        if(ingredient.typeBahan == TypeBahan.SetengahJadi)
        {
            Debug.Log("Sampe Sini ?");
        }

        totalIngredient++;

        currentIngredients.Add(ingredient);

        if (foodCustom != null)
        foodCustom.AddIngredient(ingredient);

        // Pas bahan masuk, reset wajan biar ga stuck di state "Beres"
        stateWajan = 0; 
        UpdateVisualAlat(ingredient); 
        
        if (komporInduk != null) komporInduk.CheckForValidRecipe();
        else CheckForValidRecipe();
    }

    public void ResetIngredients()
    {
        currentIngredients.Clear();
        currentValidRecipe = null;

        totalIngredient = 0;

        if (foodCustom != null)
            foodCustom.ResetCustomization();

        if (recipeProgressUI != null)
            recipeProgressUI.Hide();

        UbahStateWajan(0);
    }

    private void CheckForValidRecipe()
{
    // Tentukan alat mana yang menyimpan resep (wajan/panci/kompor)
    CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;

    alatYangDipakai.currentValidRecipe = null;

    List<RecipeData> activeRecipes = alatYangDipakai.resepYangBisaDimasak;
    if (activeRecipes == null || activeRecipes.Count == 0) return;

    if (LevelManager.Instance != null && LevelManager.Instance.currentActiveRecipes != null)
    {
        activeRecipes = activeRecipes.FindAll(r => LevelManager.Instance.currentActiveRecipes.Contains(r));
    }

    if (activeRecipes.Count == 0) return;

    List<IngredientData> bahanDiWadah = alatYangDipakai.currentIngredients;

    RecipeData resepTerbaik = null;
    int jumlahBahanTerbanyak = -1;

    RecipeData resepProgress = null;
    int jumlahBahanCocokTerbanyak = -1;

    foreach (var resep in activeRecipes)
    {
        List<IngredientData> sisaBahan = new List<IngredientData>(bahanDiWadah);
        int jumlahBahanCocok = 0;
        bool semuaBahanWajibAda = true;

        foreach (var bahanWajib in resep.inputIngredients)
        {
            if (sisaBahan.Contains(bahanWajib))
            {
                sisaBahan.Remove(bahanWajib);
                jumlahBahanCocok++;
            }
            else
            {
                semuaBahanWajibAda = false;
            }
        }

        if (jumlahBahanCocok > jumlahBahanCocokTerbanyak)
        {
            jumlahBahanCocokTerbanyak = jumlahBahanCocok;
            resepProgress = resep;
        }

        if (semuaBahanWajibAda)
        {
            bool sisaBahanHanyaBumbu = true;
            foreach (var sisa in sisaBahan)
            {
                if (sisa.peranBahan == PeranBahan.Biasa || sisa.peranBahan == PeranBahan.Tepung)
                {
                    sisaBahanHanyaBumbu = false;
                    break;
                }
            }

            if (sisaBahanHanyaBumbu && resep.inputIngredients.Count > jumlahBahanTerbanyak)
            {
                jumlahBahanTerbanyak = resep.inputIngredients.Count;
                resepTerbaik = resep;
            }
        }
    }

    // Simpan resep yang valid ke alat & kompor
    alatYangDipakai.currentValidRecipe = resepTerbaik;
    this.currentValidRecipe = resepTerbaik; 

    // ==========================
    // FIX UPDATE RECIPE UI
    // ==========================
    // Cari UI di alat yang dipakai, kalau tidak ada fallback ke kompor ini
    RecipeProgressUI targetUI = (alatYangDipakai.recipeProgressUI != null) 
                                ? alatYangDipakai.recipeProgressUI 
                                : this.recipeProgressUI;

    if (targetUI != null && resepProgress != null)
    {
        targetUI.UpdateUI(
            bahanDiWadah,
            resepProgress.inputIngredients,
            resepTerbaik != null
        );
    }
}

    public void OnStartButtonClicked()
    {
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        RecipeData resepAktif = alatYangDipakai.currentValidRecipe;
        IMinigameMechanic targetMinigame = this.activeMinigame;

        if (targetMinigame == null)
        {
            this.RefreshMinigameScript();
            targetMinigame = this.activeMinigame;
        }

        // --- LOGIKA KHUSUS TALENAN (Tanpa merusak alat lain) ---
        if (resepAktif != null)
        {
            // Cek kalau resep minta mekanik Roll, ganti targetnya ke minigame alternatif
            if (resepAktif.requiredMechanic == CookingMechanicType.Roll)
            {
                if (minigameAlternatif != null)
                    targetMinigame = minigameAlternatif as IMinigameMechanic;
                else 
                    targetMinigame = GetComponent<RollerMinigame>(); // Fallback otomatis biar sat-set
            }
        }

        if (targetMinigame == null) return;

        if (resepAktif != null)
        {
            if (recipeProgressUI != null)
            recipeProgressUI.Hide();

            if (komporInduk != null &&
                komporInduk.recipeProgressUI != null)
            {
                komporInduk.recipeProgressUI.Hide();
            }

            SetStoveState(true);
            
            // Trigger state masak
            alatYangDipakai.UbahStateWajan(1); 

            targetMinigame.StartMinigame(resepAktif, OnMinigameFinished);
            totalIngredient = 0;
        }
    }

    private void OnMinigameFinished(float finalScore)
    {
        SetStoveState(false);

        if (foodCustom != null)
        {
            CustomizationResult result = foodCustom.GetResult();

        }
        
        
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        RecipeData resepSelesai = alatYangDipakai.currentValidRecipe;
        alatYangDipakai.hasilResepTerakhir = (resepSelesai != null) ? resepSelesai.successResult : null;

        if (resepSelesai != null)
        {
            IngredientData hasilAkhir = (finalScore >= 0.6f) ? resepSelesai.successResult : resepSelesai.failResult;
            
            // GANTI: cek flag per-resep, bukan draggableItemPrefab appliance
            if (hasilAkhir != null && draggableItemPrefab != null && !resepSelesai.tidakSpawnHasil)
            {
                Transform titikSpawn = (alatYangDipakai.spawnPoint != null) ? alatYangDipakai.spawnPoint : alatYangDipakai.transform;
                GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity);
                
                DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();

                if (dragScript2D != null)
                {
                    dragScript2D.SetupData(hasilAkhir);

                    if (foodCustom != null)
                    {
                        dragScript2D.customization = foodCustom.GetResult();
                    }
                }
            }
        }
        
        alatYangDipakai.currentIngredients.Clear();
        alatYangDipakai.currentValidRecipe = null;
        if(foodCustom != null)
            foodCustom.ResetCustomization();
        
        alatYangDipakai.UbahStateWajan(2); 
    }

    private void UpdateVisualAlat(IngredientData ingredient)
    {
        if (applianceSprite2D != null)
        {
            Sprite targetSprite = spriteKosong; 

            // 1. Prioritas Utama: Kalau state 2 (Udah Beres) DAN memang boleh nampilin spriteBeres
            if (stateWajan == 2 && spriteBeres != null && BolehTampilkanSpriteBeres())
            {
                targetSprite = spriteBeres;
            }
            // 2. Kalau state 1 (Lagi Proses Masak)
            else if (stateWajan == 1 && spriteMasak != null)
            {
                targetSprite = spriteMasak;
            }
            // 3. Kalau belum dimasak, tapi SUDAH ADA bahan di dalamnya
            else if (currentIngredients.Count > 0)
            {
                targetSprite = spriteTerisi; 
                
                IngredientData bahanTerakhir = currentIngredients[currentIngredients.Count - 1]; 
                foreach (var mapping in visualSpesifikBahan)
                {
                    if (mapping.bahan == bahanTerakhir)
                    {
                        if (mapping.spriteSaatBahanMasuk != null)
                        {
                            targetSprite = mapping.spriteSaatBahanMasuk;
                        }
                        break;
                    }
                }
            }
            
            applianceSprite2D.sprite = targetSprite;
        }

        // Tumpukan Bahan (Mangkuk/Panci)
        if (gunakanTumpukanVisual && tumpukanContainer != null && prefabVisualBahan2D != null)
        {
            foreach (Transform child in tumpukanContainer) Destroy(child.gameObject);

            for (int i = 0; i < currentIngredients.Count; i++)
            {
                IngredientData bahan = currentIngredients[i];
                GameObject visualBaru = Instantiate(prefabVisualBahan2D, tumpukanContainer);
                SpriteRenderer sr =
                    visualBaru.GetComponent<SpriteRenderer>();
                
                if (bahan.typeBahanInBowl == TypeBahanInBowl.Tepung)
                {
                    visualBaru.transform.localScale = bahan.scaleSaatMasukBowl;
                    visualBaru.transform.localPosition = new Vector3(0, 0, 0); 
                }
                else if (bahan.typeBahanInBowl == TypeBahanInBowl.Cairan)
                {
                    visualBaru.transform.localScale = bahan.scaleSaatMasukBowl;
                    visualBaru.transform.localPosition = new Vector3(0, 0, 0); 
                }
                else
                {
                    visualBaru.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 

                    float randomX =
                    Random.Range(-0.2f, 0.2f);

                    float randomY =
                        Random.Range(0f, 0f);

                    visualBaru.transform.localPosition =
                        new Vector3(
                            randomX,
                            randomY,
                            0f
                        );
                }

                switch (bahan.typeBahanInBowl)
                {
                    case TypeBahanInBowl.Cairan:

                        sr.sortingOrder = 1;

                        break;


                    case TypeBahanInBowl.Tepung:

                        sr.sortingOrder = 10;

                        break;


                    case TypeBahanInBowl.Normal:

                        sr.sortingOrder = 20 + i;

                        break;
                }
                
                if (sr != null)
                {
                    if (bahan.inBowlIcon != null) sr.sprite = bahan.inBowlIcon;
                    else if (bahan.dragIcon != null) sr.sprite = bahan.dragIcon;
                    else sr.sprite = bahan.icon;
                }

        
            }
        }

        // Indikator UI Icon Bahan
    }
    private bool BolehTampilkanSpriteBeres()
    {
        // Kalau field dikosongin di Inspector, behavior lama: selalu muncul
        if (targetHasilUntukSpriteBeres == null) return true;

        // Kalau diisi, cuma muncul kalau hasil resep terakhir PERSIS sama
        return hasilResepTerakhir == targetHasilUntukSpriteBeres;
    }

  
}