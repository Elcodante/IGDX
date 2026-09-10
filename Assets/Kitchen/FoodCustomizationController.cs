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
                break;
        }
    }

    public CustomizationResult GetResult()
    {
        if(customization.manis != Tingkat.TidakAda || customization.lembut != Tingkat.TidakAda || customization.gurih != Tingkat.TidakAda || customization.isian != Tingkat.TidakAda)
        {
            return customization;
        }

        return new CustomizationResult
        {
            manis = ConvertToTingkat(countManis),
            lembut = ConvertToTingkat(countLembut),
            gurih = ConvertToTingkat(countGurih),
            isian = ConvertToTingkat(countIsian)
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