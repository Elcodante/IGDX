using UnityEngine;
public enum PeranBahan { 
    Biasa,         
    Tepung,         
    BumbuManis,     
    BumbuLembut,   
    BumbuGurih,    
    Isian          
}

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Cooking/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public string ingredientID;     
    public string ingredientName;    
    public Sprite icon;              
    
    [Header("Atribut Pesanan)")]
    public PeranBahan peranBahan = PeranBahan.Biasa; 
    
    [Tooltip("Hanya berlaku jika Peran Bahan diset ke 'Tepung'")]
    public JenisTepung jenisTepung; 
}