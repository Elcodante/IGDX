using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Cooking/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public string ingredientID;      // Contoh: "flour", "egg", "raw_dough", "cake"
    public string ingredientName;    // Contoh: "Tepung", "Telur", "Adonan Mentah"
    public Sprite icon;              // Ikon untuk ditampilkan di UI
    
}