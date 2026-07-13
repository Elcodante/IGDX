using System.Collections.Generic;
using UnityEngine;

public class CookingAppliance : MonoBehaviour
{
    [Header("Setting Alat")]
    public string applianceName;
    public GameObject startButtonUI; 
    [SerializeField] private MonoBehaviour minigameScript; 
    
    [Header("Database dan Output")]
    public List<RecipeData> resepYangBisaDimasak;
    public GameObject draggableItemPrefab;
    public Transform spawnPoint;

    private IMinigameMechanic activeMinigame; 
    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private RecipeData currentValidRecipe;

    private void Awake()
    {
        if (minigameScript != null) activeMinigame = minigameScript as IMinigameMechanic;
        if (activeMinigame == null) activeMinigame = GetComponent<IMinigameMechanic>();
        if (startButtonUI != null) startButtonUI.SetActive(false);
    }

    public void AddIngredient(IngredientData ingredient)
    {
        currentIngredients.Add(ingredient);
        CheckForValidRecipe();
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
        else
        {
            Debug.Log($"Bahan terkumpul: {currentIngredients.Count}. Masih kurang atau belum cocok.");
        }
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Tombol Start pada " + applianceName + " berhasil di-klik!");

        if (startButtonUI != null) startButtonUI.SetActive(false);
        
        if (activeMinigame == null)
        {
            Debug.LogError("Gagal! Skrip minigame belum terpasang di alat ini.");
            return;
        }

        if (currentValidRecipe == null)
        {
            Debug.LogError("Gagal! Resep mendadak kosong saat tombol ditekan.");
            return;
        }

        Debug.Log("Semua aman, menyalakan minigame...");
        activeMinigame.StartMinigame(currentValidRecipe, OnMinigameFinished); 
    }

    private void OnMinigameFinished(float finalScore)
    {
        IngredientData hasilAkhir = (finalScore >= 0.6f) ? currentValidRecipe.successResult : currentValidRecipe.failResult;
        
        if (hasilAkhir != null && draggableItemPrefab != null)
        {
            Transform titikSpawn = (spawnPoint != null) ? spawnPoint : transform;
            GameObject objekBaru = Instantiate(draggableItemPrefab, titikSpawn.position, Quaternion.identity, titikSpawn.parent);
            
            DraggableItem2D dragScript2D = objekBaru.GetComponent<DraggableItem2D>();
            if (dragScript2D != null) 
            {
                dragScript2D.SetupData(hasilAkhir); 
            }
        }
        currentIngredients.Clear(); 
    }
}