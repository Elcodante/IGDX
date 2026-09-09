using UnityEngine;

public enum JenisTepung { 
    Tapioka, 
    Terigu, 
    Ketan, 
    Beras 
};

public enum Tingkat{ 
    Sedikit, 
    Lumayan, 
    Sangat
};

public enum JenisCustomization
{
    Isian,
    Manis,
    Lembut,
    Gurih,
}

[System.Serializable]
public struct CustomizationData
{
    public JenisCustomization jenis;
    public IngredientData ingredient;
    
    public Tingkat target;
}

[System.Serializable]
public struct OrderData
{
    public string idResep;
    public Sprite ikonMakanan;

    [Header("Kustomisasi")]
    public CustomizationData[] customizations;
}