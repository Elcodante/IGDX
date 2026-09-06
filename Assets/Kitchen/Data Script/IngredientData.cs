using UnityEngine;
using System.Collections.Generic;
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

    public GameObject dropVisualPrefab;

    public Sprite dragIcon;

    public bool usePourAnimation = false;   

    public Sprite inBowlIcon;

    [Header("Garnish (isi HANYA di makanan matang yang butuh garnish)")]
    public bool butuhGarnish = false;
    public List<IngredientData> daftarGarnishDibutuhkan; // Gethuk/Putu Ayu: isi 1. Lupis/Cenil: isi 2 (gula + kelapa)
    public List<Sprite> spriteTiapTahapGarnish;           // sprite[0] = setelah garnish pertama masuk, sprite[1] = setelah garnish kedua (final), dst.
    }