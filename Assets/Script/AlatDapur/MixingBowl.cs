using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingBowl : MonoBehaviour
{
    [Header("Tumpukan Bahan")]
    [SerializeField] private bool gunakanTumpukanVisual = false; 
    [SerializeField] private Transform tumpukanContainer;       
    [SerializeField] private GameObject prefabVisualBahan2D;

    [Header("Pengaturan Mangkok")]
    [SerializeField] private GameObject startButtonUI; 

    [Header("Minigame & Resep")]
    [SerializeField] private MonoBehaviour minigameScript; 
    [SerializeField] private MonoBehaviour minigameAlternatif;
    [SerializeField] public List<RecipeData> resepYangBisaDibuat; 

    [Header("Output Adonan")]
    [SerializeField] public GameObject draggableItemPrefab;
    [SerializeField] public Transform spawnPoint;


    [Header("Visual Mangkok & Sprite State")]
    [SerializeField] public SpriteRenderer bowlSprite2D; 
    [SerializeField] public Sprite spriteKosong;              
    [SerializeField] public Sprite spriteTerisi;    
    [SerializeField] public Sprite spriteSedangProses;
    [SerializeField] public Sprite spriteSelesai;     
    
    [HideInInspector] public int stateMangkok = 0;          


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

   
    private int countManis = 0;
    private int countLembut = 0;
    private int countGurih = 0;
    private int countIsian = 0;
    private JenisTepung jenisTepung = JenisTepung.Terigu;

    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;
    private IMinigameMechanic activeMinigame; 

    private void Awake()
    {
        RefreshMinigameScript();
        if (startButtonUI != null) startButtonUI.SetActive(false);
        UbahStateMangkok(0);
    }

    private void RefreshMinigameScript()
    {
        if (minigameScript != null) 
            activeMinigame = minigameScript as IMinigameMechanic;
        else 
            activeMinigame = GetComponent<IMinigameMechanic>();
    }

    public void UbahStateMangkok(int stateIndex)
    {
        stateMangkok = stateIndex;
        UpdateVisualMangkok();
    }

    public bool AdaAdonanSiapPakai()
    {
        return stateMangkok == 2;
    }

    public void AddIngredient(IngredientData ingredient)
    {
        currentIngredients.Add(ingredient);
        
        if (ingredient.peranBahan == PeranBahan.BumbuManis) countManis++;
        else if (ingredient.peranBahan == PeranBahan.BumbuLembut) countLembut++;
        else if (ingredient.peranBahan == PeranBahan.BumbuGurih) countGurih++;
        else if (ingredient.peranBahan == PeranBahan.Isian) countIsian++;
        else if (ingredient.peranBahan == PeranBahan.Tepung) jenisTepung = ingredient.jenisTepung;

        stateMangkok = 0; 
        UpdateVisualMangkok(); 
        CheckForValidRecipe();
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
        
        UbahStateMangkok(0);
    }

    public void ResetSetelahDiambil()
    {
        ResetIngredients();
    }

    private void CheckForValidRecipe()
    {
        currentValidRecipe = null;
        if (startButtonUI != null) startButtonUI.SetActive(false);

        if (resepYangBisaDibuat == null || resepYangBisaDibuat.Count == 0) return;

        RecipeData resepTerbaik = null;
        int jumlahBahanTerbanyak = -1;

        foreach (var resep in resepYangBisaDibuat)
        {
            List<IngredientData> sisaBahan = new List<IngredientData>(currentIngredients);
            bool semuaBahanWajibAda = true;

            foreach (var bahanWajib in resep.inputIngredients)
            {
                if (sisaBahan.Contains(bahanWajib)) sisaBahan.Remove(bahanWajib);
                else { semuaBahanWajibAda = false; break; }
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

        currentValidRecipe = resepTerbaik;
        if (currentValidRecipe != null && startButtonUI != null)
        {
            startButtonUI.SetActive(true);
        }
    }

    public void OnStartButtonClicked()
    {
        IMinigameMechanic targetMinigame = this.activeMinigame;

        if (targetMinigame == null)
        {
            RefreshMinigameScript();
            targetMinigame = this.activeMinigame;
        }

        if (currentValidRecipe != null)
        {
            if (currentValidRecipe.requiredMechanic == CookingMechanicType.Roll)
            {
                if (minigameAlternatif != null)
                    targetMinigame = minigameAlternatif as IMinigameMechanic;
                else 
                    targetMinigame = GetComponent<RollerMinigame>();
            }

            if (targetMinigame == null) return;

            if (startButtonUI != null) startButtonUI.SetActive(false);

            UbahStateMangkok(1); 
            targetMinigame.StartMinigame(currentValidRecipe, OnMinigameFinished);
        }
    }

    private void OnMinigameFinished(float finalScore)
    {
        if (currentValidRecipe != null)
        {
            IngredientData hasilAkhir = (finalScore >= 0.6f) ? currentValidRecipe.successResult : currentValidRecipe.failResult;
            
            if (hasilAkhir != null && draggableItemPrefab != null)
            {
                Transform titikSpawn = (spawnPoint != null) ? spawnPoint : transform;
                GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity);
                
                DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();
                if (dragScript2D != null)
                {
                    dragScript2D.SetupData(hasilAkhir);
                    dragScript2D.tepungDigunakan = jenisTepung;
                    dragScript2D.tingkatManis = KonversiKeTingkatRasa(countManis);
                    dragScript2D.tingkatLembut = KonversiKeTingkatRasa(countLembut);
                    dragScript2D.tingkatGurih = KonversiKeTingkatRasa(countGurih);
                    dragScript2D.tingkatIsian = KonversiKeTingkatIsian(countIsian);
                }
            }
        }
        
        currentIngredients.Clear();
        currentValidRecipe = null;
        countManis = 0; countLembut = 0; countGurih = 0; countIsian = 0;
        
        UbahStateMangkok(2); 
    }

    private void UpdateVisualMangkok()
    {
        if (bowlSprite2D != null)
        {
            Sprite targetSprite = spriteKosong; 

            if (stateMangkok == 2 && spriteSelesai != null)
            {
                targetSprite = spriteSelesai;
            }
            else if (stateMangkok == 1 && spriteSedangProses != null)
            {
                targetSprite = spriteSedangProses;
            }
            else if (currentIngredients.Count > 0)
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
            
            bowlSprite2D.sprite = targetSprite;
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
                    IngredientData bahan = currentIngredients[i];
                    
                    if (bahan.inBowlIcon != null) sr.sprite = bahan.inBowlIcon;
                    else if (bahan.dragIcon != null) sr.sprite = bahan.dragIcon;
                    else sr.sprite = bahan.icon;

                    sr.sortingOrder = i + 1;
                }
                float randomX = Random.Range(-0.2f, 0.2f);
                visualBaru.transform.localPosition = new Vector3(randomX, i * 0.3f, 0); 
            }
        }

    
        if (indikatorContainer == null || indikatorPrefab == null) return;
        if (currentIngredients.Count == 0) { indikatorContainer.gameObject.SetActive(false); return; }

        indikatorContainer.gameObject.SetActive(true);
        foreach (Transform child in indikatorContainer) Destroy(child.gameObject);

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