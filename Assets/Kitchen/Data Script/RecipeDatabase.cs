using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance { get; private set; }

    [Tooltip("Semua Resep")]
    public List<RecipeData> semuaResep;

    // Index: IngredientData hasil -> RecipeData yang menghasilkannya
    private Dictionary<IngredientData, RecipeData> resepByResult = new Dictionary<IngredientData, RecipeData>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var resep in semuaResep)
        {
            if (resep.successResult != null)
                resepByResult[resep.successResult] = resep;
        }
    }

    // Rekursif: cari semua resep yang dibutuhkan untuk menghasilkan 1 ingredient
    public void KumpulkanResepUntuk(IngredientData target, HashSet<RecipeData> hasil, HashSet<IngredientData> sudahDicek)
    {
        if (target == null || sudahDicek.Contains(target)) return;
        sudahDicek.Add(target);

        if (!resepByResult.TryGetValue(target, out RecipeData resep)) return; // bahan mentah, stop di sini
        if (hasil.Contains(resep)) return;

        hasil.Add(resep);

        // Runut mundur lagi ke bahan-bahan input resep ini
        foreach (var input in resep.inputIngredients)
        {
            KumpulkanResepUntuk(input, hasil, sudahDicek);
        }
    }

    // Entry point: dari daftar menu level ini, hasilkan semua resep yang valid
    public HashSet<RecipeData> GetActiveRecipes(MenuData[] menuList)
    {
        HashSet<RecipeData> hasil = new HashSet<RecipeData>();
        HashSet<IngredientData> sudahDicek = new HashSet<IngredientData>();

        foreach (var menu in menuList)
        {
            // asumsi: MenuData/OrderData nunjuk ke IngredientData produk jadi
            // (kalau belum ada field ini, tambahin ke MenuData — lihat catatan di bawah)
            if (menu.finalProduct != null)
                KumpulkanResepUntuk(menu.finalProduct, hasil, sudahDicek);
        }

        return hasil;
    }
}