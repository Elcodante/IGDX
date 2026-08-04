using UnityEngine;
public enum TingkatIsian { Sedikit, Sedang, Banyak };
public enum JenisTepung { Tapioka, Terigu, Ketan, Beras };

public enum TingkatRasa {  TidakPakai, Sedikit, Sedang, Banyak };

[System.Serializable]
public struct OrderData
{
    public string idResep;
    public Sprite ikonMakanan;

    [Header("Kostumisasi bahan")]
    public TingkatIsian isian;
    public JenisTepung tepung;

    [Header("Target Rasa")]
    public TingkatRasa targetManis;
    public TingkatRasa targetLembut;
    public TingkatRasa targetGurih;
};