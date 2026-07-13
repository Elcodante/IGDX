using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cooking/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Bahan & Syarat")]
    public List<IngredientData> inputIngredients; // Bahan yang dibutuhkan
    public CookingMechanicType requiredMechanic;  // Enum: Stir, Chop, SweetSpot, Passive

    [Header("Tingkat Kesulitan (Adjustable)")]
    public float timeLimit = 10f;       // Batas waktu minigame
    public float targetDifficulty = 1f; // Pengali kesulitan

    [Header("Hasil")]
    public IngredientData successResult; // Jika skor bagus 
    public IngredientData failResult;    // Jika skor jelek 
}

public enum CookingMechanicType { None, Stir, Chop, SweetSpot, Passive }