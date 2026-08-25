using System.Collections.Generic;

public static class OrderTextHelper
{
    public static string BuatTeksKeyword(OrderData data)
    {
        List<string> keyword = new List<string>();

        switch (data.targetManis)
        {
            case TingkatRasa.Sedikit: keyword.Add("Sedikit manis"); break;
            case TingkatRasa.Sedang: keyword.Add("Manis sedang"); break;
            case TingkatRasa.Banyak: keyword.Add("Sangat manis"); break;
        }

        switch (data.targetGurih)
        {
            case TingkatRasa.Sedikit: keyword.Add("Sedikit gurih"); break;
            case TingkatRasa.Sedang: keyword.Add("Gurih sedang"); break;
            case TingkatRasa.Banyak: keyword.Add("Sangat gurih"); break;
        }

        switch (data.targetLembut)
        {
            case TingkatRasa.Sedikit: keyword.Add("Sedikit lembut"); break;
            case TingkatRasa.Sedang: keyword.Add("Lembut sedang"); break;
            case TingkatRasa.Banyak: keyword.Add("Sangat lembut"); break;
        }

        switch (data.isian)
        {
            case TingkatIsian.Sedikit: keyword.Add("Isian sedikit"); break;
            case TingkatIsian.Sedang: keyword.Add("Isian sedang"); break;
            case TingkatIsian.Banyak: keyword.Add("Isian banyak"); break;
        }

        if (keyword.Count == 0) return "Original";

        return string.Join(", ", keyword);
    }

    public static string BuatTeksDialog(OrderData data)
    {
        string dialog = $"\"Aku mau pesan {data.idResep}. ";

        if (data.targetManis == TingkatRasa.Banyak) dialog += "Aku suka banget yang manis, gula yang banyak ya. ";
        else if (data.targetManis == TingkatRasa.Sedikit) dialog += "Manisnya sedikit aja, jangan giung. ";

        if (data.targetGurih == TingkatRasa.Banyak || data.targetGurih == TingkatRasa.Sedang) dialog += "Terus agak gurih juga enak. ";

        if (data.targetLembut == TingkatRasa.Banyak) dialog += "Jangan terlalu padat, aku lebih suka yang lembut.\"";
        else dialog += "\"";

        return dialog;
    }
}