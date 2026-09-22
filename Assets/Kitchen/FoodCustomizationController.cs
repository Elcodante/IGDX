using UnityEngine;

[System.Serializable]
public struct CustomizationResult
{
    public Tingkat manis;
    public Tingkat lembut;
    public Tingkat gurih;
    public Tingkat isian;
}

public class FoodCustomizationController : MonoBehaviour
{
    public CustomizationResult customization;
    private int countManis = 0;
    private int countLembut = 0;
    private int countGurih = 0;
    private int countIsian = 0;

    public void AddIngredient(IngredientData ingredient)
    {
        switch (ingredient.peranBahan)
        {
            case PeranBahan.BumbuManis:
                countManis++;
                break;

            case PeranBahan.BumbuLembut:
                countLembut++;
                break;

            case PeranBahan.BumbuGurih:
                countGurih++;
                break;

            case PeranBahan.Isian:
                countIsian++;
                Debug.Log("Sampe Sini Gak, Masa Gak Nambah isian");
                break;
        }
    }

    public CustomizationResult GetResult()
    {
        // 1. Hitung tingkat baru berdasarkan bahan yang baru saja dimasukkan (AddIngredient)
        Tingkat hasilManis = ConvertToTingkat(countManis);
        Tingkat hasilLembut = ConvertToTingkat(countLembut);
        Tingkat hasilGurih = ConvertToTingkat(countGurih);
        Tingkat hasilIsian = ConvertToTingkat(countIsian);

        // 2. Jika sebelumnya sudah ada data dari AmbilResult(), gunakan data itu 
        // KECUALI jika ada bahan baru yang dimasukkan (hasilnya bukan TidakAda)
        return new CustomizationResult
        {
            manis = (hasilManis != Tingkat.TidakAda) ? hasilManis : customization.manis,
            lembut = (hasilLembut != Tingkat.TidakAda) ? hasilLembut : customization.lembut,
            gurih = (hasilGurih != Tingkat.TidakAda) ? hasilGurih : customization.gurih,
            isian = (hasilIsian != Tingkat.TidakAda) ? hasilIsian : customization.isian
        };
    }



    private Tingkat ConvertToTingkat(int count)
    {
        if (count == 0)
            return Tingkat.TidakAda;

        if (count == 1)
            return Tingkat.Sedikit;

        if (count == 2)
            return Tingkat.Lumayan;

        return Tingkat.Sangat;
    }

    public void AmbilResult(CustomizationResult custom)
    {
        customization = custom;
    }

    public CustomizationResult GetResultIfAda()
    {
        return customization;
    }


    public void ResetCustomization()
    {
        countManis = 0;
        countLembut = 0;
        countGurih = 0;
        countIsian = 0;
    }
}