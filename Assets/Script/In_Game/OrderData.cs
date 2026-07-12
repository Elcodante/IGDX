using UnityEngine;
public enum TingkatIsian { Sedikit, Sedang, Banyak };
public enum JenisTepung { Tapioka, Terigu, Ketan, Beras };

[System.Serializable]
public struct OrderData
{
    public string idResep;

    [Header("Kostumisasi bahan")]
    public TingkatIsian isian;
    public JenisTepung tepung;

    [Header("Target Rasa")]
    public float targetManis;
    public float targetLembut;
    public float targetGurih;
};