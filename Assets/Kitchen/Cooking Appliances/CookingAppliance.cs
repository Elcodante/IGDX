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

    // Variabel internal penghitung bumbu
    private int countManis = 0;
    private int countLembut = 0;
    private int countGurih = 0;
    private int countIsian = 0;
    private JenisTepung jenisTepung = JenisTepung.Terigu; // Default

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

    public void AddIngredient(IngredientData ingredient)
    {
        currentIngredients.Add(ingredient);
        
        if (ingredient.peranBahan == PeranBahan.BumbuManis) countManis++;
        else if (ingredient.peranBahan == PeranBahan.BumbuLembut) countLembut++;
        else if (ingredient.peranBahan == PeranBahan.BumbuGurih) countGurih++;
        else if (ingredient.peranBahan == PeranBahan.Isian) countIsian++;
        else if (ingredient.peranBahan == PeranBahan.Tepung) jenisTepung = ingredient.jenisTepung;

        UpdateVisualAlat(); 
        
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
        UpdateVisualAlat(); 
    }

    private void CheckForValidRecipe()
    {
        // 1. Tentukan siapa yang lagi dipakai masak? (Kalau ada alat menempel, pakai alat itu. Kalau kosong, pakai kompor)
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        
        // Reset validasi sebelumnya
        alatYangDipakai.currentValidRecipe = null;
        
        GameObject btnStartAktif = (komporInduk != null && komporInduk.startButtonUI != null) ? komporInduk.startButtonUI : startButtonUI;
        if (btnStartAktif != null) btnStartAktif.SetActive(false);

        // 2. Ambil HANYA resep dan bahan dari alat yang lagi dipakai!
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
                if (sisaBahanEkstra.Contains(bahanWajib))
                {
                    sisaBahanEkstra.Remove(bahanWajib);
                }
                else
                {
                    semuaBahanWajibAda = false;
                    break;
                }
            }

            if (semuaBahanWajibAda)
            {
                bool sisaBahanHanyaBumbu = true;

                foreach (var sisa in sisaBahanEkstra)
                {
                    if (sisa.peranBahan == PeranBahan.Biasa || sisa.peranBahan == PeranBahan.Tepung)
                    {
                        sisaBahanHanyaBumbu = false;
                        break;
                    }
                }

                if (sisaBahanHanyaBumbu)
                {
                    if (resep.inputIngredients.Count > jumlahBahanTerbanyak)
                    {
                        jumlahBahanTerbanyak = resep.inputIngredients.Count;
                        resepTerbaik = resep;
                    }
                }
            }
        }

        // 3. Simpan resep yang valid HANYA ke alat yang bersangkutan
        alatYangDipakai.currentValidRecipe = resepTerbaik;

        if (alatYangDipakai.currentValidRecipe != null && btnStartAktif != null)
        {
            btnStartAktif.SetActive(true);
        }
    }

    public void OnStartButtonClicked()
    {
        // 1. Tentukan alat mana yang lagi dipakai buat ambil data resepnya
        CookingAppliance alatYangDipakai = (mountedAppliance != null) ? mountedAppliance : this;
        RecipeData resepAktif = alatYangDipakai.currentValidRecipe;

        // 2. TAPI, Minigamenya SELALU pakai milik Kompor Base (this)
        IMinigameMechanic targetMinigame = this.activeMinigame;

        if (targetMinigame == null)
        {
            this.RefreshMinigameScript();
            targetMinigame = this.activeMinigame;
        }

        if (targetMinigame == null)
        {
            Debug.LogError("Error: Script Minigame belum dipasang di Kompor Base!");
            return;
        }

        // 3. Jalankan minigame kompor, tapi oper resep dari alat (wajan/kukusan)
        if (resepAktif != null)
        {
            if (startButtonUI != null) startButtonUI.SetActive(false);
            if (komporInduk != null && komporInduk.startButtonUI != null) komporInduk.startButtonUI.SetActive(false);

            SetStoveState(true);
            
            // Perhatikan bahwa callback OnMinigameFinished akan tetap memanggil fungsi di Kompor
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
                    
                    // Ambil bumbu dari alat yang dipakai
                    dragScript2D.tepungDigunakan = alatYangDipakai.jenisTepung;
                    dragScript2D.tingkatManis = KonversiKeTingkatRasa(alatYangDipakai.countManis);
                    dragScript2D.tingkatLembut = KonversiKeTingkatRasa(alatYangDipakai.countLembut);
                    dragScript2D.tingkatGurih = KonversiKeTingkatRasa(alatYangDipakai.countGurih);
                    dragScript2D.tingkatIsian = KonversiKeTingkatIsian(alatYangDipakai.countIsian);
                }
            }
        }
        
        alatYangDipakai.ResetIngredients();
        alatYangDipakai.UpdateVisualAlat();
    }

    private void UpdateVisualAlat()
    {

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

    private TingkatRasa KonversiKeTingkatRasa(int jumlah)
    {
        if (jumlah == 0) return TingkatRasa.TidakPakai;
        if (jumlah == 1) return TingkatRasa.Sedikit;
        if (jumlah == 2) return TingkatRasa.Sedang;
        return TingkatRasa.Banyak;
    }

    private TingkatIsian KonversiKeTingkatIsian(int jumlah)
    {
        if (jumlah <= 1) return TingkatIsian.Sedikit; // 0 atau 1 dianggap sedikit
        if (jumlah == 2) return TingkatIsian.Sedang;
        return TingkatIsian.Banyak;
    }
}