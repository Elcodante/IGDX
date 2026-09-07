using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IngredientVisualConfig
{
    public IngredientData dataBahan;
    public Vector3 customScale;
    public Sprite spriteSaatBahanMasuk;
}

public class IngredientStackVisualizer : MonoBehaviour
{
    [Header("Titik Awal & Container")]
    public Transform containerStack;

    [Header("Pengaturan Tumpukan (Stacking)")]
    public Vector3 offsetPerItem = new Vector3(0f, 0.15f, 0f);

    [Tooltip("Variasi posisi acak X dan Y agar tumpukan tidak terlalu kaku/lurus")]
    public Vector2 acakPosisiX = new Vector2(-0.05f, 0.05f);
    public Vector2 acakPosisiY = new Vector2(-0.02f, 0.02f);

    [Header("Pengaturan Ukuran Default")]
    public Vector3 defaultItemScale = new Vector3(1f, 1f, 1f);

    [Header("Pengaturan Sorting Layer (Sprite 2D)")]
    public string sortingLayerName = "Default";
    public int baseSortingOrder = 10;

    [Header("Konfigurasi Khusus Bahan (Opsional)")]
    public List<IngredientVisualConfig> konfigurasiKhususBahan;

    private List<GameObject> spawnedVisualItems = new List<GameObject>();

    private void Awake()
    {
        if (containerStack == null)
        {
            containerStack = transform;
        }
    }

    public GameObject AddItemToStack(IngredientData dataBahan, Sprite spriteBahan)
    {
        // 1. Cek konfigurasi khusus terlebih dahulu
        IngredientVisualConfig configKhusus = CariConfigKhusus(dataBahan);

        // 2. Tentukan sprite: Gunakan spriteSaatBahanMasuk jika ada, jika tidak pakai spriteBahan bawaan
        Sprite spriteFinal = spriteBahan;
        if (configKhusus.dataBahan != null && configKhusus.spriteSaatBahanMasuk != null)
        {
            spriteFinal = configKhusus.spriteSaatBahanMasuk;
        }

        // Jika tidak ada sprite sama sekali, hentikan
        if (spriteFinal == null) return null;

        // 3. Buat GameObject baru untuk sprite bahan
        GameObject itemVisual = new GameObject($"Visual_{dataBahan.ingredientName}_{spawnedVisualItems.Count}");
        itemVisual.transform.SetParent(containerStack);

        // 4. Tambahkan komponen SpriteRenderer
        SpriteRenderer sr = itemVisual.AddComponent<SpriteRenderer>();
        sr.sprite = spriteFinal;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = baseSortingOrder + spawnedVisualItems.Count;

        // 5. Hitung Posisi (Tumpukan bertingkat + Acak sedikit)
        float randomX = Random.Range(acakPosisiX.x, acakPosisiX.y);
        float randomY = Random.Range(acakPosisiY.x, acakPosisiY.y);
        Vector3 hitungPosisi = (offsetPerItem * spawnedVisualItems.Count) + new Vector3(randomX, randomY, 0f);

        // 6. Hitung Ukuran (Scale)
        Vector3 targetScale = defaultItemScale;
        if (configKhusus.dataBahan != null && configKhusus.customScale != Vector3.zero)
        {
            targetScale = configKhusus.customScale;
        }

        // 7. Terapkan Posisi & Scale
        itemVisual.transform.localPosition = hitungPosisi;
        itemVisual.transform.localScale = targetScale;

        // Simpan ke list
        spawnedVisualItems.Add(itemVisual);
        return itemVisual;
    }

    public void ClearStack()
    {
        foreach (GameObject item in spawnedVisualItems)
        {
            if (item != null) Destroy(item);
        }
        spawnedVisualItems.Clear();
    }

    public int GetStackCount() => spawnedVisualItems.Count;

    private IngredientVisualConfig CariConfigKhusus(IngredientData data)
    {
        foreach (var config in konfigurasiKhususBahan)
        {
            if (config.dataBahan == data) return config;
        }
        return default;
    }
}