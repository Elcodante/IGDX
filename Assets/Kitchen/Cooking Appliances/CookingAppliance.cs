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
    public bool isStoveBase = false; 
    public Transform applianceMountPoint; 
    public GameObject startButtonUI; 

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
    
    // 0 = Kosong, 1 = Lagi Masak, 2 = Udah Beres
    [HideInInspector] public int stateWajan = 0;          

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

    // Variabel internal penghitung bumbu
    private int countManis = 0;
    private int countLembut = 0;
    private int countGurih = 0;
    private int countIsian = 0;
    private JenisTepung jenisTepung = JenisTepung.Terigu; // Default

    // Data internal
    private CookingAppliance mountedAppliance; // Alat yang sedang menempel di atas kompor ini
    private IMinigameMechanic activeMinigame; 
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
        RefreshMinigameScript();
        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        SetStoveState(false);
        UbahStateWajan(0); // Set ke kosong saat mulai
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
        currentIngredients.Add(ingredient);

        if(ingredient.typeBahanInBowl == TypeBahanInBowl.Tepung)
        {
            
        }
        
        if (ingredient.peranBahan == PeranBahan.BumbuManis) countManis++;
        else if (ingredient.peranBahan == PeranBahan.BumbuLembut) countLembut++;
        else if (ingredient.peranBahan == PeranBahan.BumbuGurih) countGurih++;
        else if (ingredient.peranBahan == PeranBahan.Isian) countIsian++;
        else if (ingredient.peranBahan == PeranBahan.Tepung) jenisTepung = ingredient.jenisTepung;

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
        countManis = 0;
        countLembut = 0;
        countGurih = 0;
        countIsian = 0;
        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        UbahStateWajan(0); // Otomatis balik ke spriteKosong
    }

    private void CheckForValidRecipe()
    {
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        alatYangDipakai.currentValidRecipe = null;
        
        GameObject btnStartAktif = (komporInduk != null && komporInduk.startButtonUI != null) ? komporInduk.startButtonUI : startButtonUI;
        if (btnStartAktif != null) btnStartAktif.SetActive(false);

        List<RecipeData> activeRecipes = alatYangDipakai.resepYangBisaDimasak;
        if (activeRecipes == null || activeRecipes.Count == 0) return;

        List<IngredientData> bahanDiWadah = alatYangDipakai.currentIngredients;
        RecipeData resepTerbaik = null;
        int jumlahBahanTerbanyak = -1;

        foreach (var resep in activeRecipes)
        {
            List<IngredientData> sisaBahanEkstra = new List<IngredientData>(bahanDiWadah);
            bool semuaBahanWajibAda = true;

            foreach (var bahanWajib in resep.inputIngredients)
            {
                if (sisaBahanEkstra.Contains(bahanWajib)) sisaBahanEkstra.Remove(bahanWajib);
                else { semuaBahanWajibAda = false; break; }
            }

            if (semuaBahanWajibAda)
            {
                bool sisaBahanHanyaBumbu = true;
                foreach (var sisa in sisaBahanEkstra)
                {
                    if (sisa.peranBahan == PeranBahan.Biasa || sisa.peranBahan == PeranBahan.Tepung)
                    {
                        sisaBahanHanyaBumbu = false; break;
                    }
                }

                if (sisaBahanHanyaBumbu && resep.inputIngredients.Count > jumlahBahanTerbanyak)
                {
                    jumlahBahanTerbanyak = resep.inputIngredients.Count;
                    resepTerbaik = resep;
                }
            }
        }

        alatYangDipakai.currentValidRecipe = resepTerbaik;
        if (alatYangDipakai.currentValidRecipe != null && btnStartAktif != null) btnStartAktif.SetActive(true);
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
            if (startButtonUI != null) startButtonUI.SetActive(false);
            if (komporInduk != null && komporInduk.startButtonUI != null) komporInduk.startButtonUI.SetActive(false);

            SetStoveState(true);
            
            // Trigger state masak
            alatYangDipakai.UbahStateWajan(1); 

            targetMinigame.StartMinigame(resepAktif, OnMinigameFinished);
        }
    }

    private void OnMinigameFinished(float finalScore)
    {
        SetStoveState(false);
        
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        RecipeData resepSelesai = alatYangDipakai.currentValidRecipe;

        if (resepSelesai != null)
        {
            IngredientData hasilAkhir = (finalScore >= 0.6f) ? resepSelesai.successResult : resepSelesai.failResult;
            
            if (hasilAkhir != null && draggableItemPrefab != null)
            {
                Transform titikSpawn = (alatYangDipakai.spawnPoint != null) ? alatYangDipakai.spawnPoint : alatYangDipakai.transform;
                GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity);
                
                DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();
                if (dragScript2D != null)
                {
                    dragScript2D.SetupData(hasilAkhir);
                    dragScript2D.tepungDigunakan = alatYangDipakai.jenisTepung;
                    dragScript2D.tingkatManis = KonversiKeTingkatRasa(alatYangDipakai.countManis);
                    dragScript2D.tingkatLembut = KonversiKeTingkatRasa(alatYangDipakai.countLembut);
                    dragScript2D.tingkatGurih = KonversiKeTingkatRasa(alatYangDipakai.countGurih);
                    dragScript2D.tingkatIsian = KonversiKeTingkatIsian(alatYangDipakai.countIsian);
                }
            }
        }
        
        // Hapus bahan lama dari memori
        alatYangDipakai.currentIngredients.Clear();
        alatYangDipakai.currentValidRecipe = null;
        alatYangDipakai.countManis = 0; alatYangDipakai.countLembut = 0; alatYangDipakai.countGurih = 0; alatYangDipakai.countIsian = 0;
        
        // --- TRIGGER STATE BERES (2) OTOMATIS SAAT MINIGAME KELAR ---
        alatYangDipakai.UbahStateWajan(2); 
    }

    private void UpdateVisualAlat(IngredientData ingredient)
    {
        if (applianceSprite2D != null)
        {
            Sprite targetSprite = spriteKosong; 

            // 1. Prioritas Utama: Kalau state 2 (Udah Beres)
            if (stateWajan == 2 && spriteBeres != null)
            {
                targetSprite = spriteBeres;
            }
            // 2. Kalau state 1 (Lagi Proses Masak / Tombol Start ditekan)
            else if (stateWajan == 1 && spriteMasak != null)
            {
                targetSprite = spriteMasak;
            }
            // 3. Kalau belum dimasak, tapi SUDAH ADA bahan di dalamnya
            else if (currentIngredients.Count > 0)
            {
                targetSprite = spriteTerisi; 
                
                // Cek apakah ada gambar mangkuk/wajan custom dari bahan tertentu
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
                
                if (bahan.typeBahanInBowl == TypeBahanInBowl.Tepung)
                {
                    visualBaru.transform.localScale = bahan.scaleSaatMasukBowl;
                }
                else
                {
                    visualBaru.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
                }

                SpriteRenderer sr = visualBaru.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    if (bahan.inBowlIcon != null) sr.sprite = bahan.inBowlIcon;
                    else if (bahan.dragIcon != null) sr.sprite = bahan.dragIcon;
                    else sr.sprite = bahan.icon;

                    sr.sortingOrder = i + 1;
                }

            
                visualBaru.transform.localPosition = new Vector3(0, 0, 0); 
            }
        }

        // Indikator UI Icon Bahan
        Transform targetContainer = (indikatorContainer != null) ? indikatorContainer : (komporInduk != null ? komporInduk.indikatorContainer : null);
        GameObject targetPrefab = (indikatorPrefab != null) ? indikatorPrefab : (komporInduk != null ? komporInduk.indikatorPrefab : null);

        if (targetContainer == null || targetPrefab == null) return;
        if (currentIngredients.Count == 0) { targetContainer.gameObject.SetActive(false); return; }

        targetContainer.gameObject.SetActive(true);
        foreach (Transform child in targetContainer) Destroy(child.gameObject);

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

    private TingkatRasa KonversiKeTingkatRasa(int jumlah)
    {
        if (jumlah == 0) return TingkatRasa.TidakPakai;
        if (jumlah == 1) return TingkatRasa.Sedikit;
        if (jumlah == 2) return TingkatRasa.Sedang;
        return TingkatRasa.Banyak;
    }

    private TingkatIsian KonversiKeTingkatIsian(int jumlah)
    {
        if (jumlah <= 1) return TingkatIsian.Sedikit; 
        if (jumlah == 2) return TingkatIsian.Sedang;
        return TingkatIsian.Banyak;
    }
}