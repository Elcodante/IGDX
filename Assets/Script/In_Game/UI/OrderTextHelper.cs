using System.Collections.Generic;

public static class OrderTextHelper
{

    public static string BuatTeksKeyword(OrderData data)

    {

        List<string> keyword = new List<string>();

        foreach (CustomizationData customization in data.customizations)

        {

            string teks = "";

            switch (customization.jenis)

            {

                case JenisCustomization.Isian:

                    switch (customization.target)

                    {

                        case Tingkat.Sedikit:

                            teks = "Isian sedikit";

                            break;

                        case Tingkat.Lumayan:

                            teks = "Isian Sedang";

                            break;

                        case Tingkat.Sangat:

                            teks = "Isian Banyak";

                            break;

                    }

                    break;

                case JenisCustomization.Manis:

                    switch (customization.target)

                    {

                        case Tingkat.Sedikit:

                            teks = "Sedikit manis";

                            break;

                        case Tingkat.Lumayan:

                            teks = "Lumayan manis";

                            break;

                        case Tingkat.Sangat:

                            teks = "Sangat manis";

                            break;

                    }

                    break;

                case JenisCustomization.Gurih:

                    switch (customization.target)

                    {

                        case Tingkat.Sedikit:

                            teks = "Sedikit gurih";

                            break;

                        case Tingkat.Lumayan:

                            teks = "Lumayan gurih";

                            break;

                        case Tingkat.Sangat:

                            teks = "Sangat gurih";

                            break;

                    }

                    break;

                case JenisCustomization.Lembut:

                    switch (customization.target)

                    {

                        case Tingkat.Sedikit:

                            teks = "Sedikit lembut";

                            break;

                        case Tingkat.Lumayan:

                            teks = "Lumayan lembut";

                            break;

                        case Tingkat.Sangat:

                            teks = "Sangat lembut";

                            break;

                    }

                    break;

            }

            if (!string.IsNullOrEmpty(teks))

                keyword.Add(teks);

        }

        if (keyword.Count == 0)

            return "Original";

        return string.Join(", ", keyword);

    }
    
    private static string BuatTeksTingkat(
    Tingkat tingkat,
    JenisCustomization jenis)
    {
    switch (jenis)
    {
        case JenisCustomization.Manis:
            switch (tingkat)
            {
                case Tingkat.Sedikit:
                    return "Sedikit manis";

                case Tingkat.Lumayan:
                    return "Lumayan manis";

                case Tingkat.Sangat:
                    return "Sangat manis";
            }
            break;

        case JenisCustomization.Gurih:
            switch (tingkat)
            {
                case Tingkat.Sedikit:
                    return "Sedikit gurih";

                case Tingkat.Lumayan:
                    return "Lumayan gurih";

                case Tingkat.Sangat:
                    return "Sangat gurih";
            }
            break;

        case JenisCustomization.Lembut:
            switch (tingkat)
            {
                case Tingkat.Sedikit:
                    return "Sedikit lembut";

                case Tingkat.Lumayan:
                    return "Lumayan lembut";

                case Tingkat.Sangat:
                    return "Sangat lembut";
            }
            break;

        case JenisCustomization.Isian:
            switch (tingkat)
            {
                case Tingkat.Sedikit:
                    return "Isian sedikit";

                case Tingkat.Lumayan:
                    return "Isian sedang";

                case Tingkat.Sangat:
                    return "Isian banyak";
            }
            break;
    }

    return "";
    }

    public static string BuatTeksCustomization(CustomizationResult result)
    {
        List<string> teksList = new List<string>();

        if (result.manis != Tingkat.TidakAda)
        {
            teksList.Add(BuatTeksTingkat(result.manis, JenisCustomization.Manis));
        }

        if (result.gurih != Tingkat.TidakAda)
        {
            teksList.Add(BuatTeksTingkat(result.gurih, JenisCustomization.Gurih));
        }

        if (result.lembut != Tingkat.TidakAda)
        {
            teksList.Add(BuatTeksTingkat(result.lembut, JenisCustomization.Lembut));
        }

        if (result.isian != Tingkat.TidakAda)
        {
            teksList.Add(BuatTeksTingkat(result.isian, JenisCustomization.Isian));
        }

        if (teksList.Count == 0)
            return null;

        return string.Join(", ", teksList);
    }


    public static string BuatTeksDialog(OrderData data)
    {
        string dialog = $"\"Aku mau pesan {data.idResep}. ";

        foreach (CustomizationData customization in data.customizations)
        {
            switch (customization.jenis)
            {
                case JenisCustomization.Manis:

                    if (customization.target == Tingkat.Sangat)
                        dialog += "Aku suka banget yang manis, gula yang Banyak ya. ";

                    else if (customization.target == Tingkat.Sedikit)
                        dialog += "Manisnya sedikit aja, ntar diabetes. ";

                    break;


                case JenisCustomization.Gurih:

                    if (customization.target == Tingkat.Sangat)
                        dialog += "Yang gurih banget ya. ";

                    else if (customization.target == Tingkat.Lumayan)
                        dialog += "Agak gurih juga enak. ";

                    else if (customization.target == Tingkat.Sedikit)
                        dialog += "Gurihnya sedikit aja. ";

                    break;


                case JenisCustomization.Lembut:

                    if (customization.target == Tingkat.Sangat)
                        dialog += "Aku lebih suka yang lembut ya. ";

                    else if (customization.target == Tingkat.Sedikit)
                        dialog += "Jangan terlalu lembut ya. ";

                    break;


                case JenisCustomization.Isian:

                    if (customization.target == Tingkat.Sangat)
                        dialog += "Isiannya yang Banyak ya. ";

                    else if (customization.target == Tingkat.Sedikit)
                        dialog += "Isiannya sedikit aja. ";

                    break;


                
            }
        }

        dialog += "\"";

        return dialog;
    }
}